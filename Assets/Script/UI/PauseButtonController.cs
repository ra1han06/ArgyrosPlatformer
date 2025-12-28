using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller untuk tombol pause di game.
/// Attach script ini ke PauseButton GameObject.
/// </summary>
[RequireComponent(typeof(Button))]
public class PauseButtonController : MonoBehaviour
{
    private Button button;
    private PauseManager pauseManager;
    
    void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnPauseButtonClicked);
            Debug.Log("[PauseButtonController] ✅ Pause button listener added");
        }
        else
        {
            Debug.LogError("[PauseButtonController] ❌ Button component not found!");
        }
    }
    
    void Start()
    {
        // Cari PauseManager di scene
        pauseManager = FindFirstObjectByType<PauseManager>();
        
        if (pauseManager == null)
        {
            Debug.LogError("[PauseButtonController] PauseManager not found in scene!");
        }
    }
    
    public void OnPauseButtonClicked()
    {
        Debug.Log("[PauseButtonController] 🔘 Pause button clicked");
        
        if (pauseManager != null)
        {
            pauseManager.Pause();
        }
        else
        {
            Debug.LogError("[PauseButtonController] PauseManager is null!");
        }
    }
}
