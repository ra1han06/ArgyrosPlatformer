using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// LevelResetManager - Handles full level reset on player death
/// 
/// When player dies:
/// 1. Increment death count
/// 2. Save current timer value
/// 3. Reload scene (resets ALL platforms, abilities, runtime objects)
/// 4. Restore timer and death count after reload
/// 
/// This ensures a completely fresh level restart while preserving death count and timer.
/// </summary>
public class LevelResetManager : MonoBehaviour
{
    public static LevelResetManager Instance { get; private set; }

    [Header("Reset Settings")]
    [SerializeField] private float deathDelayBeforeReset = 1f;
    [Tooltip("Show debug messages in console")]
    [SerializeField] private bool showDebugLog = true;

    // Temporary storage for data that should survive scene reload
    private static float savedTimerValue = 0f;
    private static int savedDeathCount = 0;
    private static bool isResettingFromDeath = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Cleanup event listeners
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Called when player dies - handles full level reset
    /// </summary>
    public void HandlePlayerDeath()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[LevelResetManager] GameManager not found! Cannot handle death.");
            return;
        }

        if (showDebugLog)
            Debug.Log($"[LevelResetManager] Player death triggered. Death count before increment: {GameManager.Instance.deathCount}");

        // 1. Increment death count BEFORE saving
        GameManager.Instance.IncrementDeaths();

        // 2. Save timer and death count for restoration after reload
        savedTimerValue = GameManager.Instance.levelTimer;
        savedDeathCount = GameManager.Instance.deathCount;
        isResettingFromDeath = true;

        if (showDebugLog)
        {
            Debug.Log($"[LevelResetManager] Saved state for reload:");
            Debug.Log($"  - Timer: {savedTimerValue:F2}s");
            Debug.Log($"  - Death Count: {savedDeathCount}");
        }

        // 3. Schedule scene reload after delay
        Invoke(nameof(ReloadCurrentLevel), deathDelayBeforeReset);
    }

    /// <summary>
    /// Reload the current scene to reset everything
    /// </summary>
    private void ReloadCurrentLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (showDebugLog)
            Debug.Log($"[LevelResetManager] Reloading scene '{currentScene}' to reset level...");

        // Reset Time.timeScale to ensure game isn't paused
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(currentScene);
    }

    /// <summary>
    /// Called after scene is loaded - restore saved data if resetting from death
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only restore data if we're resetting from death
        if (!isResettingFromDeath)
            return;

        if (showDebugLog)
            Debug.Log($"[LevelResetManager] Scene '{scene.name}' loaded after death. Restoring saved data...");

        // Wait one frame to ensure GameManager and all systems are initialized
        StartCoroutine(RestoreSavedDataAfterFrame());
    }

    /// <summary>
    /// Restore timer and death count after scene reload
    /// </summary>
    private System.Collections.IEnumerator RestoreSavedDataAfterFrame()
    {
        // Wait for all scene objects to initialize
        yield return null;

        if (GameManager.Instance == null)
        {
            Debug.LogError("[LevelResetManager] GameManager not found after scene reload!");
            isResettingFromDeath = false;
            yield break;
        }

        // Stop any initialization coroutines that might be running
        GameManager.Instance.StopAllCoroutines();

        // Restore timer value (continue from where it was)
        GameManager.Instance.levelTimer = savedTimerValue;

        // Restore death count (already incremented before reload)
        GameManager.Instance.deathCount = savedDeathCount;

        // Manually start the timer (it should continue running)
        GameManager.Instance.StartTimer();

        if (showDebugLog)
        {
            Debug.Log($"[LevelResetManager] ✅ Data restored after level reset:");
            Debug.Log($"  - Timer: {GameManager.Instance.levelTimer:F2}s (CONTINUING)");
            Debug.Log($"  - Death Count: {GameManager.Instance.deathCount}");
            Debug.Log($"  - All platforms, abilities, and objects RESET to initial state");
        }

        // Clear the reset flag AFTER restoration is complete
        isResettingFromDeath = false;

        // Reset PlayerPlatformInteractor abilities
        ResetPlayerAbilities();
    }

    /// <summary>
    /// Reset player's copy/cut/paste abilities to default state
    /// </summary>
    private void ResetPlayerAbilities()
    {
        PlayerPlatformInteractor interactor = FindFirstObjectByType<PlayerPlatformInteractor>();
        
        if (interactor != null)
        {
            interactor.ResetLimits();
            
            if (showDebugLog)
                Debug.Log("[LevelResetManager] ✅ Player abilities reset (copy/cut/paste limits cleared)");
        }
        else
        {
            Debug.LogWarning("[LevelResetManager] PlayerPlatformInteractor not found - abilities not reset!");
        }
    }

    /// <summary>
    /// Get whether a level reset is in progress
    /// </summary>
    public bool IsResettingFromDeath()
    {
        return isResettingFromDeath;
    }
}
