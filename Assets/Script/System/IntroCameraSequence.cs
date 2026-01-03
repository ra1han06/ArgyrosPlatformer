using System.Collections;
using UnityEngine;

/// <summary>
/// Camera intro sequence (no Cinemachine):
/// 1) Focus finish/goal
/// 2) Zoom out to show map
/// 3) Move to player then hand off to CameraFollow
/// </summary>
[DisallowMultipleComponent]
public class IntroCameraSequence : MonoBehaviour
{
    // =====================================================
    // STATIC EVENT - Sequence Complete
    // =====================================================
    /// <summary>
    /// Event dipanggil ketika intro camera sequence selesai.
    /// GameManager bisa subscribe ke event ini untuk start timer.
    /// </summary>
    public static event System.Action OnSequenceComplete;

    // NOTE: If you see a compile error mentioning CameraFollow's private offset field here,
    // it is from an older version; this script no longer touches it.
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private Transform player;
    [SerializeField] private Transform finish;

    [Header("Auto-Resolve (fallback)")]
    [Tooltip("If Player reference is empty, try to find via tag then by name.")]
    [SerializeField] private bool autoFindPlayer = true;

    [Tooltip("If Finish reference is empty, try to find by this GameObject name.")]
    [SerializeField] private string finishObjectName = "PlatformStar_001";

    [Header("Timings (seconds)")]
    [SerializeField] private float focusFinishDuration = 1.2f;
    [SerializeField] private float zoomOutDuration = 1.0f;
    [SerializeField] private float moveToPlayerDuration = 1.2f;

    [Header("Optional Holds (seconds)")]
    [SerializeField] private float holdOnFinish = 0.2f;
    [SerializeField] private float holdOnWide = 0.2f;

    [Header("Trace Pan (optional)")]
    [Tooltip("If enabled, after the wide shot the camera will pan in a tracing pattern (right, down, left, down, ...).")]
    [SerializeField] private bool enableTracePan = false;

    [Tooltip("If enabled, trace pan uses an explicit ordered list of waypoint GameObject names instead of the right/down pattern.")]
    [SerializeField] private bool traceUseWaypoints = true;

    [Tooltip("Ordered list of GameObject names to visit during the intro trace (finish is handled separately). If empty, the pattern trace is used.")]
    [SerializeField] private string[] traceWaypointNames =
    {
        "PlatformSmall_004 (11)",
        "PlatformSmall_004 (2)",
        "Hazard_PlatformSmall_002 (3)",
        "RockSmall_003 (5)",
        "PlatformSmall_004 (1)",
        "RockSmall_003 (4)",
    };

    [Tooltip("Offset applied when moving to each trace waypoint (world space).")]
    [SerializeField] private Vector3 traceWaypointWorldOffset = Vector3.zero;

    [Tooltip("Extra zoom-out offset applied on top of each waypoint (world space). Use +Y to lift the camera further away from the ground.")]
    [SerializeField] private Vector3 traceExtraZoomOutOffset = new Vector3(0f, 15f, 0f);

    [Tooltip("If > 0, force FOV during the waypoint trace (lets you zoom out further than the waypoint alone).")]
    [SerializeField] private float traceWaypointFovOverride = 0f;

    [Tooltip("Duration per trace segment (seconds).")]
    [SerializeField] private float traceSegmentDuration = 0.5f;

    [Tooltip("Hold time between trace segments (seconds).")]
    [SerializeField] private float traceHoldBetweenSegments = 0.05f;

    [Tooltip("How much to move horizontally per segment (world units). If 0, it will be derived from wide bounds.")]
    [SerializeField] private float traceStepX = 0f;

    [Tooltip("How much to move down vertically per segment (world units). If 0, it will be derived from wide bounds.")]
    [SerializeField] private float traceStepY = 0f;

    [Header("Camera Settings")]
    [Tooltip("Smaller FOV = more zoomed in. Used when focusing the finish.")]
    [SerializeField] private float finishFov = 35f;

    [Tooltip("Bigger FOV = more zoomed out. Used for the wide/map view.")]
    [SerializeField] private float wideFov = 75f;

    [Tooltip("FOV used when handing control back to CameraFollow.")]
    [SerializeField] private float gameplayFov = 60f;

