using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// TUTORIAL WINNING TRIGGER - Sistem Kemenangan untuk Tutorial Scene
/// 
/// Fungsi:
/// - Sama seperti WinningTrigger tapi load scene TutorialCompleted instead of Complete
/// - Mendeteksi ketika player menginjak platform kemenangan (star)
/// - Freeze player (tidak bisa bergerak)
/// - Glow effect pada platform (warna emas)
/// - Camera zoom in secara smooth
/// - Play audio kemenangan
/// - Transisi ke scene TutorialCompleted setelah 2 detik
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject "Winning" (platform star) di TUTORIAL SCENE
/// 2. Tambahkan BoxCollider, set Is Trigger = true
/// 3. Pastikan player punya tag "Player" dan script PlayerController
/// 4. Assign AudioClip gameWin di AudioManager Inspector
/// </summary>
public class TutorialWinningTrigger : MonoBehaviour
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
    [Tooltip("Nama scene TutorialCompleted yang akan di-load")]
    [SerializeField] private string tutorialCompletedSceneName = "TutorialCompleted";

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
                Debug.Log("[TutorialWinningTrigger] BoxCollider otomatis ditambahkan dan set sebagai trigger");
        }
        else
        {
            // Jika sudah ada collider, pastikan isTrigger = true
            col.isTrigger = true;
            
            if (showDebugLog)
                Debug.Log("[TutorialWinningTrigger] Collider sudah ada dan di-set sebagai trigger");
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
                Debug.Log("[TutorialWinningTrigger] Player menyentuh winning platform di Tutorial!");

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
                Debug.Log("[TutorialWinningTrigger] Player menyentuh winning platform di Tutorial! (via Collision)");

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
    /// Sequence kemenangan untuk Tutorial:
    /// 1. Freeze player
    /// 2. Glow platform
    /// 3. Zoom camera (Coroutine)
    /// 4. Play audio
    /// 5. Delay total 2 detik
    /// 6. Load scene TutorialCompleted
    /// 
    /// NOTE: Tidak memanggil GameManager.CompleteLevel() karena tutorial
    /// tidak perlu simpan stats/best records
    /// </summary>
    private IEnumerator WinSequence(GameObject player)
    {
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
            Debug.Log($"[TutorialWinningTrigger] Menunggu {transitionDelay} detik sebelum pindah scene...");

        yield return new WaitForSeconds(transitionDelay);

        // =====================================
        // STEP 6: LOAD SCENE TUTORIALCOMPLETED
        // =====================================
        LoadTutorialCompletedScene();
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
                Debug.Log("[TutorialWinningTrigger] Player di-freeze (canMove = false)");
        }
        else
        {
            Debug.LogWarning("[TutorialWinningTrigger] PlayerController tidak ditemukan di Player GameObject!");
        }

        // Trigger animasi kemenangan
        PlayerAnimation playerAnimation = player.GetComponent<PlayerAnimation>();

        if (playerAnimation != null)
        {
            playerAnimation.PlayWinAnimation();

            if (showDebugLog)
                Debug.Log("[TutorialWinningTrigger] Animasi kemenangan di-trigger!");
        }
        else
        {
            Debug.LogWarning("[TutorialWinningTrigger] PlayerAnimation tidak ditemukan di Player GameObject!");
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
            Debug.LogWarning("[TutorialWinningTrigger] Tidak ada Renderer ditemukan untuk glow effect!");
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
                Debug.Log($"[TutorialWinningTrigger] Glow effect aktif di {rend.gameObject.name}");
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
            Debug.LogWarning("[TutorialWinningTrigger] Main Camera tidak ditemukan!");
            yield break;
        }

        float startFOV = mainCamera.fieldOfView;
        float targetFOV = zoomFOV;
        float elapsed = 0f;

        if (showDebugLog)
            Debug.Log($"[TutorialWinningTrigger] Memulai zoom camera: {startFOV} → {targetFOV}");

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
            Debug.Log("[TutorialWinningTrigger] Zoom camera selesai!");
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
            Debug.LogWarning("[TutorialWinningTrigger] AudioManager.Instance tidak ditemukan!");
            return;
        }

        // Cek apakah AudioClip gameWin sudah di-assign di AudioManager
        if (AudioManager.Instance.gameWin == null)
        {
            Debug.LogWarning("[TutorialWinningTrigger] AudioClip gameWin belum di-assign di AudioManager Inspector!");
            return;
        }

        // Play audio
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameWin);

        if (showDebugLog)
            Debug.Log("[TutorialWinningTrigger] Audio kemenangan diputar!");
    }

    // =====================================================
    // STEP 6: LOAD SCENE
    // =====================================================
    /// <summary>
    /// Load scene TutorialCompleted.
    /// </summary>
    private void LoadTutorialCompletedScene()
    {
        if (showDebugLog)
            Debug.Log($"[TutorialWinningTrigger] Loading scene: {tutorialCompletedSceneName}");

        // CRITICAL: Reset Time.timeScale sebelum load TutorialCompleted scene
        // Ini mencegah bug stuck time dari cutscene/pause
        Time.timeScale = 1f;

        SceneManager.LoadScene(tutorialCompletedSceneName);
    }
}
