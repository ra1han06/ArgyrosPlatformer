using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// LEVEL BUTTON HANDLER - Script untuk Button Level di SelectLevel Scene
/// 
/// Fungsi:
/// - Menangani klik button level (Level 1, Level 2, dll)
/// - Load scene level yang dituju
/// - Set flag untuk trigger cutscene (jika dibutuhkan)
/// - Menampilkan best time dan best deaths dari PlayerPrefs
/// 
/// Cara Pakai:
/// 1. Attach script ini ke Button "1" (Level 1) di scene SelectLevel
/// 2. Isi field di Inspector:
///    - levelSceneName = "level1" 
///    - levelNumber = 1
///    - needsCutsceneOnFirstPlay = true (centang ini jika level ini punya cutscene intro)
///    - bestTimeText = drag TextMeshProUGUI untuk best time
///    - bestDeathsText = drag TextMeshProUGUI untuk best deaths
/// 3. Script akan otomatis:
///    - Load scene level yang dituju
///    - Set flag "ShouldPlayCutscene_Level1" jadi true (jika belum pernah dimainkan)
///    - Display best record dari PlayerPrefs
/// </summary>
[RequireComponent(typeof(Button))]
public class LevelButtonHandler : MonoBehaviour
{
    // =====================================================
    // SETTINGS
    // =====================================================
    [Header("=== Level Settings ===")]
    [Tooltip("Nama scene level yang akan di-load (contoh: level1, level2, level3)")]
    public string levelSceneName = "level1";
    
    [Tooltip("Nomor level (1, 2, 3, dll) - untuk tracking")]
    public int levelNumber = 1;
    
    [Tooltip("Apakah level ini perlu cutscene saat pertama kali dimainkan?")]
    public bool needsCutsceneOnFirstPlay = true;

    [Header("=== Best Record UI ===")]
    [Tooltip("TextMeshProUGUI untuk menampilkan best time")]
    public TextMeshProUGUI bestTimeText;
    
    [Tooltip("TextMeshProUGUI untuk menampilkan best deaths")]
    public TextMeshProUGUI bestDeathsText;

    [Header("=== Format Settings ===")]
    [Tooltip("Format untuk best time (contoh: 'Best: {0}')")]
    public string bestTimeFormat = "{0}";
    
    [Tooltip("Format untuk best deaths (contoh: 'Deaths: {0}')")]
    public string bestDeathsFormat = "{0}";
    
    [Tooltip("Text yang ditampilkan jika belum pernah menyelesaikan level")]
    public string notCompletedText = "Not Completed";

    // =====================================================
    // REFERENCES
    // =====================================================
    private Button button;
    
    // Key untuk PlayerPrefs - tracking apakah cutscene sudah pernah dimainkan
    private string cutscenePlayedKey;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    void Start()
    {
        // Buat key untuk PlayerPrefs berdasarkan level number
        cutscenePlayedKey = $"CutscenePlayed_Level{levelNumber}";
        
        // Ambil komponen Button
        button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError($"[LevelButtonHandler] Tidak ada komponen Button di {gameObject.name}!");
            return;
        }
        
        // Tambahkan listener untuk button click
        button.onClick.AddListener(OnLevelButtonClicked);
        
        Debug.Log($"[LevelButtonHandler] Button '{gameObject.name}' siap! → akan load scene '{levelSceneName}'");
        
        // Load dan display best record
        LoadBestRecord();
    }

    // =====================================================
    // LOAD BEST RECORD
    // =====================================================
    /// <summary>
    /// Load best time dan best deaths dari PlayerPrefs, lalu display di UI
    /// </summary>
    private void LoadBestRecord()
    {
        // Cek apakah GameManager ada
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[LevelButtonHandler] GameManager.Instance belum ada. Best record akan di-load saat GameManager tersedia.");
            DisplayNotCompleted();
            return;
        }

        // Cek apakah level pernah diselesaikan
        bool hasCompleted = GameManager.Instance.HasCompletedLevel(levelNumber);

        if (hasCompleted)
        {
            // Get best stats
            float bestTime = GameManager.Instance.GetBestTime(levelNumber);
            int bestDeaths = GameManager.Instance.GetBestDeaths(levelNumber);

            // Display best stats
            DisplayBestRecord(bestTime, bestDeaths);
        }
        else
        {
            // Display "Not Completed"
            DisplayNotCompleted();
        }
    }

    // =====================================================
    // DISPLAY BEST RECORD
    // =====================================================
    /// <summary>
    /// Display best time dan best deaths di UI
    /// </summary>
    private void DisplayBestRecord(float bestTime, int bestDeaths)
    {
        // Display best time
        if (bestTimeText != null)
        {
            string formattedTime = GameManager.FormatTime(bestTime);
            bestTimeText.text = string.Format(bestTimeFormat, formattedTime);
        }

        // Display best deaths
        if (bestDeathsText != null)
        {
            bestDeathsText.text = string.Format(bestDeathsFormat, bestDeaths);
        }
    }

    // =====================================================
    // DISPLAY NOT COMPLETED
    // =====================================================
    /// <summary>
    /// Display "Not Completed" jika level belum pernah diselesaikan
    /// </summary>
    private void DisplayNotCompleted()
    {
        if (bestTimeText != null)
        {
            bestTimeText.text = notCompletedText;
        }

        if (bestDeathsText != null)
        {
            bestDeathsText.text = "";
        }
    }

    // =====================================================
    // FUNGSI: ON LEVEL BUTTON CLICKED
    // =====================================================
    /// <summary>
    /// Dipanggil saat button level diklik.
    /// Set flag cutscene (jika diperlukan), lalu load scene level.
    /// </summary>
    private void OnLevelButtonClicked()
    {
        Debug.Log($"[LevelButtonHandler] Button Level {levelNumber} diklik!");
        
        // Set current level index di GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevelIndex = levelNumber;
            Debug.Log($"[LevelButtonHandler] GameManager.currentLevelIndex set to {levelNumber}");
        }
        
        // CEK: Apakah level ini perlu cutscene DAN belum pernah dimainkan?
        if (needsCutsceneOnFirstPlay)
        {
            // Cek apakah cutscene sudah pernah dimainkan sebelumnya
            int cutsceneAlreadyPlayed = PlayerPrefs.GetInt(cutscenePlayedKey, 0); // 0 = belum pernah, 1 = sudah pernah
            
            if (cutsceneAlreadyPlayed == 0)
            {
                // Belum pernah dimainkan → Set flag untuk trigger cutscene
                PlayerPrefs.SetInt("ShouldPlayCutscene", 1); // Flag global untuk trigger cutscene
                PlayerPrefs.SetInt("CutsceneLevel", levelNumber); // Simpan nomor level untuk cutscene
                PlayerPrefs.Save();
                
                Debug.Log($"[LevelButtonHandler] ✓ Flag cutscene di-set untuk Level {levelNumber}");
            }
            else
            {
                Debug.Log($"[LevelButtonHandler] Cutscene Level {levelNumber} sudah pernah dimainkan sebelumnya → skip");
            }
        }
        
        // Resume time (untuk jaga-jaga jika ada pause)
        Time.timeScale = 1f;
        
        // Load scene level
        Debug.Log($"[LevelButtonHandler] Loading scene: {levelSceneName}");
        SceneManager.LoadScene(levelSceneName);
    }

    // =====================================================
    // UNITY LIFECYCLE: ON DESTROY
    // =====================================================
    void OnDestroy()
    {
        // Hapus listener saat GameObject dihancurkan
        if (button != null)
        {
            button.onClick.RemoveListener(OnLevelButtonClicked);
        }
    }
}
