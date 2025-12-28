using UnityEngine;
using TMPro;

/// <summary>
/// COMPLETE SCENE HANDLER - Display Level Completion Stats
/// 
/// Fungsi:
/// - Menampilkan waktu penyelesaian level (current)
/// - Menampilkan jumlah kematian level (current)
/// - Menampilkan best time (dari PlayerPrefs)
/// - Menampilkan best deaths (dari PlayerPrefs)
/// - Membandingkan apakah current stats adalah new record
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject di Complete scene
/// 2. Assign 4 TextMeshProUGUI di Inspector:
///    - currentTimeText
///    - currentDeathsText
///    - bestTimeText
///    - bestDeathsText
/// 3. Script akan otomatis load dan display stats saat scene dimulai
/// </summary>
public class CompleteSceneHandler : MonoBehaviour
{
    // =====================================================
    // SERIALIZED FIELDS
    // =====================================================
    [Header("=== CURRENT STATS UI ===")]
    [Tooltip("TextMeshProUGUI untuk menampilkan waktu penyelesaian current")]
    [SerializeField] private TextMeshProUGUI currentTimeText;

    [Tooltip("TextMeshProUGUI untuk menampilkan deaths current")]
    [SerializeField] private TextMeshProUGUI currentDeathsText;

    [Header("=== BEST RECORD UI ===")]
    [Tooltip("TextMeshProUGUI untuk menampilkan best time")]
    [SerializeField] private TextMeshProUGUI bestTimeText;

    [Tooltip("TextMeshProUGUI untuk menampilkan best deaths")]
    [SerializeField] private TextMeshProUGUI bestDeathsText;

    [Header("=== FORMAT SETTINGS ===")]
    [Tooltip("Format untuk current time (contoh: 'Time: {0}')")]
    [SerializeField] private string currentTimeFormat = "Time: {0}";

    [Tooltip("Format untuk current deaths (contoh: 'Deaths: {0}')")]
    [SerializeField] private string currentDeathsFormat = "Deaths: {0}";

    [Tooltip("Format untuk best time (contoh: 'Best Time: {0}')")]
    [SerializeField] private string bestTimeFormat = "Best Time: {0}";

    [Tooltip("Format untuk best deaths (contoh: 'Best Deaths: {0}')")]
    [SerializeField] private string bestDeathsFormat = "Best Deaths: {0}";

    [Header("=== COLOR SETTINGS ===")]
    [Tooltip("Warna untuk current stats")]
    [SerializeField] private Color currentStatsColor = Color.white;

    [Tooltip("Warna untuk best stats")]
    [SerializeField] private Color bestStatsColor = Color.yellow;

    [Tooltip("Warna untuk NEW RECORD")]
    [SerializeField] private Color newRecordColor = Color.green;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug log messages")]
    [SerializeField] private bool showDebugLog = true;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    void Start()
    {
        // Validasi references
        ValidateReferences();

        // Display stats dengan retry untuk wait GameManager instance
        StartCoroutine(DisplayStatsWhenReady());
    }

    // =====================================================
    // DISPLAY STATS WITH RETRY
    // =====================================================
    /// <summary>
    /// Wait untuk GameManager instance ready, lalu display stats
    /// </summary>
    private System.Collections.IEnumerator DisplayStatsWhenReady()
    {
        // Wait hingga GameManager instance ada (max 3 detik)
        float timeout = 3f;
        float elapsed = 0f;

        while (GameManager.Instance == null && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Check apakah GameManager sudah ditemukan
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CompleteSceneHandler] GameManager.Instance tidak ditemukan setelah 3 detik! Pastikan GameManager dibuat di MainMenu scene dengan DontDestroyOnLoad.");
            
            // Display default/placeholder values
            DisplayDefaultStats();
            yield break;
        }

