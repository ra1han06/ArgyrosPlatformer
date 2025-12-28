using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// WINNING TRIGGER - Sistem Kemenangan
/// 
/// Fungsi:
/// - Mendeteksi ketika player menginjak platform kemenangan (star)
/// - Freeze player (tidak bisa bergerak)
/// - Glow effect pada platform (warna emas)
/// - Camera zoom in secara smooth
/// - Play audio kemenangan
/// - Transisi ke scene Complete setelah 2 detik
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject "Winning" (platform star)
/// 2. Tambahkan BoxCollider, set Is Trigger = true
/// 3. Pastikan player punya tag "Player" dan script PlayerController
/// 4. Assign AudioClip gameWin di AudioManager Inspector
/// </summary>
public class WinningTrigger : MonoBehaviour
{
    // =====================================================
    // SETTINGS (Bisa diubah di Inspector)
    // =====================================================
    [Header("=== PLAYER SETTINGS ===")]
    [Tooltip("Tag untuk mendeteksi player (default: Player)")]
    [SerializeField] private string playerTag = "Player";

    [Header("=== GLOW SETTINGS ===")]
    [Tooltip("Warna glow platform (default: kuning emas)")]
    [SerializeField] private Color glowColor = Color.yellow;

    [Tooltip("Intensitas cahaya glow (default: 2)")]
    [SerializeField] private float glowIntensity = 2f;

    [Header("=== CAMERA ZOOM SETTINGS ===")]
    [Tooltip("FOV normal camera (default: 60)")]
    [SerializeField] private float normalFOV = 60f;

    [Tooltip("FOV saat zoom in (default: 35, lebih kecil = lebih zoom)")]
    [SerializeField] private float zoomFOV = 35f;

    [Tooltip("Durasi zoom animation dalam detik (default: 1.5)")]
    [SerializeField] private float zoomDuration = 1.5f;

    [Header("=== SCENE TRANSITION SETTINGS ===")]
    [Tooltip("Nama scene Complete yang akan di-load")]
    [SerializeField] private string completeSceneName = "Complete";

    [Tooltip("Total delay sebelum pindah scene (default: 2 detik)")]
    [SerializeField] private float transitionDelay = 2f;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug log messages")]
    [SerializeField] private bool showDebugLog = true;

    // =====================================================
    // PRIVATE VARIABLES
    // =====================================================
    private bool hasTriggered = false; // Cegah trigger berkali-kali
    private Material[] glowMaterials; // Material untuk glow effect

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    void Start()
    {
        // Setup collider otomatis jika belum ada
        SetupCollider();
    }

