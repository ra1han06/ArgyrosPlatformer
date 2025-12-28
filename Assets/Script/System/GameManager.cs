using UnityEngine;
using System.Collections;

/// <summary>
/// GAME MANAGER - Singleton Persistent Manager
/// 
/// Fungsi:
/// - Melacak timer level (mulai dari 0 hingga finish)
/// - Melacak jumlah kematian player per level
/// - Menyimpan best time & best death count per level ke PlayerPrefs
/// - Auto-start timer setelah cutscene selesai
/// - Persist across scenes menggunakan DontDestroyOnLoad
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject "GameManager" di MainMenu scene
/// 2. Script akan otomatis persist dan available di semua scene
/// 3. Access via GameManager.Instance dari script lain
/// </summary>
public class GameManager : MonoBehaviour
{
    // =====================================================
    // SINGLETON PATTERN
    // =====================================================
    public static GameManager Instance { get; private set; }

    // =====================================================
    // PUBLIC FIELDS
    // =====================================================
    [Header("=== CURRENT LEVEL DATA ===")]
    [Tooltip("Index level yang sedang dimainkan (1 = level1, 2 = level2, dst)")]
    public int currentLevelIndex = 1;

    [Tooltip("Timer level dalam detik (berjalan dari 0)")]
    public float levelTimer = 0f;

    [Tooltip("Jumlah kematian di level saat ini")]
    public int deathCount = 0;

    [Header("=== SAVE DATA ===")]
    [Tooltip("Current save data - loaded from SaveSystem")]
    public GameSaveData currentSave;

    [Header("=== TIMER STATUS ===")]
    [Tooltip("Status timer sedang berjalan atau tidak")]
    [SerializeField] private bool isTimerRunning = false;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug log messages")]
    [SerializeField] private bool showDebugLog = true;

    // =====================================================
    // PRIVATE FIELDS
    // =====================================================
    private bool hasInitialized = false;

    // =====================================================
    // UNITY LIFECYCLE: AWAKE
    // =====================================================
    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load save data
        LoadProgress();