        // GameManager ditemukan - display stats normal
        DisplayStats();
    }

    // =====================================================
    // VALIDATION
    // =====================================================
    private void ValidateReferences()
    {
        if (currentTimeText == null)
            Debug.LogError("[CompleteSceneHandler] currentTimeText belum di-assign!");

        if (currentDeathsText == null)
            Debug.LogError("[CompleteSceneHandler] currentDeathsText belum di-assign!");

        if (bestTimeText == null)
            Debug.LogError("[CompleteSceneHandler] bestTimeText belum di-assign!");

        if (bestDeathsText == null)
            Debug.LogError("[CompleteSceneHandler] bestDeathsText belum di-assign!");
    }

    // =====================================================
    // DISPLAY STATS
    // =====================================================
    private void DisplayStats()
    {
        // Cek apakah GameManager ada
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CompleteSceneHandler] GameManager.Instance tidak ditemukan! Pastikan GameManager dibuat di MainMenu scene.");
            return;
        }

        // Get current level index
        int levelIndex = GameManager.Instance.currentLevelIndex;

        // Get current stats dari GameManager
        float currentTime = GameManager.Instance.levelTimer;
        int currentDeaths = GameManager.Instance.deathCount;

        // Get best stats dari PlayerPrefs
        float bestTime = GameManager.Instance.GetBestTime(levelIndex);
        int bestDeaths = GameManager.Instance.GetBestDeaths(levelIndex);

        if (showDebugLog)
        {
            Debug.Log($"[CompleteSceneHandler] Level {levelIndex} Completed!");
            Debug.Log($"[CompleteSceneHandler] Current - Time: {GameManager.FormatTime(currentTime)} | Deaths: {currentDeaths}");
            Debug.Log($"[CompleteSceneHandler] Best - Time: {(bestTime >= 0 ? GameManager.FormatTime(bestTime) : "---")} | Deaths: {(bestDeaths >= 0 ? bestDeaths.ToString() : "---")}");
        }

        // Display current stats
        DisplayCurrentStats(currentTime, currentDeaths, bestTime, bestDeaths);

        // Display best stats
        DisplayBestStats(bestTime, bestDeaths);
    }

    // =====================================================
    // DISPLAY CURRENT STATS
    // =====================================================
    private void DisplayCurrentStats(float currentTime, int currentDeaths, float bestTime, int bestDeaths)
    {
        // Current Time
        if (currentTimeText != null)
        {
            string formattedTime = GameManager.FormatTime(currentTime);
            currentTimeText.text = string.Format(currentTimeFormat, formattedTime);

            // Cek apakah NEW RECORD untuk time
            bool isNewTimeRecord = (bestTime < 0 || currentTime < bestTime);
            currentTimeText.color = isNewTimeRecord ? newRecordColor : currentStatsColor;

            if (isNewTimeRecord && showDebugLog)
                Debug.Log("[CompleteSceneHandler] 🏆 NEW BEST TIME RECORD!");
        }

        // Current Deaths
        if (currentDeathsText != null)
        {
            currentDeathsText.text = string.Format(currentDeathsFormat, currentDeaths);

            // Cek apakah NEW RECORD untuk deaths
            bool isNewDeathsRecord = (bestDeaths < 0 || currentDeaths < bestDeaths);
            currentDeathsText.color = isNewDeathsRecord ? newRecordColor : currentStatsColor;

            if (isNewDeathsRecord && showDebugLog)
                Debug.Log("[CompleteSceneHandler] 🏆 NEW BEST DEATHS RECORD!");
        }
    }

    // =====================================================
    // DISPLAY BEST STATS
    // =====================================================
    private void DisplayBestStats(float bestTime, int bestDeaths)
    {
        // Best Time
        if (bestTimeText != null)
        {
            if (bestTime >= 0)
            {
                string formattedBestTime = GameManager.FormatTime(bestTime);
                bestTimeText.text = string.Format(bestTimeFormat, formattedBestTime);
            }
            else
            {
                bestTimeText.text = string.Format(bestTimeFormat, "---");
            }

            bestTimeText.color = bestStatsColor;
        }

        // Best Deaths
        if (bestDeathsText != null)
        {
            if (bestDeaths >= 0)
            {
                bestDeathsText.text = string.Format(bestDeathsFormat, bestDeaths);
            }
            else
            {
                bestDeathsText.text = string.Format(bestDeathsFormat, "---");
            }

            bestDeathsText.color = bestStatsColor;
        }
    }

    // =====================================================
    // DISPLAY DEFAULT STATS (FALLBACK)
    // =====================================================
    /// <summary>
    /// Display default placeholder values ketika GameManager tidak ditemukan
    /// </summary>
    private void DisplayDefaultStats()
    {
        if (showDebugLog)
            Debug.LogWarning("[CompleteSceneHandler] Displaying default stats karena GameManager tidak ada.");

        // Current stats - tampilkan placeholder
        if (currentTimeText != null)
        {
            currentTimeText.text = string.Format(currentTimeFormat, "00:00:00");
            currentTimeText.color = currentStatsColor;
        }

        if (currentDeathsText != null)
        {
            currentDeathsText.text = string.Format(currentDeathsFormat, 0);
            currentDeathsText.color = currentStatsColor;
        }

        // Best stats - tampilkan "---"
        if (bestTimeText != null)
        {
            bestTimeText.text = string.Format(bestTimeFormat, "---");
            bestTimeText.color = bestStatsColor;
        }

        if (bestDeathsText != null)
        {
            bestDeathsText.text = string.Format(bestDeathsFormat, "---");
            bestDeathsText.color = bestStatsColor;
        }
    }
}
