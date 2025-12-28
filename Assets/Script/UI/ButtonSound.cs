using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BUTTON SOUND - Script untuk Tombol dengan SFX Otomatis
/// 
/// Fungsi:
/// - Otomatis memutar SFX saat tombol diklik
/// - Memilih SFX yang tepat berdasarkan tipe scene (UI atau Level)
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject yang punya komponen Button
/// 2. Pilih tipe scene di Inspector (UI atau Level)
/// 3. Selesai! SFX akan otomatis berbunyi saat tombol diklik
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    // =====================================================
    // SETTINGS
    // =====================================================
    [Header("=== Scene Type ===")]
    [Tooltip("Pilih tipe scene: UI (untuk menu/UI) atau Level (untuk gameplay)")]
    public AudioManager.SceneType sceneType = AudioManager.SceneType.UI;

    // =====================================================
    // REFERENCES
    // =====================================================
    private Button button;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    /// <summary>
    /// Dipanggil saat script pertama kali dijalankan.
    /// Setup listener untuk tombol.
    /// </summary>
    void Start()
    {
        // Ambil komponen Button
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("[ButtonSound] Tidak ada komponen Button di GameObject ini!");
            return;
        }

        // Tambahkan listener: saat tombol diklik, panggil PlaySound()
        button.onClick.AddListener(PlaySound);
    }

    // =====================================================
    // FUNGSI: PLAY SOUND
    // =====================================================
    /// <summary>
    /// Memutar SFX saat tombol diklik.
    /// Otomatis memilih SFX yang sesuai dengan tipe scene.
    /// </summary>
    private void PlaySound()
    {
        // Cek apakah AudioManager ada
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[ButtonSound] AudioManager tidak ditemukan! Pastikan AudioManager ada di scene.");
            return;
        }

        // Play SFX sesuai tipe scene
        bool isUI = (sceneType == AudioManager.SceneType.UI);
        AudioManager.Instance.PlayButtonSFX(isUI);
    }

    // =====================================================
    // UNITY LIFECYCLE: ON DESTROY
    // =====================================================
    /// <summary>
    /// Dipanggil saat GameObject dihancurkan.
    /// Hapus listener untuk menghindari memory leak.
    /// </summary>
    void OnDestroy()
    {
        // Hapus listener jika button masih ada
        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
        }
    }
}
