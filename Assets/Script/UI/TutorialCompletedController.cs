using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// TUTORIAL COMPLETED CONTROLLER - Handle button di TutorialCompleted Scene
/// 
/// Fungsi:
/// - Main Menu Button: Kembali ke MainMenu
/// - Retry Button: Mengulang tutorial (load scene tutorial)
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject di TutorialCompleted scene
/// 2. Assign MainMenuButton dan RetryButton di Inspector
/// </summary>
public class TutorialCompletedController : MonoBehaviour
{
    // =====================================================
    // SERIALIZED FIELDS
    // =====================================================
    
    [Header("=== BUTTON REFERENCES ===")]
    [Tooltip("Button untuk kembali ke Main Menu")]
    [SerializeField] private Button mainMenuButton;

    [Tooltip("Button untuk mengulang tutorial")]
    [SerializeField] private Button retryButton;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug log messages")]
    [SerializeField] private bool showDebugLog = true;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    
    void Start()
    {
        // Subscribe button click events
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            LogDebug("MainMenuButton connected");
        }
        else
        {
            LogWarning("Main Menu Button reference belum di-assign!");
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
            LogDebug("RetryButton connected");
        }
        else
        {
            LogWarning("Retry Button reference belum di-assign!");
        }
    }

    // =====================================================
    // BUTTON CLICK HANDLERS
    // =====================================================
    
    /// <summary>
    /// Main Menu Button - kembali ke Main Menu
    /// </summary>
    private void OnMainMenuClicked()
    {
        LogDebug("Main Menu button clicked → Loading MainMenu");

        // CRITICAL: Reset Time.timeScale sebelum load scene
        Time.timeScale = 1f;

        // Load Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Retry Button - ulang tutorial
    /// </summary>
    private void OnRetryClicked()
    {
        LogDebug("Retry button clicked → Restarting Tutorial");

        // CRITICAL: Reset Time.timeScale sebelum load scene
        Time.timeScale = 1f;

        // Load ulang tutorial scene
        SceneManager.LoadScene("tutorial");
    }

    // =====================================================
    // CLEANUP
    // =====================================================
    
    private void OnDestroy()
    {
        // Unsubscribe events
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        if (retryButton != null)
            retryButton.onClick.RemoveListener(OnRetryClicked);
    }

    // =====================================================
    // DEBUG HELPERS
    // =====================================================
    
    private void LogDebug(string message)
    {
        if (showDebugLog)
            Debug.Log($"[TutorialCompletedController] {message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[TutorialCompletedController] {message}");
    }
}
