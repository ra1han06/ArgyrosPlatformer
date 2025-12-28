using UnityEngine;

/// <summary>
/// SCENE AUDIO INITIALIZER - Script untuk Auto-Play Music di Scene
/// 
/// Fungsi:
/// - Otomatis memutar musik yang tepat saat scene dimuat
/// - UI Scene → greekMusic
/// - Level Scene → levelMusic
/// 
/// Cara Pakai:
/// 1. Buat Empty GameObject di setiap scene, beri nama "SceneAudio"
/// 2. Attach script ini
/// 3. Pilih tipe scene di Inspector (UI atau Level)
/// 4. Selesai! Musik akan otomatis diputar saat scene dimuat
/// </summary>
public class SceneAudioInitializer : MonoBehaviour
{
    // =====================================================
    // SETTINGS
    // =====================================================
    [Header("=== Scene Type ===")]
    [Tooltip("Pilih tipe scene ini: UI (untuk menu/UI) atau Level (untuk gameplay)")]
    public AudioManager.SceneType sceneType = AudioManager.SceneType.UI;

    // =====================================================
    // PLAYERPREFS KEY (sama dengan MusicToggleButton)
    // =====================================================
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    /// <summary>
    /// Dipanggil saat scene pertama kali dimuat.
    /// Memutar musik yang sesuai dengan tipe scene HANYA jika musik enabled.
    /// </summary>
    void Start()
    {
        // Cek apakah AudioManager ada
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[SceneAudioInitializer] AudioManager tidak ditemukan! Pastikan AudioManager ada di scene pertama.");
            return;
        }

        // PENTING: SELALU panggil PlayUIMusic/PlayLevelMusic untuk update currentSceneType
        // Method tersebut sudah handle case musik OFF secara internal
        if (sceneType == AudioManager.SceneType.UI)
        {
            AudioManager.Instance.PlayUIMusic();
            Debug.Log("[SceneAudioInitializer] Scene UI dimuat");
        }
        else if (sceneType == AudioManager.SceneType.Level)
        {
            AudioManager.Instance.PlayLevelMusic();
            Debug.Log("[SceneAudioInitializer] Scene Level dimuat");
        }
    }
}
