using UnityEngine;

public class PlayerPlatformInteractor : MonoBehaviour
{
    [Header("Cheat / Testing")]
    [Tooltip("Type this sequence on the keyboard to toggle Infinite Paste mode.")]
    [SerializeField] private string infinitePasteCheatCode = "iwantinvinitepaste";
    [Tooltip("Type this sequence on the keyboard to toggle Godmode (infinite copy/cut/paste limits).")]
    [SerializeField] private string godmodeCheatCode = "godmodeops";
    [Tooltip("How many recent characters to keep buffered for cheat detection.")]
    [SerializeField] private int cheatBufferSize = 32;
    [SerializeField] private bool infinitePasteEnabled = false;
    [SerializeField] private bool godmodeEnabled = false;
    private string cheatBuffer = string.Empty;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private Transform raycastPoint = null; // Optional: Drag transform untuk raycast origin (biarkan null untuk pakai offset)
    [SerializeField] private Vector3 raycastOffset = new Vector3(0, 1.2f, 0); // Offset dari player center (dipakai kalau raycastPoint null)
    [Tooltip("Lebar dan tinggi area raycast (X=lebar, Y=tinggi dari kaki-kepala, Z=depth)")]
    [SerializeField] private Vector3 boxCastSize = new Vector3(0.5f, 2f, 0.3f); // Lebar x Tinggi x Depth
    [SerializeField] private LayerMask platformLayer;

    [Header("Paste Settings")]
    [SerializeField] private float pasteOffset = 1.5f;

    [Header("Operation Limits")]
    [SerializeField] private int maxCopyCount = 1;
    [SerializeField] private int maxCutCount = 1;
    [SerializeField] private int maxPasteCount = 3;

    [Header("UI Feedback")]
    [Tooltip("Optional. If assigned, shows a toast on successful copy/cut/paste.")]
    [SerializeField] private ToastNotifier toastNotifier;

    // Current usage counters
    private int currentCopyCount = 0;
    private int currentCutCount = 0;
    private int currentPasteCount = 0;

    // Clipboard data structure to store platform information
    private class ClipboardData
    {
        public GameObject platformPrefab;
        public Vector3 scale;
        public bool isCut;
    }

    private ClipboardData clipboard;
    private bool isFacingRight = true;

