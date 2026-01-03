using UnityEngine;

/// <summary>
/// PLATFORM ACTION INDICATOR - Sistem Visual Feedback untuk Copyable/Cutable Platform
/// 
/// Fungsi:
/// - Mendeteksi apakah platform adalah CopyablePlatform (copy only) atau CutablePlatform (copy+cut)
/// - Menampilkan icon UI di atas platform saat raycast dari player mengenai platform
/// - Memberikan emission glow effect saat platform di-target oleh player raycast
/// - Warna glow berbeda: Cyan untuk Copyable, Orange untuk Cutable (copy+cut)
/// 
/// PENTING:
/// - Tidak menggunakan trigger collider - hanya raycast-based detection
/// - Dipanggil oleh PlayerPlatformInteractor saat raycast hit platform
/// - Icon UI dan glow muncul/hilang hanya berdasarkan raycast, bukan proximity
/// 
/// Cara Pakai:
/// 1. Attach script ini ke platform GameObject yang punya CopyablePlatform atau CutablePlatform script
/// 2. Setup UI Icon Canvas sebagai child GameObject (world-space canvas dengan Image component)
/// 3. Assign iconCanvas di Inspector
/// 4. PlayerPlatformInteractor akan otomatis memanggil EnableGlow/DisableGlow
/// </summary>
public class PlatformActionIndicator : MonoBehaviour
{
    // =====================================================
    // SETTINGS (Bisa diubah di Inspector)
    // =====================================================
    [Header("=== UI ICON SETTINGS ===")]
    [Tooltip("World-space Canvas GameObject yang berisi icon UI (akan di-show/hide otomatis)")]
    [SerializeField] private GameObject iconCanvas;

    [Header("=== GLOW SETTINGS - COPYABLE (Copy Only) ===")]
    [Tooltip("Warna glow untuk platform Copyable (default: cyan)")]
    [SerializeField] private Color copyableGlowColor = new Color(0f, 1f, 1f, 0.039f); // Cyan with alpha 10/255

    [Tooltip("Intensitas glow untuk Copyable platform (default: 0.01)")]
    [SerializeField] private float copyableGlowIntensity = 0.01f;

    [Header("=== GLOW SETTINGS - CUTABLE (Copy + Cut) ===")]
    [Tooltip("Warna glow untuk platform Cutable (default: orange)")]
    [SerializeField] private Color cutableGlowColor = new Color(1f, 0.5f, 0f, 0.039f); // Orange with alpha 10/255

    [Tooltip("Intensitas glow untuk Cutable platform (default: 0.01)")]
    [SerializeField] private float cutableGlowIntensity = 0.01f;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug log messages")]
    [SerializeField] private bool showDebugLog = false;

    // =====================================================
    // PRIVATE VARIABLES
    // =====================================================
    private Renderer platformRenderer;
    private Material originalMaterial;
    private Material glowMaterial;
    private bool isGlowActive = false;

    private bool isCutablePlatform = false; // true = Cutable (copy+cut), false = Copyable (copy only)
    private Color activeGlowColor;
    private float activeGlowIntensity;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    void Start()
    {
        // Deteksi tipe platform (Copyable vs Cutable)
        DetectPlatformType();

        // Setup renderer dan material
        SetupRenderer();

        // Hide icon canvas di awal
        if (iconCanvas != null)
        {
            iconCanvas.SetActive(false);
        }
    }

    // =====================================================
    // DETECT PLATFORM TYPE
    // =====================================================
    /// <summary>
    /// Deteksi apakah platform ini CopyablePlatform atau CutablePlatform.
    /// Set warna dan intensitas glow sesuai tipe.
    /// </summary>
    private void DetectPlatformType()
    {
        // Cek apakah ada CutablePlatform component
        CutablePlatform cutableComponent = GetComponent<CutablePlatform>();
        
        if (cutableComponent != null)
        {
            // Platform ini adalah Cutable (copy + cut)
            isCutablePlatform = true;
            activeGlowColor = cutableGlowColor;
            activeGlowIntensity = cutableGlowIntensity;

            if (showDebugLog)
                Debug.Log($"[PlatformActionIndicator] Platform '{gameObject.name}' terdeteksi sebagai CUTABLE (Copy+Cut) - Glow: Orange");
        }
        else
        {
            // Check apakah ada CopyablePlatform component
            CopyablePlatform copyableComponent = GetComponent<CopyablePlatform>();
            
            if (copyableComponent != null)
            {
                // Platform ini adalah Copyable (copy only)
                isCutablePlatform = false;
                activeGlowColor = copyableGlowColor;
                activeGlowIntensity = copyableGlowIntensity;

                if (showDebugLog)
                    Debug.Log($"[PlatformActionIndicator] Platform '{gameObject.name}' terdeteksi sebagai COPYABLE (Copy Only) - Glow: Cyan");
            }
            else
            {
                // Tidak ada CopyablePlatform atau CutablePlatform component
                Debug.LogWarning($"[PlatformActionIndicator] Platform '{gameObject.name}' tidak memiliki CopyablePlatform atau CutablePlatform component!");
            }
        }
    }

