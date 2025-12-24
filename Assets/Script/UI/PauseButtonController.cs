using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller for the pause button in gameplay scene
/// Opens the Pause menu scene when clicked
/// </summary>
public class PauseButtonController : MonoBehaviour
{
    /// <summary>
    /// Called when the pause button is clicked
    /// Loads the Pause scene additively and pauses the game
    /// </summary>
    public void OnPauseButtonClicked()
    {
        // Pause the game
        Time.timeScale = 0f;
        
        // Load Pause scene additively
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
    }
}