    // =====================================================
    // SETUP COLLIDER
    // =====================================================
    /// <summary>
    /// Setup BoxCollider otomatis jika belum ada.
    /// Set collider sebagai trigger.
    /// </summary>
    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        
        if (col == null)
        {
            // Jika belum ada collider, buat BoxCollider baru
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;
            
            if (showDebugLog)
                Debug.Log("[WinningTrigger] BoxCollider otomatis ditambahkan dan set sebagai trigger");
        }
        else
        {
            // Jika sudah ada collider, pastikan isTrigger = true
            col.isTrigger = true;
            
            if (showDebugLog)
                Debug.Log("[WinningTrigger] Collider sudah ada dan di-set sebagai trigger");
        }
    }

    // =====================================================
    // TRIGGER DETECTION
    // =====================================================
    /// <summary>
    /// Dipanggil saat ada object masuk ke trigger.
    /// Cek apakah object adalah Player.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah sudah pernah trigger
        if (hasTriggered)
            return;

        // Cek apakah yang masuk adalah Player
        if (other.CompareTag(playerTag))
        {
            if (showDebugLog)
                Debug.Log("[WinningTrigger] Player menyentuh winning platform!");

            // Set flag agar tidak trigger lagi
            hasTriggered = true;

            // Mulai sequence kemenangan
            StartCoroutine(WinSequence(other.gameObject));
        }
    }

    /// <summary>
    /// Support untuk CharacterController (collision detection).
    /// Beberapa setup player pakai collision instead of trigger.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Cek apakah sudah pernah trigger
        if (hasTriggered)
            return;

        // Cek apakah yang masuk adalah Player
        if (collision.gameObject.CompareTag(playerTag))
        {
            if (showDebugLog)
                Debug.Log("[WinningTrigger] Player menyentuh winning platform! (via Collision)");

            // Set flag agar tidak trigger lagi
            hasTriggered = true;

            // Mulai sequence kemenangan
            StartCoroutine(WinSequence(collision.gameObject));
        }
    }

    // =====================================================
    // WIN SEQUENCE COROUTINE
    // =====================================================
    /// <summary>
    /// Sequence kemenangan:
    /// 1. Complete level (stop timer & save best record)
    /// 2. Freeze player
    /// 3. Glow platform
    /// 4. Zoom camera (Coroutine)
    /// 5. Play audio
    /// 6. Delay total 2 detik
    /// 7. Load scene Complete
    /// </summary>
    private IEnumerator WinSequence(GameObject player)
    {
        // =====================================
        // STEP 0: COMPLETE LEVEL - STOP TIMER & SAVE BEST RECORD
        // =====================================
        GameManager.Instance?.CompleteLevel();
        
        // =====================================
        // STEP 1: FREEZE PLAYER
        // =====================================
        FreezePlayer(player);

        // =====================================
        // STEP 2: GLOW PLATFORM
        // =====================================
        ActivateGlowEffect();

        // =====================================
        // STEP 3: ZOOM CAMERA (Coroutine)
        // =====================================
        // Mulai zoom camera (ini akan berjalan paralel)
        StartCoroutine(ZoomCamera());

        // =====================================
        // STEP 4: PLAY AUDIO
        // =====================================
        PlayWinAudio();

        // =====================================
        // STEP 5: DELAY TOTAL 2 DETIK
        // =====================================
        if (showDebugLog)
            Debug.Log($"[WinningTrigger] Menunggu {transitionDelay} detik sebelum pindah scene...");

        yield return new WaitForSeconds(transitionDelay);

        // =====================================
        // STEP 6: LOAD SCENE COMPLETE
        // =====================================
        LoadCompleteScene();
    }

    // =====================================================
    // STEP 1: FREEZE PLAYER
    // =====================================================
    /// <summary>
    /// Freeze player agar tidak bisa bergerak.
    /// Set playerController.canMove = false.
    /// Trigger animasi kemenangan.
    /// </summary>
    private void FreezePlayer(GameObject player)
    {
        // Cari PlayerController di player GameObject
        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            // Set canMove = false
            playerController.canMove = false;

            if (showDebugLog)
                Debug.Log("[WinningTrigger] Player di-freeze (canMove = false)");
        }
        else
        {
            Debug.LogWarning("[WinningTrigger] PlayerController tidak ditemukan di Player GameObject!");
        }

        // Trigger animasi kemenangan
        PlayerAnimation playerAnimation = player.GetComponent<PlayerAnimation>();

        if (playerAnimation != null)
        {
            playerAnimation.PlayWinAnimation();

            if (showDebugLog)
                Debug.Log("[WinningTrigger] Animasi kemenangan di-trigger!");
        }
        else
        {
            Debug.LogWarning("[WinningTrigger] PlayerAnimation tidak ditemukan di Player GameObject!");
        }
    }

    // =====================================================
    // STEP 2: GLOW EFFECT
    // =====================================================
    /// <summary>
    /// Aktivasi glow effect pada platform.
    /// - Clone material (jangan pakai material asli)
    /// - Aktifkan emission
    /// - Set warna emissive ke emas
    /// </summary>
    private void ActivateGlowEffect()
    {
        // Ambil semua Renderer di object (termasuk child)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("[WinningTrigger] Tidak ada Renderer ditemukan untuk glow effect!");
            return;
        }

        // Buat array untuk simpan material baru
        glowMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer rend = renderers[i];

            // Clone material (agar tidak mengubah material asli)
            Material mat = new Material(rend.material);
            rend.material = mat; // Assign material baru
            glowMaterials[i] = mat;

            // Aktifkan emission
            mat.EnableKeyword("_EMISSION");

            // Set emissive color (warna * intensitas)
            Color emissiveColor = glowColor * glowIntensity;
            mat.SetColor("_EmissionColor", emissiveColor);

            // Set base color juga
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", glowColor);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", glowColor);
            }

            if (showDebugLog)
                Debug.Log($"[WinningTrigger] Glow effect aktif di {rend.gameObject.name}");
        }
    }

    // =====================================================
    // STEP 3: CAMERA ZOOM
    // =====================================================
    /// <summary>
    /// Zoom camera secara smooth.
    /// FOV: 60 → 35
    /// Durasi: 1.5 detik
    /// </summary>
    private IEnumerator ZoomCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("[WinningTrigger] Main Camera tidak ditemukan!");
            yield break;
        }

        float startFOV = mainCamera.fieldOfView;
        float targetFOV = zoomFOV;
        float elapsed = 0f;

        if (showDebugLog)
            Debug.Log($"[WinningTrigger] Memulai zoom camera: {startFOV} → {targetFOV}");

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            
            // Hitung progress (0 sampai 1)
            float t = elapsed / zoomDuration;

            // Gunakan SmoothStep untuk smooth easing
            // Formula: t² × (3 - 2t)
            t = t * t * (3f - 2f * t);

            // Lerp FOV
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        // Pastikan FOV tepat di target
        mainCamera.fieldOfView = targetFOV;

        if (showDebugLog)
            Debug.Log("[WinningTrigger] Zoom camera selesai!");
    }

    // =====================================================
    // STEP 4: PLAY AUDIO
    // =====================================================
    /// <summary>
    /// Play audio kemenangan via AudioManager.
    /// </summary>
    private void PlayWinAudio()
    {
        // Cek apakah AudioManager tersedia
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[WinningTrigger] AudioManager.Instance tidak ditemukan!");
            return;
        }

        // Cek apakah AudioClip gameWin sudah di-assign di AudioManager
        if (AudioManager.Instance.gameWin == null)
        {
            Debug.LogWarning("[WinningTrigger] AudioClip gameWin belum di-assign di AudioManager Inspector!");
            return;
        }

        // Play audio
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameWin);

        if (showDebugLog)
            Debug.Log("[WinningTrigger] Audio kemenangan diputar!");
    }

    // =====================================================
    // STEP 6: LOAD SCENE
    // =====================================================
    /// <summary>
    /// Load scene Complete.
    /// </summary>
    private void LoadCompleteScene()
    {
        if (showDebugLog)
            Debug.Log($"[WinningTrigger] Loading scene: {completeSceneName}");

        SceneManager.LoadScene(completeSceneName);
    }

    // =====================================================
    // GIZMOS (VISUAL DI SCENE VIEW)
    // =====================================================
    /// <summary>
    /// Tampilkan visual gizmo di Scene View untuk mudah debug.
    /// </summary>
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        
        if (col != null)
        {
            // Warna kuning transparan
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);

            // Jika BoxCollider, gambar box
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.center, boxCol.size);
            }
            // Jika SphereCollider, gambar sphere
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
        }
    }
}
