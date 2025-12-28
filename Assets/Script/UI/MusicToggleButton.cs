using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MUSIC TOGGLE BUTTON (COLOR VERSION)
/// 
/// Script untuk tombol toggle musik ON/OFF di Settings Scene.
/// Menggunakan WARNA untuk visual feedback (gelap saat OFF).
/// State musik disimpan dengan PlayerPrefs dan sinkron ke semua scene.
/// 
/// Fitur:
/// - Toggle musik on/off saat diklik (BGM untuk UI dan Level)
/// - Visual feedback: button jadi gelap saat musik off
/// - State tersimpan di PlayerPrefs, persist antar session
/// - Sinkron otomatis dengan MusicToggleIconButton di Level Scene
/// 
/// Setup:
/// 1. Attach ke Button GameObject (di Settings Scene)
/// 2. Set Enabled Color (putih) dan Disabled Color (abu-abu gelap)
/// 3. Button onClick sudah otomatis (lewat AddListener)
/// 
/// CATATAN:
/// - Hanya mengontrol BGM (Background Music), SFX tidak terpengaruh
/// - Cutscene BGM punya kontrol sendiri, tidak dipengaruhi toggle ini
/// </summary>
[RequireComponent(typeof(Button))]
public class MusicToggleButton : MonoBehaviour
{
    // =====================================================
    // KOMPONEN
    // =====================================================
    private Button button;
    private Image buttonImage;

    // =====================================================
    // VISUAL SETTINGS
    // =====================================================
    [Header("=== VISUAL SETTINGS ===")]
    [Tooltip("Warna button saat musik ON (default: putih/normal)")]
    public Color enabledColor = Color.white;

    [Tooltip("Warna button saat musik OFF (gelap)")]
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Abu-abu gelap

    // =====================================================
    // UNITY LIFECYCLE: AWAKE & START
    // =====================================================
    void Awake()
    {
        // Get komponen Button dan Image
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
    }

    void Start()
    {
        // Tambahkan listener untuk button click
        button.onClick.AddListener(OnMusicButtonClicked);
        
        // Update visual sesuai status BGM dari AudioManager
        UpdateVisual();
        
        // Debug log
        if (AudioManager.Instance != null)
        {
            bool musicStatus = AudioManager.Instance.IsBGMEnabled();
            Debug.Log($"[MusicToggleButton] Start - Music Status: {(musicStatus ? "ON" : "OFF")}");
        }
    }

    void OnEnable()
    {
        // PENTING: Update visual setiap kali scene/panel dibuka
        // Memastikan button selalu sync dengan state global
        if (buttonImage != null)
        {
            Invoke(nameof(DelayedUpdateVisual), 0.1f);
        }
    }
    
    /// <summary>
    /// Update visual dengan delay untuk memastikan AudioManager ready
    /// </summary>
    private void DelayedUpdateVisual()
    {
        UpdateVisual();
        
        if (AudioManager.Instance != null)
        {
            bool musicStatus = AudioManager.Instance.IsBGMEnabled();
            Debug.Log($"[MusicToggleButton] OnEnable (Delayed) - Music Status: {(musicStatus ? "ON" : "OFF")}");
        }
    }

    // =====================================================
    // ON MUSIC BUTTON CLICKED
    // =====================================================
    /// <summary>
    /// Dipanggil saat button diklik.
    /// Toggle musik on/off.
    /// </summary>
    private void OnMusicButtonClicked()
    {
        if (AudioManager.Instance == null) return;

        // Play button SFX
        AudioManager.Instance.PlayButtonSFX(true);

        // Toggle BGM via AudioManager (otomatis save ke PlayerPrefs)
        bool isMusicOn = AudioManager.Instance.ToggleBGM();

        // Update visual button
        UpdateVisual();

        Debug.Log($"[MusicToggleButton] Musik toggled: {(isMusicOn ? "ON" : "OFF")}");
    }

    // =====================================================
    // UPDATE BUTTON VISUAL
    // =====================================================
    /// <summary>
    /// Update tampilan button berdasarkan state musik dari AudioManager global
    /// </summary>
    private void UpdateVisual()
    {
        if (buttonImage == null || AudioManager.Instance == null) return;

        // Baca status musik dari AudioManager
        bool isMusicOn = AudioManager.Instance.IsBGMEnabled();
        buttonImage.color = isMusicOn ? enabledColor : disabledColor;
    }

    // =====================================================
    // UNITY LIFECYCLE: ON DESTROY
    // =====================================================
    void OnDestroy()
    {
        // Remove listener saat object dihancurkan
        if (button != null)
        {
            button.onClick.RemoveListener(OnMusicButtonClicked);
        }
    }
}
