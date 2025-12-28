using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AUDIO SETTINGS - Script untuk Settings Menu dengan Volume Control
/// 
/// Fungsi:
/// - Mengatur volume BGM dan SFX menggunakan Slider
/// - Menyimpan pengaturan volume (menggunakan PlayerPrefs)
/// - Memuat pengaturan volume saat Settings Menu dibuka
/// 
/// Cara Pakai:
/// 1. Buat UI Slider untuk BGM Volume
/// 2. Buat UI Slider untuk SFX Volume
/// 3. Attach script ini ke GameObject Settings Panel
/// 4. Drag kedua Slider ke Inspector
/// 5. Selesai! Volume akan otomatis tersimpan dan dimuat
/// </summary>
public class AudioSettings : MonoBehaviour
{
    // =====================================================
    // REFERENCES (Isi di Inspector)
    // =====================================================
    [Header("=== Volume Sliders ===")]
    [Tooltip("Slider untuk mengatur volume BGM (Background Music)")]
    public Slider bgmVolumeSlider;

    [Tooltip("Slider untuk mengatur volume SFX (Sound Effects)")]
    public Slider sfxVolumeSlider;

    [Header("=== Optional: Volume Text Display ===")]
    [Tooltip("(Optional) Text untuk menampilkan % volume BGM")]
    public Text bgmVolumeText;

    [Tooltip("(Optional) Text untuk menampilkan % volume SFX")]
    public Text sfxVolumeText;

    // =====================================================
    // PLAYERPREFS KEYS
    // =====================================================
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    // =====================================================
    // DEFAULT VOLUMES
    // =====================================================
    private const float DEFAULT_BGM_VOLUME = 0.5f;  // 50%
    private const float DEFAULT_SFX_VOLUME = 0.7f;  // 70%

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    /// <summary>
    /// Dipanggil saat Settings Menu pertama kali dibuka.
    /// Memuat pengaturan volume yang tersimpan dan setup slider.
    /// </summary>
    void Start()
    {
        // Cek apakah AudioManager ada
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[AudioSettings] AudioManager tidak ditemukan!");
            return;
        }

        // Setup BGM Slider
        if (bgmVolumeSlider != null)
        {
            // Load saved volume atau gunakan default
            float savedBGMVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_BGM_VOLUME);

            // Set slider value
            bgmVolumeSlider.value = savedBGMVolume;

            // Apply volume ke AudioManager
            AudioManager.Instance.SetBGMVolume(savedBGMVolume);

            // Update text display (jika ada)
            UpdateBGMVolumeText(savedBGMVolume);

            // Add listener untuk perubahan slider
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

            Debug.Log($"[AudioSettings] BGM Volume loaded: {savedBGMVolume * 100}%");
        }

        // Setup SFX Slider
        if (sfxVolumeSlider != null)
        {
            // Load saved volume atau gunakan default
            float savedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);

            // Set slider value
            sfxVolumeSlider.value = savedSFXVolume;

            // Apply volume ke AudioManager
            AudioManager.Instance.SetSFXVolume(savedSFXVolume);

            // Update text display (jika ada)
            UpdateSFXVolumeText(savedSFXVolume);

            // Add listener untuk perubahan slider
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

            Debug.Log($"[AudioSettings] SFX Volume loaded: {savedSFXVolume * 100}%");
        }
    }

    // =====================================================
    // FUNGSI: ON BGM VOLUME CHANGED
    // =====================================================
    /// <summary>
    /// Dipanggil setiap kali slider BGM digerakkan.
    /// Mengatur volume BGM dan menyimpan pengaturan.
    /// </summary>
    private void OnBGMVolumeChanged(float value)
    {
        // Cek AudioManager
        if (AudioManager.Instance == null) return;

        // Set volume BGM
        AudioManager.Instance.SetBGMVolume(value);

        // Update text display
        UpdateBGMVolumeText(value);

        // Save ke PlayerPrefs
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();

        Debug.Log($"[AudioSettings] BGM Volume changed to: {value * 100}%");
    }

    // =====================================================
    // FUNGSI: ON SFX VOLUME CHANGED
    // =====================================================
    /// <summary>
    /// Dipanggil setiap kali slider SFX digerakkan.
    /// Mengatur volume SFX, memutar preview SFX, dan menyimpan pengaturan.
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        // Cek AudioManager
        if (AudioManager.Instance == null) return;

        // Set volume SFX
        AudioManager.Instance.SetSFXVolume(value);

        // Update text display
        UpdateSFXVolumeText(value);

        // Save ke PlayerPrefs
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();

        // (Optional) Play preview SFX untuk mendengar perubahan volume
        // AudioManager.Instance.PlayButtonSFX(true);

        Debug.Log($"[AudioSettings] SFX Volume changed to: {value * 100}%");
    }

    // =====================================================
    // FUNGSI: UPDATE VOLUME TEXT
    // =====================================================
    /// <summary>
    /// Update text display untuk BGM volume (jika ada).
    /// </summary>
    private void UpdateBGMVolumeText(float value)
    {
        if (bgmVolumeText != null)
        {
            // Tampilkan dalam bentuk persentase (0-100%)
            bgmVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    /// <summary>
    /// Update text display untuk SFX volume (jika ada).
    /// </summary>
    private void UpdateSFXVolumeText(float value)
    {
        if (sfxVolumeText != null)
        {
            // Tampilkan dalam bentuk persentase (0-100%)
            sfxVolumeText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    // =====================================================
    // FUNGSI: RESET TO DEFAULT
    // =====================================================
    /// <summary>
    /// Reset volume ke pengaturan default.
    /// Bisa dipanggil dari button "Reset to Default".
    /// </summary>
    public void ResetToDefault()
    {
        // Reset BGM Volume
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.value = DEFAULT_BGM_VOLUME;
        }

        // Reset SFX Volume
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = DEFAULT_SFX_VOLUME;
        }

        Debug.Log("[AudioSettings] Volume reset to default");
    }

    // =====================================================
    // UNITY LIFECYCLE: ON DESTROY
    // =====================================================
    /// <summary>
    /// Dipanggil saat Settings Menu ditutup.
    /// Hapus listener untuk menghindari memory leak.
    /// </summary>
    void OnDestroy()
    {
        // Hapus listener
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }
}