    [Header("Gameplay Framing (handoff)")]
    [Tooltip("Extra world-space offset added when returning to the player at the end of the intro. Use +Y to pull the camera slightly back / show more level.")]
    [SerializeField] private Vector3 gameplayWorldOffset = Vector3.zero;

    [Tooltip("If > 0, overrides the FOV used for normal gameplay after the intro (slight zoom-out helper). Leave 0 to use gameplayFov.")]
    [SerializeField] private float gameplayFovOverride = 0f;

    [Tooltip("Extra offset used while focusing the finish (in world space).")]
    [SerializeField] private Vector3 finishWorldOffset = new Vector3(0f, 6f, 0f);

    [Tooltip("Extra offset used for the wide/map view (in world space).")]
    [SerializeField] private Vector3 wideWorldOffset = new Vector3(0f, 25f, 0f);

    [Header("Wide Shot Framing")]
    [Tooltip("Try to frame the whole platform layout (all Renderers on this layer).")]
    [SerializeField] private bool wideShotFramePlatforms = true;

    [Header("Wide Shot Anchors (optional)")]
    [Tooltip("If set, the wide framing will always include this left-most object by name.")]
    [SerializeField] private string wideLeftAnchorObjectName = "Hazard_PlatformSmall_002";

    [Tooltip("If set, the wide framing will always include this right-most object by name.")]
    [SerializeField] private string wideRightAnchorObjectName = "PlatformSmall_004";

    [Header("Wide Shot Fixed Target (optional)")]
    [Tooltip("If enabled, the wide shot moves to a fixed camera position (useful once you've found a view you like in the Game window).")]
    [SerializeField] private bool useFixedWideShot = false;

    [Tooltip("Fixed wide shot camera position. Default is taken from your screenshot.")]
    [SerializeField] private Vector3 fixedWideShotPosition = new Vector3(-10.88f, 74.45f, -33.06f);

    [Tooltip("Layer used by platforms in this project (matches PlayerPlatformInteractor.platformLayer = 64).")]
    [SerializeField] private LayerMask platformLayer = 1 << 6;

    [Tooltip("Extra padding when framing bounds. 1.0 = tight, 1.2 = 20% wider.")]
    [SerializeField] private float wideBoundsPadding = 1.15f;

    [Tooltip("Extra vertical bias applied to the wide framing center. Use a small negative value if you want to see more of the ground/platforms.")]
    [SerializeField] private float wideCenterYBias = 0f;

    [Tooltip("Clamp wide FOV to keep it reasonable.")]
    [SerializeField] private Vector2 wideFovClamp = new Vector2(35f, 90f);

    [Tooltip("Minimum camera-to-target Z distance used for FOV framing (prevents needing extreme FOV if Z distance is tiny).")]
    [SerializeField] private float minWideDistance = 6f;

    [Tooltip("When driving the camera manually, keep Z fixed to current (helps 2.5D setups).")]
    [SerializeField] private bool lockZToInitial = true;

    [Header("Behavior")]
    [SerializeField] private bool playOnStart = true;

    private float _initialZ;
    private bool _played;
    private Quaternion _initialRotation;
    private Vector3 _gameplayDelta;

