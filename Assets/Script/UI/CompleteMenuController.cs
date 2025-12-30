using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller untuk Complete Scene - handle Main Menu, Retry, dan Next Level buttons
/// </summary>
public class CompleteMenuController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextLevelButton;

    [Header("Settings")]
    [SerializeField] private bool showDebugLog = true;

    private const int TOTAL_LEVELS = 10; // Sesuaikan dengan jumlah level yang ada

    private void Start()
    {
        // Subscribe button click events
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        else
            LogWarning("Main Menu Button reference belum di-assign!");

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        else
            LogWarning("Retry Button reference belum di-assign!");

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            UpdateNextButtonState();
        }
        else
            LogWarning("Next Level Button reference belum di-assign!");
    }

    /// <summary>
    /// Update state Next Level button - disable jika ini level terakhir
    /// </summary>
    private void UpdateNextButtonState()
    {
        if (nextLevelButton == null || GameManager.Instance == null) return;

        int currentLevel = GameManager.Instance.currentLevelIndex;
        bool isLastLevel = currentLevel >= TOTAL_LEVELS;

        // Disable button jika sudah level terakhir
        nextLevelButton.interactable = !isLastLevel;

        if (isLastLevel)
        {
            LogDebug($"Level {currentLevel} adalah level terakhir - Next button disabled");
        }
    }

    /// <summary>
    /// Main Menu Button - kembali ke Main Menu
    /// </summary>
    private void OnMainMenuClicked()
    {
        LogDebug("Main Menu button clicked");

        // CRITICAL: Reset Time.timeScale sebelum load scene
        Time.timeScale = 1f;

        // Load Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Retry Button - ulang level yang sama
    /// </summary>
    private void OnRetryClicked()
    {
        if (GameManager.Instance == null)
        {
            LogWarning("GameManager.Instance is null! Cannot retry level.");
            return;
        }

        int currentLevel = GameManager.Instance.currentLevelIndex;
        LogDebug($"Retry button clicked - Restarting Level {currentLevel}");

        // Reset timer dan death count untuk sesi baru
        GameManager.Instance.ResetLevel();

        // CRITICAL: Reset Time.timeScale sebelum load scene
        Time.timeScale = 1f;

        // Load ulang level yang sama
        string levelSceneName = $"level{currentLevel}";
        SceneManager.LoadScene(levelSceneName);
    }

    /// <summary>
    /// Next Level Button - masuk ke level berikutnya
    /// </summary>
    private void OnNextLevelClicked()
    {
        if (GameManager.Instance == null)
        {
            LogWarning("GameManager.Instance is null! Cannot proceed to next level.");
            return;
        }

        int currentLevel = GameManager.Instance.currentLevelIndex;
        int nextLevel = currentLevel + 1;

        // Check apakah masih ada level berikutnya
        if (nextLevel > TOTAL_LEVELS)
        {
            LogDebug($"Sudah di level terakhir ({currentLevel}) - Kembali ke Main Menu");
            OnMainMenuClicked();
            return;
        }

        LogDebug($"Next Level button clicked - Loading Level {nextLevel}");

        // Set current level index ke level berikutnya
        GameManager.Instance.currentLevelIndex = nextLevel;

        // Reset timer dan death count untuk level baru
        GameManager.Instance.ResetLevel();

        // CRITICAL: Reset Time.timeScale sebelum load scene
        Time.timeScale = 1f;

        // Load level berikutnya
        string nextLevelSceneName = $"level{nextLevel}";
        
        // Check apakah scene exists di Build Settings
        if (Application.CanStreamedLevelBeLoaded(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            LogWarning($"Scene '{nextLevelSceneName}' tidak ditemukan di Build Settings! Kembali ke Main Menu.");
            OnMainMenuClicked();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe events
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        if (retryButton != null)
            retryButton.onClick.RemoveListener(OnRetryClicked);
        if (nextLevelButton != null)
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
    }

    #region Debug Helpers
    private void LogDebug(string message)
    {
        if (showDebugLog)
            Debug.Log($"[CompleteMenuController] {message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[CompleteMenuController] {message}");
    }
    #endregion
}