    // =====================================================
    // SETUP RENDERER
    // =====================================================
    /// <summary>
    /// Setup renderer dan simpan material asli.
    /// Hanya ambil main renderer (bukan child renderers).
    /// </summary>
    private void SetupRenderer()
    {
        // Ambil renderer dari GameObject ini (main renderer only, tidak include children)
        platformRenderer = GetComponent<Renderer>();

        if (platformRenderer == null)
        {
            Debug.LogWarning($"[PlatformActionIndicator] Tidak ada Renderer di platform '{gameObject.name}'!");
            return;
        }

        // Simpan reference ke material asli (shared material)
        originalMaterial = platformRenderer.sharedMaterial;

        if (showDebugLog)
            Debug.Log($"[PlatformActionIndicator] Renderer setup complete untuk '{gameObject.name}'");
    }

    // =====================================================
    // PUBLIC METHODS - CALLED BY PLAYERPLATFORMINTERACTOR
    // =====================================================
    
    /// <summary>
    /// Enable glow effect dan show icon UI.
    /// Dipanggil oleh PlayerPlatformInteractor saat raycast hit platform ini.
    /// </summary>
    public void EnableGlow()
    {
        // Jika glow sudah aktif, skip
        if (isGlowActive)
            return;

        // Show icon UI
        if (iconCanvas != null)
        {
            iconCanvas.SetActive(true);
        }

        // Activate glow effect
        if (platformRenderer != null && originalMaterial != null)
        {
            // Clone material (jangan ubah material asli)
            glowMaterial = new Material(originalMaterial);
            platformRenderer.material = glowMaterial;

            // Enable emission keyword
            glowMaterial.EnableKeyword("_EMISSION");

            // Set emissive color (warna * intensitas)
            Color emissiveColor = activeGlowColor * activeGlowIntensity;
            glowMaterial.SetColor("_EmissionColor", emissiveColor);

            // Set base color juga (optional, untuk visual yang lebih kuat)
            if (glowMaterial.HasProperty("_BaseColor"))
            {
                glowMaterial.SetColor("_BaseColor", activeGlowColor);
            }
            else if (glowMaterial.HasProperty("_Color"))
            {
                glowMaterial.SetColor("_Color", activeGlowColor);
            }

            isGlowActive = true;

            if (showDebugLog)
                Debug.Log($"[PlatformActionIndicator] Glow ENABLED untuk '{gameObject.name}' - {(isCutablePlatform ? "Orange (Cutable)" : "Cyan (Copyable)")}");
        }
    }

    /// <summary>
    /// Disable glow effect dan hide icon UI.
    /// Dipanggil oleh PlayerPlatformInteractor saat raycast tidak lagi hit platform ini.
    /// </summary>
    public void DisableGlow()
    {
        // Jika glow tidak aktif, skip
        if (!isGlowActive)
            return;

        // Hide icon UI
        if (iconCanvas != null)
        {
            iconCanvas.SetActive(false);
        }

        // Restore original material
        if (platformRenderer != null && originalMaterial != null)
        {
            platformRenderer.material = originalMaterial;

            // Destroy cloned material untuk free up memory
            if (glowMaterial != null)
            {
                Destroy(glowMaterial);
                glowMaterial = null;
            }

            isGlowActive = false;

            if (showDebugLog)
                Debug.Log($"[PlatformActionIndicator] Glow DISABLED untuk '{gameObject.name}'");
        }
    }

    // =====================================================
    // UNITY LIFECYCLE: ONDESTROY
    // =====================================================
    /// <summary>
    /// Cleanup saat object dihancurkan.
    /// Pastikan glowMaterial di-destroy untuk avoid memory leak.
    /// </summary>
    void OnDestroy()
    {
        if (glowMaterial != null)
        {
            Destroy(glowMaterial);
            glowMaterial = null;
        }
    }

    // =====================================================
    // PUBLIC GETTERS
    // =====================================================
    
    /// <summary>
    /// Cek apakah platform ini Cutable (copy+cut) atau Copyable (copy only).
    /// </summary>
    public bool IsCutablePlatform()
    {
        return isCutablePlatform;
    }

    /// <summary>
    /// Cek apakah glow sedang aktif.
    /// </summary>
    public bool IsGlowActive()
    {
        return isGlowActive;
    }
}