    private void Reset()
    {
        targetCamera = GetComponent<Camera>();
        cameraFollow = GetComponent<CameraFollow>();
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Awake()
    {
        Debug.Log("[IntroCameraSequence] Awake() called!");
        
        // Always prefer the Camera on THIS GameObject (Main Camera).
        // This avoids accidentally driving a different camera.
        targetCamera = GetComponent<Camera>();
        if (targetCamera == null) targetCamera = Camera.main;

        if (cameraFollow == null)
        {
            cameraFollow = GetComponent<CameraFollow>();
        }

        if (autoFindPlayer && player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else
            {
                var byName = GameObject.Find("Player");
                if (byName != null) player = byName.transform;
            }
        }

        if (finish == null && !string.IsNullOrWhiteSpace(finishObjectName))
        {
            var f = GameObject.Find(finishObjectName);
            if (f != null) finish = f.transform;
        }

        if (targetCamera != null)
        {
            _initialZ = targetCamera.transform.position.z;
            _initialRotation = targetCamera.transform.rotation;
        }
        
        Debug.Log($"[IntroCameraSequence] Awake complete. targetCamera={targetCamera?.name}, player={player?.name}, finish={finish?.name}");
    }

    private void Start()
    {
        Debug.Log("[IntroCameraSequence] Start() called. playOnStart = " + playOnStart);
        
        if (playOnStart)
        {
            TryPlay();
        }
    }

    public void TryPlay()
    {
        if (_played) return;

        Debug.Log("[IntroCameraSequence] TryPlay() called - starting intro sequence...");

        // Last-chance resolve in case objects spawn/enable order is weird.
        if (autoFindPlayer && player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else
            {
                var byName = GameObject.Find("Player");
                if (byName != null) player = byName.transform;
            }
        }

        if (finish == null && !string.IsNullOrWhiteSpace(finishObjectName))
        {
            var f = GameObject.Find(finishObjectName);
            if (f != null) finish = f.transform;
        }

        if (targetCamera == null || player == null || finish == null)
        {
            Debug.LogWarning("IntroCameraSequence missing references; skipping intro.", this);
            return;
        }

        _played = true;
        Debug.Log("[IntroCameraSequence] Starting PlayRoutine coroutine...");
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (targetCamera == null) yield break;

        if (cameraFollow != null) cameraFollow.enabled = false;

        var camT = targetCamera.transform;

        // Resolve the player root without relying on CameraFollow internals.
        Transform playerRoot = player;
        if (autoFindPlayer)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerRoot = p.transform;
            else
            {
                var byName = GameObject.Find("Player");
                if (byName != null) playerRoot = byName.transform;
            }
        }

        // Capture gameplay framing delta BEFORE we move the camera around.
        // Prefer CameraFollow.Offset so final handoff matches gameplay.
        if (cameraFollow != null) _gameplayDelta = cameraFollow.Offset;
        else _gameplayDelta = camT.position - (playerRoot != null ? playerRoot.position : player.position);

    // Finish focus (optional zoom-in)
    Vector3 finishPos = finish.position + finishWorldOffset;
        finishPos = ApplyZLock(finishPos);
        yield return MoveAndFov(camT, finishPos, finishFov, focusFinishDuration);
        if (holdOnFinish > 0f) yield return new WaitForSeconds(holdOnFinish);

        // After finish focus, go straight into tracing (no initial wide/map zoom-out step).
        if (enableTracePan)
        {
            if (traceUseWaypoints && traceWaypointNames != null && traceWaypointNames.Length > 0)
            {
                yield return PlayWaypointTrace(camT, gameplayFov);
            }
            else
            {
                // Start pattern trace from current position.
                yield return PlayTracePan(camT, camT.position, gameplayFov);
            }

            if (holdOnWide > 0f) yield return new WaitForSeconds(holdOnWide);
        }

    // Move to player and restore gameplay framing.
        Vector3 rootPos = playerRoot != null ? playerRoot.position : player.position;
    Vector3 playerPos = rootPos + _gameplayDelta + gameplayWorldOffset;
        playerPos = ApplyZLock(playerPos);

    float finalFov = gameplayFovOverride > 0f ? gameplayFovOverride : gameplayFov;

        // Restore the original gameplay rotation so we don't end the intro looking at weird angles.
        camT.rotation = _initialRotation;

    yield return MoveAndFov(camT, playerPos, finalFov, moveToPlayerDuration);

        if (cameraFollow != null) cameraFollow.enabled = true;

        // ✅ Intro camera sequence selesai - notify subscribers (GameManager)
        Debug.Log("[IntroCameraSequence] ✅ Sequence complete! Invoking OnSequenceComplete event...");
        OnSequenceComplete?.Invoke();
    }

    private Vector3 ApplyZLock(Vector3 pos)
    {
        if (!lockZToInitial) return pos;
        pos.z = _initialZ;
        return pos;
    }

