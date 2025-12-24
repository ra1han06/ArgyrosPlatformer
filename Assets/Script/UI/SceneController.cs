using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Universal Scene Controller - Works on ALL UI scenes
/// Auto-detects and connects buttons based on their GameObject names
/// Just attach this script to a GameObject in ANY scene - it handles the rest!
/// </summary>
public class SceneController : MonoBehaviour
{
    // Scene paths - Use scene names for SceneManager.LoadScene
    public static readonly string MAIN_MENU = "MainMenu";
    public static readonly string SELECT_LEVEL = "SelectLevel";
    public static readonly string SETTINGS = "Settings";
    public static readonly string ACHIEVEMENTS = "AchievementGlobal";
    public static readonly string ACHIEVEMENT_1 = "Achievement1";
    public static readonly string ACHIEVEMENT_2 = "Achievement2";
    public static readonly string ACHIEVEMENT_3 = "Achievement3";
    public static readonly string GUIDE = "Guide";
    public static readonly string PAUSE = "Pause";
    public static readonly string COMPLETE = "Complete";
    public static readonly string RESTART = "Restart";
    public static readonly string EXIT = "Exit";

    [Header("Optional: Custom Return Scene")]
    [Tooltip("Leave empty to use MainMenu as default return scene")]
    public string customReturnScene = "";

    [Header("Audio Settings")]
    [Tooltip("Assign the button click sound here")]
    public AudioClip buttonClickSound;
    [Tooltip("Assign the background music here (Greek Music)")]
    public AudioClip backgroundMusic;

    private AudioSource audioSource;
    private static GameObject musicObject; // Static reference to keep track of music across scenes

    private void Awake()
    {
        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        // Auto-setup ALL buttons in this scene
        SetupButtonListeners();
        // Setup Background Music
        SetupBackgroundMusic();
    }

    private void SetupBackgroundMusic()
    {
        // If music is already playing, don't restart it
        if (musicObject != null) return;

        if (backgroundMusic != null)
        {
            musicObject = new GameObject("UI_BackgroundMusic");
            AudioSource musicSource = musicObject.AddComponent<AudioSource>();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = 0.5f; // Adjust volume as needed
            musicSource.Play();
            DontDestroyOnLoad(musicObject);
            Debug.Log("[SceneController] Background Music Started");
        }
    }

    /// <summary>
    /// Automatically find and setup ALL button listeners
    /// Works for MainMenu, Settings, Pause, Complete, Exit, etc.
    /// Searches by button name - NO MANUAL SETUP NEEDED!
    /// </summary>
    private void SetupButtonListeners()
    {
        // ===== MAIN MENU BUTTONS =====
        ConnectButton("ContinueButton", OnContinueButtonClicked);
        ConnectButton("NewgameButton", OnNewGameButtonClicked);
        ConnectButton("AchievementButton", OnAchievementButtonClicked);
        ConnectButton("SettingsButton", OnSettingsButtonClicked);
        ConnectButton("ExitgameButton", OnExitGameButtonClicked);

        // ===== COMMON NAVIGATION BUTTONS =====
        ConnectButton("BackButton", OnBackButtonClicked);
        ConnectButton("MainmenuButton", OnMainMenuButtonClicked);
        ConnectButton("HomeButton", OnMainMenuButtonClicked);

        // ===== CONFIRMATION BUTTONS =====
        ConnectButton("YesButton", OnYesButtonClicked);
        ConnectButton("NoButton", OnNoButtonClicked);

        // ===== PAUSE MENU BUTTONS =====
        ConnectButton("RestartButton", OnRestartButtonClicked);

        // ===== COMPLETE SCREEN BUTTONS =====
        ConnectButton("NextlevelButton", OnNextLevelButtonClicked);

        // ===== SETTINGS BUTTONS =====
        ConnectButton("MusicButton", OnMusicButtonClicked);
        ConnectButton("GuideButton", OnGuideButtonClicked);

        // ===== ACHIEVEMENT NAVIGATION BUTTONS =====
        ConnectButton("Next", OnNextAchievementClicked);
        ConnectButton("Back", OnBackAchievementClicked);

        Debug.Log($"[SceneController] Auto-setup complete for scene: {SceneManager.GetActiveScene().name}");
    }