        if (showDebugLog)
            Debug.Log("[GameManager] Singleton created and persisted across scenes");
    }

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    void Start()
    {
        // Initialize timer detection
        StartCoroutine(InitializeTimer());
    }

    // =====================================================
    // UNITY LIFECYCLE: UPDATE
    // =====================================================
    void Update()
    {
        // Update timer jika sedang berjalan
        if (isTimerRunning)
        {
            levelTimer += Time.deltaTime;
        }
    }

    // =====================================================
    // INITIALIZATION - WAIT FOR CUTSCENE END
    // =====================================================
    /// <summary>
    /// Menunggu cutscene selesai, lalu auto-start timer.
    /// Jika tidak ada cutscene, timer akan start setelah delay 2 detik.
    /// </summary>
    private IEnumerator InitializeTimer()
    {
        if (hasInitialized)
        {
            if (showDebugLog)
                Debug.Log("[GameManager] Timer already initialized, skipping...");
            yield break;
        }

        if (showDebugLog)
            Debug.Log("[GameManager] Waiting for cutscene to finish...");

        // Wait untuk NovelCutsceneManager exist (dengan timeout 2 detik)
        float timeout = 2f;
        float elapsed = 0f;

        while (NovelCutsceneManager.Instance == null && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (NovelCutsceneManager.Instance != null)
        {
            if (showDebugLog)
                Debug.Log("[GameManager] NovelCutsceneManager found, waiting for cutscene to end...");

            // Wait sampai cutscene selesai
            while (NovelCutsceneManager.Instance.IsCutscenePlaying)
            {
                yield return null;
            }

            // Tambahan buffer kecil setelah cutscene selesai
            yield return new WaitForSeconds(0.1f);

            if (showDebugLog)
                Debug.Log("[GameManager] Cutscene finished!");
        }
        else
        {
            // Tidak ada cutscene manager, tunggu 0.5 detik untuk level setup
            if (showDebugLog)
                Debug.Log("[GameManager] No cutscene detected, starting timer after short delay...");

            yield return new WaitForSeconds(0.5f);
        }

        // Start timer otomatis
        hasInitialized = true;
        StartTimer();
    }

    // =====================================================
    // TIMER CONTROL METHODS
    // =====================================================

    /// <summary>
    /// Mulai menjalankan timer level
    /// </summary>
    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;

            if (showDebugLog)
                Debug.Log($"[GameManager] ▶️ Timer STARTED for Level {currentLevelIndex}");
        }
    }

    /// <summary>
    /// Stop timer (saat pause atau level selesai)
    /// </summary>
    public void StopTimer()
    {
        if (isTimerRunning)
        {
            isTimerRunning = false;

            if (showDebugLog)
                Debug.Log($"[GameManager] ⏸️ Timer STOPPED at {FormatTime(levelTimer)}");
        }
    }

    /// <summary>
    /// Reset timer dan death count (saat restart level)
    /// </summary>
    public void ResetLevel()
    {
        levelTimer = 0f;
        deathCount = 0;
        isTimerRunning = false;
        hasInitialized = false;

        if (showDebugLog)
            Debug.Log($"[GameManager] 🔄 Level {currentLevelIndex} RESET (Timer & Deaths cleared)");

        // Restart initialization coroutine untuk level baru
        StartCoroutine(InitializeTimer());
    }

    // =====================================================
    // DEATH COUNTER METHODS
    // =====================================================

    /// <summary>
    /// Increment death count (dipanggil dari RespawnManager)
    /// </summary>
    public void IncrementDeaths()
    {
        deathCount++;

        if (showDebugLog)
            Debug.Log($"[GameManager] 💀 Player died! Total deaths: {deathCount}");
    }

    // =====================================================
    // LEVEL COMPLETION
    // =====================================================

    /// <summary>
    /// Dipanggil saat player mencapai finish line.
    /// Stop timer dan save best record jika lebih baik dari sebelumnya.
    /// Unlock level berikutnya.
    /// </summary>
    public void CompleteLevel()
    {
        // Stop timer
        StopTimer();

        if (showDebugLog)
            Debug.Log($"[GameManager] 🏁 Level {currentLevelIndex} COMPLETED!");
        
        if (showDebugLog)
            Debug.Log($"[GameManager] Final Stats - Time: {FormatTime(levelTimer)} | Deaths: {deathCount}");

        // Save best record
        SaveBestRecord();
        
        // Unlock level berikutnya
        UnlockNextLevel();
    }

    // =====================================================
    // SAVE & LOAD BEST RECORDS
    // =====================================================

    /// <summary>
    /// Simpan best record ke PlayerPrefs jika lebih baik dari sebelumnya
    /// </summary>
    private void SaveBestRecord()
    {
        string timeKey = $"BestTime_Level{currentLevelIndex}";
        string deathsKey = $"BestDeaths_Level{currentLevelIndex}";

        // Cek apakah ada record sebelumnya
        bool hasRecord = PlayerPrefs.HasKey(timeKey);

        if (hasRecord)
        {
            float previousBestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);
            int previousBestDeaths = PlayerPrefs.GetInt(deathsKey, int.MaxValue);

            // Update best time jika lebih cepat
            if (levelTimer < previousBestTime)
            {
                PlayerPrefs.SetFloat(timeKey, levelTimer);
                if (showDebugLog)
                    Debug.Log($"[GameManager] 🏆 NEW BEST TIME! {FormatTime(levelTimer)} (previous: {FormatTime(previousBestTime)})");
            }

            // Update best deaths jika lebih sedikit
            if (deathCount < previousBestDeaths)
            {
                PlayerPrefs.SetInt(deathsKey, deathCount);
                if (showDebugLog)
                    Debug.Log($"[GameManager] 🏆 NEW BEST DEATHS! {deathCount} (previous: {previousBestDeaths})");
            }
        }
        else
        {
            // First time completion - save semua
            PlayerPrefs.SetFloat(timeKey, levelTimer);
            PlayerPrefs.SetInt(deathsKey, deathCount);

            if (showDebugLog)
                Debug.Log($"[GameManager] 🎉 FIRST COMPLETION! Time: {FormatTime(levelTimer)} | Deaths: {deathCount}");
        }

        PlayerPrefs.Save();
        
        // Save progress to save system
        SaveProgress();
    }
    
    /// <summary>
    /// Unlock level berikutnya di save data
    /// </summary>
    private void UnlockNextLevel()
    {
        int nextLevelIndex = currentLevelIndex; // Array index (0-based untuk level 2, dst)
        
        // Validasi: pastikan tidak unlock lebih dari jumlah level
        if (nextLevelIndex >= currentSave.unlockedLevels.Length)
        {
            if (showDebugLog)
                Debug.Log($"[GameManager] Level {currentLevelIndex} adalah level terakhir.");
            return;
        }
        
        // Set current level sebagai completed
        if (currentLevelIndex - 1 >= 0 && currentLevelIndex - 1 < currentSave.completedLevels.Length)
        {
            currentSave.completedLevels[currentLevelIndex - 1] = true;
        }
        
        // Unlock level berikutnya (jika belum)
        if (!currentSave.unlockedLevels[nextLevelIndex])
        {
            currentSave.unlockedLevels[nextLevelIndex] = true;
            
            if (showDebugLog)
                Debug.Log($"[GameManager] 🔓 Level {nextLevelIndex + 1} UNLOCKED!");
        }
        
        // Save progress
        SaveProgress();
    }

    /// <summary>
    /// Get best time untuk level tertentu
    /// </summary>
    public float GetBestTime(int levelIndex)
    {
        string key = $"BestTime_Level{levelIndex}";
        return PlayerPrefs.GetFloat(key, -1f); // -1 = no record
    }

    /// <summary>
    /// Get best deaths untuk level tertentu
    /// </summary>
    public int GetBestDeaths(int levelIndex)
    {
        string key = $"BestDeaths_Level{levelIndex}";
        return PlayerPrefs.GetInt(key, -1); // -1 = no record
    }

    /// <summary>
    /// Cek apakah level pernah diselesaikan
    /// </summary>
    public bool HasCompletedLevel(int levelIndex)
    {
        string key = $"BestTime_Level{levelIndex}";
        return PlayerPrefs.HasKey(key);
    }

    // =====================================================
    // UTILITY METHODS
    // =====================================================

    /// <summary>
    /// Format waktu ke MM:SS:MS
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    /// <summary>
    /// Get current timer sebagai formatted string
    /// </summary>
    public string GetFormattedTimer()
    {
        return FormatTime(levelTimer);
    }

    /// <summary>
    /// Get timer status untuk debugging
    /// </summary>
    public bool IsTimerRunning()
    {
        return isTimerRunning;
    }

    // =====================================================
    // SAVE SYSTEM INTEGRATION
    // =====================================================

    /// <summary>
    /// Load progress dari SaveSystem
    /// Dipanggil di Awake() saat GameManager pertama kali dibuat
    /// </summary>
    private void LoadProgress()
    {
        // Cek apakah ada save file
        if (SaveSystem.HasSaveFile())
        {
            // Load save data
            currentSave = SaveSystem.LoadGame();

            if (currentSave != null)
            {
                if (showDebugLog)
                {
                    Debug.Log("[GameManager] Save data loaded successfully!");
                    Debug.Log($"[GameManager] Last Played Level: {currentSave.lastPlayedLevel}");
                    Debug.Log($"[GameManager] Total Play Time: {currentSave.totalPlayTime:F1}s");
                }
            }
            else
            {
                // Failed to load - create new save
                Debug.LogWarning("[GameManager] Failed to load save data. Creating new save.");
                currentSave = new GameSaveData();
            }
        }
        else
        {
            // No save file - create new save data
            if (showDebugLog)
                Debug.Log("[GameManager] No save file found. Creating new save data.");
            
            currentSave = new GameSaveData();
        }
    }

    /// <summary>
    /// Save progress ke SaveSystem
    /// Dipanggil saat level selesai (CompleteLevel)
    /// </summary>
    public void SaveProgress()
    {
        if (currentSave == null)
        {
            Debug.LogError("[GameManager] Cannot save progress - currentSave is null!");
            return;
        }

        // Update last played level
        currentSave.lastPlayedLevel = currentLevelIndex;

        // Update total play time (tambahkan waktu level saat ini)
        currentSave.totalPlayTime += levelTimer;

        // Mark level as completed (index = currentLevelIndex - 1)
        int levelArrayIndex = currentLevelIndex - 1;
        if (levelArrayIndex >= 0 && levelArrayIndex < currentSave.completedLevels.Length)
        {
            currentSave.completedLevels[levelArrayIndex] = true;
        }

        // Save to disk
        SaveSystem.SaveGame(currentSave);

        if (showDebugLog)
        {
            Debug.Log($"[GameManager] Progress saved! Last Level: {currentSave.lastPlayedLevel}");
            Debug.Log($"[GameManager] Total Play Time: {currentSave.totalPlayTime:F1}s");
        }
    }
}
