using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Mengelola sistem pause game, termasuk pause, resume, restart, dan navigasi ke main menu.
/// Attach script ini ke GameObject "PauseManager" di scene.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("UI to Hide on Pause")]
    [Tooltip("UI yang akan disembunyikan saat pause (Guide button, Music button, DeathTimerUI, dll)")]
    [SerializeField] private GameObject[] uiElementsToHide;
    
    [Header("Player Reference")]
    [SerializeField] private Transform player;
    
    // State
    private bool isPaused = false;
    private Vector3 playerPositionBeforePause;
    
    // Properties
    public bool IsPaused => isPaused;
    
    void Start()
    {
        // Pastikan panel tersembunyi di awal
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Auto-find UI elements yang perlu disembunyikan jika belum di-assign
        if (uiElementsToHide == null || uiElementsToHide.Length == 0)
        {
            AutoFindUIElements();
        }
        
        // Auto-find player jika belum di-assign
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("[PauseManager] Player found automatically");
            }
            else
            {
                Debug.LogWarning("[PauseManager] Player not found! Please assign manually or add 'Player' tag.");
            }
        }
        
        // Auto-find PauseMenuPanel jika belum di-assign
        if (pauseMenuPanel == null)
        {
            pauseMenuPanel = GameObject.Find("PauseMenuPanel");
            if (pauseMenuPanel != null)
            {
                Debug.Log("[PauseManager] PauseMenuPanel found automatically");
                pauseMenuPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("[PauseManager] PauseMenuPanel not found!");
            }
        }
    }
    
    /// <summary>
    /// Auto-find UI elements yang perlu disembunyikan saat pause
    /// </summary>
    private void AutoFindUIElements()
    {
        var foundElements = new System.Collections.Generic.List<GameObject>();
        
        // Cari Guide button
        GameObject guideButton = GameObject.Find("GuideButton");
        if (guideButton != null)
        {
            foundElements.Add(guideButton);
            Debug.Log("[PauseManager] GuideButton found automatically");
        }
        
        // Cari Music button
        GameObject musicButton = GameObject.Find("MusicButton");
        if (musicButton != null)
        {
            foundElements.Add(musicButton);
            Debug.Log("[PauseManager] MusicButton found automatically");
        }
        
        // Cari DeathTimerUI (bisa berupa GameObject atau Canvas child)
        GameObject deathTimerUI = GameObject.Find("DeathTimerUI");
        if (deathTimerUI != null)
        {
            foundElements.Add(deathTimerUI);
            Debug.Log("[PauseManager] DeathTimerUI found automatically");
        }
        
        // Convert list to array
        uiElementsToHide = foundElements.ToArray();
        
        if (uiElementsToHide.Length > 0)
        {
            Debug.Log($"[PauseManager] Auto-found {uiElementsToHide.Length} UI elements to hide on pause");
        }
        else
        {
            Debug.LogWarning("[PauseManager] No UI elements found to hide! Check GameObject names.");
        }
    }
    
    void Update()
    {
        // Toggle pause dengan ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    /// <summary>
    /// Pause game dan tampilkan pause menu
    /// </summary>
    public void Pause()
    {
        if (isPaused) return;
        
        Debug.Log("[PauseManager] ⏸️ Game Paused");
        
        // Simpan posisi player sebelum pause
        if (player != null)
        {
            playerPositionBeforePause = player.position;
        }
        
        // Set game state
        isPaused = true;
        Time.timeScale = 0f;
        
        // Stop timer
        GameManager.Instance?.StopTimer();
        
        // PAUSE BGM (Level Music)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseBGM();
        }
        
        // HIDE UI elements yang tidak diperlukan saat pause
        HideUIElements();
        
        // Tampilkan panel
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Resume game dan sembunyikan pause menu
    /// </summary>
    public void Resume()
    {
        if (!isPaused) return;
        
        Debug.Log("[PauseManager] ▶️ Game Resumed");
        
        // Kembalikan posisi player (optional, bisa di-comment jika tidak perlu)
        if (player != null)
        {
            player.position = playerPositionBeforePause;
        }
        
        // Set game state
        isPaused = false;
        Time.timeScale = 1f;
        
        // Resume timer
        GameManager.Instance?.StartTimer();
        
        // RESUME BGM (Level Music)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeBGM();
        }
        
        // SHOW kembali UI elements yang disembunyikan
        ShowUIElements();
        
        // Sembunyikan panel
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Restart level saat ini
    /// </summary>
    public void RestartLevel()
    {
        Debug.Log("[PauseManager] 🔄 Restarting Level");
        
        // Reset level stats (timer & deaths)
        GameManager.Instance?.ResetLevel();
        
        // Reset time scale sebelum load scene
        Time.timeScale = 1f;
        isPaused = false;
        
        // Reload active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    
    /// <summary>
    /// Pindah ke Main Menu
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("[PauseManager] 🏠 Going to Main Menu");
        
        // Reset time scale sebelum load scene
        Time.timeScale = 1f;
        isPaused = false;
        
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
    }
    
    // =====================================================
    // HELPER METHODS: HIDE/SHOW UI ELEMENTS
    // =====================================================
    
    /// <summary>
    /// Sembunyikan UI elements saat pause
    /// </summary>
    private void HideUIElements()
    {
        if (uiElementsToHide == null || uiElementsToHide.Length == 0)
        {
            return;
        }
        
        foreach (var uiElement in uiElementsToHide)
        {
            if (uiElement != null && uiElement.activeSelf)
            {
                uiElement.SetActive(false);
                Debug.Log($"[PauseManager] 👁️‍🗨️ Hiding {uiElement.name}");
            }
        }
    }
    
    /// <summary>
    /// Tampilkan kembali UI elements saat resume
    /// </summary>
    private void ShowUIElements()
    {
        if (uiElementsToHide == null || uiElementsToHide.Length == 0)
        {
            return;
        }
        
        foreach (var uiElement in uiElementsToHide)
        {
            if (uiElement != null && !uiElement.activeSelf)
            {
                uiElement.SetActive(true);
                Debug.Log($"[PauseManager] 👁️ Showing {uiElement.name}");
            }
        }
    }
}
