using UnityEngine;

/// <summary>
/// Temporary script to test Win animation trigger
/// </summary>
public class WinAnimationTester : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[WinAnimationTester] Script started - Finding Player...");
        
        // Find the Player GameObject
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.LogError("[WinAnimationTester] ❌ Player not found!");
            return;
        }
        
        Debug.Log("[WinAnimationTester] ✅ Player found: " + player.name);
        
        // Get the PlayerAnimation component
        PlayerAnimation playerAnimation = player.GetComponent<PlayerAnimation>();
        
        if (playerAnimation == null)
        {
            Debug.LogError("[WinAnimationTester] ❌ PlayerAnimation component not found!");
            return;
        }
        
        Debug.Log("[WinAnimationTester] ✅ PlayerAnimation component found!");
        
        // Trigger the win animation
        Debug.Log("[WinAnimationTester] 🎯 Calling PlayWinAnimation()...");
        playerAnimation.PlayWinAnimation();
        
        Debug.Log("[WinAnimationTester] ✅ PlayWinAnimation() called successfully!");
        
        // Also get the animator and check its state
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            Debug.Log("[WinAnimationTester] Animator state info:");
            Debug.Log("  - Enabled: " + animator.enabled);
            Debug.Log("  - Current State: " + animator.GetCurrentAnimatorStateInfo(0).ToString());
        }
    }
}
