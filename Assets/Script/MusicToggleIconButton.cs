using UnityEngine;
using UnityEngine.UI;

// FORCE RECOMPILE FLAG - Version 2.0.0

/// <summary>
/// MUSIC TOGGLE ICON BUTTON
/// Script untuk button ON/OFF music dengan WARNA GELAP di Level Scene
/// 
/// SETUP DI INSPECTOR:
/// 1. Buat UI Button untuk toggle music di Level Scene
/// 2. Pasang script ini ke GameObject Button
/// 3. Set Enabled Color (putih/normal) dan Disabled Color (abu-abu gelap)
/// 4. Di Button onClick:
///    - Tambahkan event → pilih MusicToggleIconButton → ToggleMusic()
/// 
/// CATATAN:
/// - Button ini terhubung dengan Settings (sinkron via AudioManager + PlayerPrefs)
/// - Button akan GELAP saat musik OFF, normal saat musik ON
/// - Hanya mempengaruhi BGM (Background Music), SFX tidak terpengaruh
/// - Cutscene BGM punya kontrol sendiri, tidak dipengaruhi toggle ini
/// </summary>
public class MusicToggleIconButton : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("Warna button saat musik ON (default: putih/normal)")]
    public Color enabledColor = Color.white;

    [Tooltip("Warna button saat musik OFF (gelap)")]
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Abu-abu gelap

    private Image buttonImage;
    private UnityEngine.UI.Button button;

    void Awake()
    {
        Debug.Log("[MusicToggleIconButton] ===== AWAKE CALLED =====");
        // Ambil komponen Image dan Button
        buttonImage = GetComponent<Image>();
        button = GetComponent<UnityEngine.UI.Button>();
        
        if (button == null)
        {
            Debug.LogError("[MusicToggleIconButton] ❌ Button component TIDAK DITEMUKAN!");
        }
        else
        {
            Debug.Log($"[MusicToggleIconButton] ✅ Button component found: {button.name}");
        }
    }

    void Start()
    {
        Debug.Log("[MusicToggleIconButton] ===== START CALLED =====");
        
        // PENTING: Tambahkan listener untuk button click
        if (button != null)
        {
            button.onClick.AddListener(ToggleMusic);
            Debug.Log("[MusicToggleIconButton] ✅ Button listener added");
        }
        else
        {
            Debug.LogError("[MusicToggleIconButton] ❌ Cannot add listener - button is NULL!");
        }
        
        // Update visual sesuai status music saat scene dimulai
        UpdateVisual();
        
        // Debug log untuk verifikasi
        if (AudioManager.Instance != null)
        {
            bool musicStatus = AudioManager.Instance.IsBGMEnabled();
            Debug.Log($"[MusicToggleIconButton] Start - Music Status: {(musicStatus ? "ON" : "OFF")}");
        }
    }

    void OnEnable()
    {
        // PENTING: Delay update visual untuk memastikan AudioManager sudah ready
        // Invoke dengan delay 0.1f untuk memberikan waktu AudioManager ter-initialize
        Invoke(nameof(DelayedUpdateVisual), 0.1f);
    }
    
    void OnDestroy()
    {
        // Remove listener saat object dihancurkan
        if (button != null)
        {
            button.onClick.RemoveListener(ToggleMusic);
        }
    }
    
    /// <summary>
    /// Update visual dengan delay untuk memastikan AudioManager ready
    /// </summary>
    private void DelayedUpdateVisual()
    {
        UpdateVisual();
        
        // Debug log untuk verifikasi OnEnable dipanggil
        if (AudioManager.Instance != null)
        {
            bool musicStatus = AudioManager.Instance.IsBGMEnabled();
            Debug.Log($"[MusicToggleIconButton] OnEnable (Delayed) - Music Status: {(musicStatus ? "ON" : "OFF")}");
        }
    }

    /// <summary>
    /// Toggle music ON/OFF saat button diklik
    /// Fungsi ini dipanggil OTOMATIS oleh button listener
    /// </summary>
    private void ToggleMusic()
    {
        if (AudioManager.Instance == null) return;

        // Play button SFX
        AudioManager.Instance.PlayButtonSFX(false); // false = Level scene

        // Toggle BGM via AudioManager (otomatis save ke PlayerPrefs)
        bool isMusicOn = AudioManager.Instance.ToggleBGM();

        // Update visual button
        UpdateVisual();

        // Debug log
        Debug.Log("[MusicToggleIconButton] Music " + (isMusicOn ? "ON" : "OFF"));
    }

    /// <summary>
    /// Update visual button sesuai status music (gelap = OFF, normal = ON)
    /// </summary>
    private void UpdateVisual()
    {
        if (buttonImage == null || AudioManager.Instance == null) return;

        // Set warna sesuai status BGM dari AudioManager
        bool isMusicOn = AudioManager.Instance.IsBGMEnabled();
        buttonImage.color = isMusicOn ? enabledColor : disabledColor;
    }
}