    /// <summary>
    /// Helper to connect button by name to a handler
    /// </summary>
    private void ConnectButton(string buttonName, UnityEngine.Events.UnityAction handler)
    {
        Button button = FindButtonByName(buttonName);
        if (button != null)
        {
            button.onClick.AddListener(handler);
            // Add sound listener
            button.onClick.AddListener(PlayButtonSound);
            // Debug.Log($"[SceneController] ✓ Connected: {buttonName}"); // Removed to reduce console spam
        }
    }

    private void PlayButtonSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    /// <summary>
    /// Helper method to find button by name
    /// </summary>
    private Button FindButtonByName(string buttonName)
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name == buttonName)
            {
                return btn;
            }
        }
        // No warning - it's normal for some scenes not to have all buttons
        return null;
    }

    // ========== BUTTON HANDLERS ==========

    // ----- MAIN MENU BUTTONS -----
    public void OnContinueButtonClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Pause menu: Resume game (close pause, return to gameplay)
        if (currentScene == "Pause")
        {
            Debug.Log("[SceneController] Resume → Close Pause Menu");
            // TODO: Resume game logic - unload pause scene additively or Time.timeScale = 1
            Time.timeScale = 1f; // Resume time
            SceneManager.UnloadSceneAsync("Pause");
        }
        else
        {
            // Main Menu: Continue to level selection
            Debug.Log("[SceneController] Continue → SelectLevel");
            LoadScene(SELECT_LEVEL);
        }
    }

    public void OnNewGameButtonClicked()
    {
        Debug.Log("[SceneController] New Game → SelectLevel");
        PlayerPrefs.DeleteKey("HasSaveFile");
        PlayerPrefs.Save();
        LoadScene(SELECT_LEVEL);
    }

    public void OnAchievementButtonClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Pause menu: Just go to achievements
        if (currentScene == "Pause")
        {
            Debug.Log("[SceneController] Achievement from Pause");
            LoadScene(ACHIEVEMENTS);
        }
        else
        {
            // Main Menu: Go to achievements
            Debug.Log("[SceneController] Achievement");
            LoadScene(ACHIEVEMENTS);
        }
    }

    public void OnSettingsButtonClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Pause menu or Main Menu: Go to settings
        Debug.Log("[SceneController] Settings");
        LoadScene(SETTINGS);
    }

    public void OnExitGameButtonClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // MainMenu exits game directly
        if (currentScene == "MainMenu")
        {
            Debug.Log("[SceneController] Exit Game - Quitting application");
            QuitGame();
        }
        else
        {
            // Other scenes (like Pause) go to Exit confirmation
            Debug.Log("[SceneController] Exit Game → Exit confirmation scene");
            LoadScene(EXIT);
        }
    }

    // ----- COMMON NAVIGATION BUTTONS -----
    public void OnBackButtonClicked()
    {
        Debug.Log("[SceneController] Back");
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Determine where to go based on current scene
        if (currentScene == "Guide")
        {
            // Guide returns to Settings
            LoadScene(SETTINGS);
        }
        else if (currentScene == "Settings")
        {
            // Settings returns to Main Menu (or Pause if came from there)
            // For now, always return to Main Menu
            LoadScene(MAIN_MENU);
        }
        else if (currentScene == "SelectLevel")
        {
            // Select Level returns to Main Menu
            LoadScene(MAIN_MENU);
        }
        else if (currentScene == "AchievementGlobal" || currentScene == "Achievement1" || 
                 currentScene == "Achievement2" || currentScene == "Achievement3")
        {
            // Achievements return to Main Menu
            LoadScene(MAIN_MENU);
        }
        else
        {
            // Default: return to main menu or custom return scene
            string returnScene = string.IsNullOrEmpty(customReturnScene) ? MAIN_MENU : customReturnScene;
            LoadScene(returnScene);
        }
    }

    public void OnMainMenuButtonClicked()
    {
        Debug.Log("[SceneController] Main Menu");
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Pause")
        {
            // Resume time first
            Time.timeScale = 1f;
        }
        
        LoadScene(MAIN_MENU);
    }

    // ----- CONFIRMATION BUTTONS -----
    public void OnYesButtonClicked()
    {
        Debug.Log("[SceneController] Yes");
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Restart")
        {
            // TODO: Restart current level - implement level restart logic
            Debug.Log("[SceneController] TODO: Restart current level");
        }
        else if (currentScene == "Exit")
        {
            // Exit confirmed - go to main menu
            Debug.Log("[SceneController] Exit confirmed → MainMenu");
            LoadScene(MAIN_MENU);
        }
    }

    public void OnNoButtonClicked()
    {
        Debug.Log("[SceneController] No → Back to Pause");
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Both Restart and Exit scenes return to Pause when No is clicked
        if (currentScene == "Restart" || currentScene == "Exit")
        {
            LoadScene(PAUSE);
        }
        else
        {
            OnBackButtonClicked();
        }
    }

    // ----- PAUSE MENU BUTTONS -----
    public void OnRestartButtonClicked()
    {
        Debug.Log("[SceneController] Restart → Restart Level");
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Pause")
        {
            // Resume time first
            Time.timeScale = 1f;
            // Reload the level scene (level1)
            SceneManager.LoadScene("level1");
        }
        else
        {
            // From Restart confirmation scene
            LoadScene(RESTART);
        }
    }

    // ----- COMPLETE SCREEN BUTTONS -----
    public void OnNextLevelButtonClicked()
    {
        Debug.Log("[SceneController] Next Level");
        // TODO: Load next level from progression system
        LoadScene(SELECT_LEVEL);
    }

    // ----- SETTINGS BUTTONS -----
    public void OnMusicButtonClicked()
    {
        Debug.Log("[SceneController] Music Toggle");
        // TODO: Toggle music via AudioManager
    }

    public void OnGuideButtonClicked()
    {
        Debug.Log("[SceneController] Guide");
        LoadScene(GUIDE);
    }

    // ----- ACHIEVEMENT NAVIGATION -----
    public void OnNextAchievementClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneController] Next Achievement from: {currentScene}");

        // Navigate through achievement pages: Global→1, 1→2, 2→3, 3→Global
        if (currentScene == "AchievementGlobal")
        {
            LoadScene(ACHIEVEMENT_1);
        }
        else if (currentScene == "Achievement1")
        {
            LoadScene(ACHIEVEMENT_2);
        }
        else if (currentScene == "Achievement2")
        {
            LoadScene(ACHIEVEMENT_3);
        }
        else if (currentScene == "Achievement3")
        {
            LoadScene(ACHIEVEMENTS); // Back to AchievementGlobal
        }
    }

    public void OnBackAchievementClicked()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneController] Back Achievement from: {currentScene}");

        // Navigate backwards: 1→Global, Global→3, 3→2, 2→1
        if (currentScene == "AchievementGlobal")
        {
            LoadScene(ACHIEVEMENT_3);
        }
        else if (currentScene == "Achievement1")
        {
            LoadScene(ACHIEVEMENTS); // Back to AchievementGlobal
        }
        else if (currentScene == "Achievement2")
        {
            LoadScene(ACHIEVEMENT_1);
        }
        else if (currentScene == "Achievement3")
        {
            LoadScene(ACHIEVEMENT_2);
        }
    }

    // ----- UTILITY METHODS -----
    private void QuitGame()
    {
        Debug.Log("[SceneController] Quitting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Load scene by name
    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneController] Scene name is empty!");
            return;
        }

        Debug.Log($"[SceneController] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
