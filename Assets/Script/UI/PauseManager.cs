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
}