    private IEnumerator PlayTracePan(Transform camT, Vector3 startPos, float fov)
    {
        // Derive step sizes from bounds if requested.
        float stepX = traceStepX;
        float stepY = traceStepY;

        if ((stepX <= 0f || stepY <= 0f) && TryGetWideBounds(out var b))
        {
            // Use a fraction of the level extents to create a nice "scan".
            if (stepX <= 0f) stepX = Mathf.Max(1f, b.extents.x * 0.9f);
            if (stepY <= 0f) stepY = Mathf.Max(1f, b.extents.y * 0.35f);
        }

        if (stepX <= 0f) stepX = 10f;
        if (stepY <= 0f) stepY = 5f;

        // Pattern: right, down, left, down, right, down, left
        Vector3[] deltas =
        {
            new Vector3(+stepX, 0f, 0f),
            new Vector3(0f, -stepY, 0f),
            new Vector3(-stepX, 0f, 0f),
            new Vector3(0f, -stepY, 0f),
            new Vector3(+stepX, 0f, 0f),
            new Vector3(0f, -stepY, 0f),
            new Vector3(-stepX, 0f, 0f),
        };

        Vector3 pos = startPos;
        pos = ApplyZLock(pos);

        for (int i = 0; i < deltas.Length; i++)
        {
            Vector3 next = pos + deltas[i];
            next = ApplyZLock(next);
            yield return MoveAndFov(camT, next, fov, traceSegmentDuration);
            pos = next;
            if (traceHoldBetweenSegments > 0f) yield return new WaitForSeconds(traceHoldBetweenSegments);
        }
    }

    private IEnumerator PlayWaypointTrace(Transform camT, float fov)
    {
        // Waypoints are for panning only; keep FOV stable.
        float useFov = gameplayFov;

        for (int i = 0; i < traceWaypointNames.Length; i++)
        {
            string n = traceWaypointNames[i];
            if (string.IsNullOrWhiteSpace(n)) continue;

            var go = FindGameObjectByNameLoose(n);
            if (go == null)
            {
                Debug.LogWarning($"[IntroCameraSequence] Trace waypoint not found: '{n}'", this);
                continue;
            }

            // Waypoints are anchors; we still apply extra offset so the camera is more zoomed out than the blocks.
            Vector3 pos = go.transform.position + traceWaypointWorldOffset + traceExtraZoomOutOffset;
            pos = ApplyZLock(pos);
            yield return MoveAndFov(camT, pos, useFov, traceSegmentDuration);
            if (traceHoldBetweenSegments > 0f) yield return new WaitForSeconds(traceHoldBetweenSegments);
        }
    }

    private static GameObject FindGameObjectByNameLoose(string name)
    {
        // Exact first
        var exact = GameObject.Find(name);
        if (exact != null) return exact;

        // Fallback: starts-with match (useful when instance suffix changes).
        var all = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            var t = all[i];
            if (t == null) continue;
            if (t.name.StartsWith(name)) return t.gameObject;
        }

