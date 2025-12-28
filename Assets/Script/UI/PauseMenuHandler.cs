using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handler untuk tombol-tombol di Pause Menu.
/// Attach script ini ke PauseMenuPanel.
/// </summary>
public class PauseMenuHandler : MonoBehaviour
{
    private PauseManager pauseManager;
    
    void Start()
    {
        // Cari PauseManager di scene
        pauseManager = FindFirstObjectByType<PauseManager>();
        
        if (pauseManager == null)
        {
            Debug.LogError("[PauseMenuHandler] PauseManager not found in scene!");
            return;
        }
        
        // Auto-connect buttons
        ConnectButtons();
    }
    
    void OnEnable()
    {
        // Setiap kali panel aktif, pastikan button ter-connect
        if (pauseManager == null)
        {
            pauseManager = FindFirstObjectByType<PauseManager>();
        }
    }
    
    /// <summary>
    /// Otomatis menghubungkan tombol-tombol dengan method yang sesuai
    /// </summary>
    private void ConnectButtons()
    {
        // Resume Button
        Button resumeBtn = transform.Find("ResumeButton")?.GetComponent<Button>();
        if (resumeBtn != null)
        {
            resumeBtn.onClick.RemoveAllListeners();
            resumeBtn.onClick.AddListener(OnResumeClicked);
            Debug.Log("[PauseMenuHandler] ✅ ResumeButton connected");
        }
        
        // Restart Button
        Button restartBtn = transform.Find("RestartButton")?.GetComponent<Button>();
        if (restartBtn != null)
        {
            restartBtn.onClick.RemoveAllListeners();
            restartBtn.onClick.AddListener(OnRestartClicked);
            Debug.Log("[PauseMenuHandler] ✅ RestartButton connected");
        }
        
        // Main Menu Button
        Button mainMenuBtn = transform.Find("MainMenuButton")?.GetComponent<Button>();
        if (mainMenuBtn != null)
        {
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(OnMainMenuClicked);
            Debug.Log("[PauseMenuHandler] ✅ MainMenuButton connected");
        }
        
        // Settings Button (optional - bisa dikembangkan nanti)
        Button settingsBtn = transform.Find("SettingsButton")?.GetComponent<Button>();
        if (settingsBtn != null)
        {
            settingsBtn.onClick.RemoveAllListeners();
            settingsBtn.onClick.AddListener(OnSettingsClicked);
            Debug.Log("[PauseMenuHandler] ✅ SettingsButton connected");
        }
    }
    
    // === Button Click Handlers ===
    
    public void OnResumeClicked()
    {
        Debug.Log("[PauseMenuHandler] Resume button clicked");
        if (pauseManager != null)
        {
            pauseManager.Resume();
        }
    }
    
    public void OnRestartClicked()
    {
        Debug.Log("[PauseMenuHandler] Restart button clicked");
        if (pauseManager != null)
        {
            pauseManager.RestartLevel();
        }
    }
    
    public void OnMainMenuClicked()
    {
        Debug.Log("[PauseMenuHandler] Main Menu button clicked");
        if (pauseManager != null)
        {
            pauseManager.GoToMainMenu();
        }
    }
    
    public void OnSettingsClicked()
    {
        Debug.Log("[PauseMenuHandler] Settings button clicked (not implemented yet)");
        // TODO: Implement settings menu
    }
}