    void Update()
    {
        HandleCheatCodeInput();
        UpdateFacingDirection();

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.C))
                TryCopy();
            else if (Input.GetKeyDown(KeyCode.X))
                TryCut();
            else if (Input.GetKeyDown(KeyCode.V))
                TryPaste();
        }
    }

    private void HandleCheatCodeInput()
    {
        // Unity gives us the typed characters this frame (respects shift, etc.)
        string input = Input.inputString;
        if (string.IsNullOrEmpty(input))
            return;

        // Filter to letters/numbers only to reduce accidental triggers.
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (!char.IsLetterOrDigit(c))
                continue;

            cheatBuffer += char.ToLowerInvariant(c);
        }

        if (cheatBuffer.Length > cheatBufferSize)
            cheatBuffer = cheatBuffer.Substring(cheatBuffer.Length - cheatBufferSize);

        if (!string.IsNullOrEmpty(infinitePasteCheatCode)
            && cheatBuffer.Contains(infinitePasteCheatCode.ToLowerInvariant()))
        {
            infinitePasteEnabled = !infinitePasteEnabled;
            cheatBuffer = string.Empty;

            Debug.Log($"[CHEAT] Infinite Paste: {(infinitePasteEnabled ? "ON" : "OFF")}");
            toastNotifier?.Show(infinitePasteEnabled ? "Infinite paste: ON" : "Infinite paste: OFF");
        }

        if (!string.IsNullOrEmpty(godmodeCheatCode)
            && cheatBuffer.Contains(godmodeCheatCode.ToLowerInvariant()))
        {
            godmodeEnabled = !godmodeEnabled;

            // Godmode implies infinite paste as well.
            if (godmodeEnabled)
                infinitePasteEnabled = true;

            cheatBuffer = string.Empty;

            Debug.Log($"[CHEAT] Godmode Ops: {(godmodeEnabled ? "ON" : "OFF")}");
            toastNotifier?.Show(godmodeEnabled ? "Godmode ops: ON" : "Godmode ops: OFF");
        }
    }

    private void UpdateFacingDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0) isFacingRight = true;
        else if (horizontal < 0) isFacingRight = false;
    }

    private void TryCopy()
    {
        // Check copy limit
        if (!godmodeEnabled && currentCopyCount >= maxCopyCount)
        {
            Debug.LogWarning($"Copy limit reached! ({currentCopyCount}/{maxCopyCount})");
            return;
        }

        Debug.Log("TryCopy() called - attempting to copy platform");
        
        GameObject platform = RaycastPlatform();
        
        if (platform == null)
        {
            Debug.LogWarning("No platform detected by raycast!");
            return;
        }
        
        Debug.Log($"Platform detected: {platform.name}");
        
        clipboard = new ClipboardData
        {
            platformPrefab = platform,
            scale = platform.transform.localScale,
            isCut = false
        };

        if (!godmodeEnabled)
        {
            currentCopyCount++;
        }

        Debug.Log(
            godmodeEnabled
                ? $"✓ Successfully copied platform: {platform.name} | Scale: {platform.transform.localScale} | Copies used: (GODMODE)"
                : $"✓ Successfully copied platform: {platform.name} | Scale: {platform.transform.localScale} | Copies used: {currentCopyCount}/{maxCopyCount}"
        );

        toastNotifier?.Show("Platform successfully copied");
    }

    private void TryCut()
    {
        // Check cut limit
        if (!godmodeEnabled && currentCutCount >= maxCutCount)
        {
            Debug.LogWarning($"Cut limit reached! ({currentCutCount}/{maxCutCount})");
            return;
        }

        GameObject platform = RaycastPlatform();
        if (platform == null)
        {
            Debug.LogWarning("No platform detected by raycast for cut!");
            toastNotifier?.Show("No platform to cut");
            return;
        }

        if (platform.GetComponent<CutablePlatform>())
        {
            clipboard = new ClipboardData
            {
                platformPrefab = platform,
                scale = platform.transform.localScale,
                isCut = true
            };
            platform.SetActive(false); // Hide instead of destroy immediately

            if (!godmodeEnabled)
            {
                currentCutCount++;
            }

            Debug.Log(
                godmodeEnabled
                    ? $"✂️ Cut platform: {platform.name} | Cuts used: (GODMODE)"
                    : $"✂️ Cut platform: {platform.name} | Cuts used: {currentCutCount}/{maxCutCount}"
            );

            toastNotifier?.Show("Platform successfully cut");
            return;
        }

        Debug.LogWarning($"❌ Platform '{platform.name}' tidak bisa di-cut! Platform ini tidak memiliki komponen CutablePlatform.");
        toastNotifier?.Show("This platform can't be cut");
    }

    private void TryPaste()
    {
        // Check paste limit
        if (!infinitePasteEnabled && !godmodeEnabled && currentPasteCount >= maxPasteCount)
        {
            Debug.LogWarning($"Paste limit reached! ({currentPasteCount}/{maxPasteCount})");
            return;
        }

        if (clipboard == null || clipboard.platformPrefab == null) 
        {
            Debug.LogWarning("Clipboard is empty or platform reference is null!");
            toastNotifier?.Show("Nothing to be pasted");
            return;
        }

        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        Vector3 pastePosition = transform.position + direction * pasteOffset;

        GameObject newPlatform = Instantiate(
            clipboard.platformPrefab,
            pastePosition,
            clipboard.platformPrefab.transform.rotation
        );
        newPlatform.transform.localScale = clipboard.scale;
        newPlatform.SetActive(true);

        if (!infinitePasteEnabled && !godmodeEnabled)
        {
            currentPasteCount++;
        }

        Debug.Log(
            (infinitePasteEnabled || godmodeEnabled)
                ? $"Pasted platform at position: {pastePosition} | Pastes used: (INFINITE MODE)"
                : $"Pasted platform at position: {pastePosition} | Pastes used: {currentPasteCount}/{maxPasteCount}"
        );

    toastNotifier?.Show("Platform successfully pasted");

        // If it was a cut operation, destroy the original
        if (clipboard.isCut && clipboard.platformPrefab != null)
        {
            Destroy(clipboard.platformPrefab);
        }

        // Clear clipboard after paste (for cut) or keep it (for copy)
        if (clipboard.isCut)
        {
            clipboard = null;
        }
    }

    private GameObject RaycastPlatform()
    {
        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        
        // Gunakan raycastPoint kalau ada, kalau tidak pakai offset
        Vector3 rayOrigin = raycastPoint != null ? raycastPoint.position : transform.position + raycastOffset;
        
        // BoxCast untuk detect area, bukan hanya satu titik
        Vector3 halfExtents = boxCastSize * 0.5f;
        
        Debug.DrawRay(rayOrigin, direction * raycastDistance, Color.yellow, 0.5f);
        Debug.Log($"BoxCast from {rayOrigin} | Size: {boxCastSize} | Direction: {direction} | Distance: {raycastDistance}");

        if (Physics.BoxCast(rayOrigin, halfExtents, direction, out RaycastHit hit, Quaternion.identity, raycastDistance, platformLayer, QueryTriggerInteraction.Collide))
        {
            Debug.Log($"BoxCast HIT: {hit.collider.gameObject.name} at distance {hit.distance}");
            return hit.collider.gameObject;
        }

        Debug.Log("BoxCast MISS: No platform found");
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        DrawBoxCastGizmo();
    }

    private void OnDrawGizmos()
    {
        // Selalu tampilkan gizmo (tidak perlu select GameObject)
        DrawBoxCastGizmo();
    }

    private void DrawBoxCastGizmo()
    {
        // Tentukan raycast origin - INI YANG BERUBAH SAAT OFFSET DIUBAH!
        Vector3 rayOrigin = raycastPoint != null ? raycastPoint.position : transform.position + raycastOffset;
        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        
        // 1. Player center (hijau kecil)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.08f);
        
        // 2. Box area di raycast origin (MERAH TRANSPARAN) - INI YANG GERAK!
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(rayOrigin, boxCastSize);
        
        // 3. Box outline (MERAH SOLID) - INI YANG GERAK!
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rayOrigin, boxCastSize);
        
        // 4. Garis dari player ke raycast origin (cyan) - MENUNJUKKAN OFFSET!
        if (raycastPoint == null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, rayOrigin);
        }
        
        // 5. Box area di endpoint (KUNING TRANSPARAN)
        Vector3 endPosition = rayOrigin + direction * raycastDistance;
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(endPosition, boxCastSize);
        
        // 6. Box outline di endpoint (KUNING SOLID)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(endPosition, boxCastSize);
        
        // 7. Garis penghubung (dari center box ke center box)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayOrigin, endPosition);
        
        // 8. Gambar 4 garis pinggir box untuk visualisasi sweep
        Vector3 halfSize = boxCastSize * 0.5f;
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        // Top-front edge
        Gizmos.DrawLine(rayOrigin + new Vector3(0, halfSize.y, halfSize.z), 
                       endPosition + new Vector3(0, halfSize.y, halfSize.z));
        // Top-back edge
        Gizmos.DrawLine(rayOrigin + new Vector3(0, halfSize.y, -halfSize.z), 
                       endPosition + new Vector3(0, halfSize.y, -halfSize.z));
        // Bottom-front edge
        Gizmos.DrawLine(rayOrigin + new Vector3(0, -halfSize.y, halfSize.z), 
                       endPosition + new Vector3(0, -halfSize.y, halfSize.z));
        // Bottom-back edge
        Gizmos.DrawLine(rayOrigin + new Vector3(0, -halfSize.y, -halfSize.z), 
                       endPosition + new Vector3(0, -halfSize.y, -halfSize.z));
    }

    /// <summary>
    /// Reset operation counters (call this when starting a new level)
    /// </summary>
    public void ResetLimits()
    {
        currentCopyCount = 0;
        currentCutCount = 0;
        currentPasteCount = 0;
        clipboard = null;
        Debug.Log("Operation limits reset!");
    }

    /// <summary>
    /// Set custom limits for this level (call from LevelManager)
    /// </summary>
    public void SetLimits(int copyLimit, int cutLimit, int pasteLimit)
    {
        maxCopyCount = copyLimit;
        maxCutCount = cutLimit;
        maxPasteCount = pasteLimit;
        ResetLimits();
        Debug.Log($"Limits set: Copy={copyLimit}, Cut={cutLimit}, Paste={pasteLimit}");
    }

    /// <summary>
    /// Get remaining operations for UI display
    /// </summary>
    public (int copy, int cut, int paste) GetRemainingOperations()
    {
        return (
            maxCopyCount - currentCopyCount,
            maxCutCount - currentCutCount,
            maxPasteCount - currentPasteCount
        );
    }

    /// <summary>
    /// Get current usage for UI display
    /// </summary>
    public (int copy, int cut, int paste) GetCurrentUsage()
    {
        return (currentCopyCount, currentCutCount, currentPasteCount);
    }

    /// <summary>
    /// Get max limits for UI display
    /// </summary>
    public (int copy, int cut, int paste) GetMaxLimits()
    {
        return (maxCopyCount, maxCutCount, maxPasteCount);
    }
}