        return null;
    }

    private bool TryGetWideBounds(out Bounds bounds)
    {
        bounds = new Bounds();

        // 1) If anchors exist, use them to force the wide shot to include the full level width.
        bool hasAnchorBounds = false;
        if (!string.IsNullOrWhiteSpace(wideLeftAnchorObjectName))
        {
            var go = GameObject.Find(wideLeftAnchorObjectName);
            if (go != null && TryGetBoundsForGameObject(go, out var b))
            {
                bounds = b;
                hasAnchorBounds = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(wideRightAnchorObjectName))
        {
            var go = GameObject.Find(wideRightAnchorObjectName);
            if (go != null && TryGetBoundsForGameObject(go, out var b))
            {
                if (!hasAnchorBounds)
                {
                    bounds = b;
                    hasAnchorBounds = true;
                }
                else
                {
                    bounds.Encapsulate(b);
                }
            }
        }

        if (hasAnchorBounds)
        {
            // Expand using layer scan too, so we still include vertical extremes, hazards, etc.
            if (TryGetPlatformBounds(out var layerBounds))
            {
                bounds.Encapsulate(layerBounds);
            }
            return true;
        }

        // 2) Fallback: scan by platform layer.
        return TryGetPlatformBounds(out bounds);
    }

    private bool TryGetPlatformBounds(out Bounds bounds)
    {
        bounds = new Bounds();

        // Prefer Renderers, but fall back to Colliders in case platforms are invisible (collider-only).
        bool hasAny = false;
        int platformLayerIndex = LayerMaskToLayerIndex(platformLayer);

        var renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (var r in renderers)
        {
            if (r == null || r.gameObject == null) continue;
            if (platformLayerIndex >= 0 && r.gameObject.layer != platformLayerIndex) continue;
            if (!r.enabled) continue;

            if (!hasAny)
            {
                bounds = r.bounds;
                hasAny = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        if (!hasAny)
        {
            var col2d = Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
            foreach (var c in col2d)
            {
                if (c == null || c.gameObject == null) continue;
                if (platformLayerIndex >= 0 && c.gameObject.layer != platformLayerIndex) continue;
                if (!c.enabled) continue;

                if (!hasAny)
                {
                    bounds = c.bounds;
                    hasAny = true;
                }
                else
                {
                    bounds.Encapsulate(c.bounds);
                }
            }
        }

        if (!hasAny)
        {
            var col3d = Object.FindObjectsByType<Collider>(FindObjectsSortMode.None);
            foreach (var c in col3d)
            {
                if (c == null || c.gameObject == null) continue;
                if (platformLayerIndex >= 0 && c.gameObject.layer != platformLayerIndex) continue;
                if (!c.enabled) continue;

                if (!hasAny)
                {
                    bounds = c.bounds;
                    hasAny = true;
                }
                else
                {
                    bounds.Encapsulate(c.bounds);
                }
            }
        }

        return hasAny;
    }

    private static bool TryGetBoundsForGameObject(GameObject go, out Bounds bounds)
    {
        // Prefer renderer, then 2D collider, then 3D collider.
        var r = go.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            bounds = r.bounds;
            return true;
        }

        var c2d = go.GetComponentInChildren<Collider2D>();
        if (c2d != null)
        {
            bounds = c2d.bounds;
            return true;
        }

        var c3d = go.GetComponentInChildren<Collider>();
        if (c3d != null)
        {
            bounds = c3d.bounds;
            return true;
        }

        bounds = default;
        return false;
    }

    private static int LayerMaskToLayerIndex(LayerMask mask)
    {
        int value = mask.value;
        if (value == 0) return -1;

        // If multiple bits set, we can't represent it as a single layer index.
        if ((value & (value - 1)) != 0) return -1;

        int index = 0;
        while (value > 1)
        {
            value >>= 1;
            index++;
        }
        return index;
    }

    private (Vector3 pos, float fov) ComputeWideShotFromBounds(Bounds b, Vector3 currentCamPos)
    {
        // Keep camera Z fixed (2.5D). Center shot on the middle of all platforms.
        Vector3 center = b.center;
        center.y += wideCenterYBias;
        center = ApplyZLock(center);

        // Use current distance to target (along Z) to avoid pushing camera "up".
    float distance = Mathf.Abs(currentCamPos.z - center.z);
    distance = Mathf.Max(distance, minWideDistance);

        // Compute vertical FOV that fits both vertical and horizontal extents.
        // Based on perspective projection:
        // halfHeight = distance * tan(fov/2)
        // fov = 2 * atan(halfHeight / distance)
        float halfHeight = b.extents.y * wideBoundsPadding;
        float aspect = targetCamera != null ? targetCamera.aspect : 16f / 9f;
        float halfWidth = b.extents.x * wideBoundsPadding;

        // Convert width requirement into vertical-half-height requirement.
        float requiredHalfHeightForWidth = (aspect > 0.0001f) ? (halfWidth / aspect) : halfWidth;
        float requiredHalfHeight = Mathf.Max(halfHeight, requiredHalfHeightForWidth);

        float fovRad = 2f * Mathf.Atan(requiredHalfHeight / distance);
        float fovDeg = Mathf.Rad2Deg * fovRad;
        fovDeg = Mathf.Clamp(fovDeg, wideFovClamp.x, wideFovClamp.y);

        // Position: keep same Z, just move X/Y to center (no "fly up" effect).
        Vector3 pos = new Vector3(center.x, center.y, currentCamPos.z);
        return (pos, fovDeg);
    }

    private IEnumerator MoveAndFov(Transform camT, Vector3 targetPos, float targetFov, float duration)
    {
        if (duration <= 0f)
        {
            camT.position = targetPos;
            if (targetCamera != null) targetCamera.fieldOfView = targetFov;
            yield break;
        }

        Vector3 startPos = camT.position;
        float startFov = targetCamera != null ? targetCamera.fieldOfView : targetFov;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / duration);

            // Smoothstep-ish
            a = a * a * (3f - 2f * a);

            camT.position = Vector3.Lerp(startPos, targetPos, a);
            if (targetCamera != null) targetCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, a);

            yield return null;
        }

        camT.position = targetPos;
        if (targetCamera != null) targetCamera.fieldOfView = targetFov;
    }
}
