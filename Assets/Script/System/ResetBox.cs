using UnityEngine;

public class ResetBox : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt; // Optional UI prompt
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private Color usedColor = Color.gray; // Color when box is already used
    
    private bool playerInRange = false;
    private bool hasBeenUsed = false; // Track if box has been used
    private PlayerPlatformInteractor playerInteractor;
    private Renderer boxRenderer;
    private Color originalColor;

    void Start()
    {
        boxRenderer = GetComponent<Renderer>();
        if (boxRenderer != null)
        {
            originalColor = boxRenderer.material.color;
        }

        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && playerInteractor != null && !hasBeenUsed)
        {
            if (Input.GetKeyDown(interactKey))
            {
                ResetPlayerActions();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerInteractor = other.GetComponent<PlayerPlatformInteractor>();
            
            // Only show visual feedback if box hasn't been used
            if (!hasBeenUsed)
            {
                // Show visual feedback
                if (boxRenderer != null)
                {
                    boxRenderer.material.color = highlightColor;
                }
                
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(true);
                }
                
                Debug.Log("Player entered reset box area. Press E to reset actions!");
            }
            else
            {
                Debug.Log("This reset box has already been used.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerInteractor = null;
            
            // Always hide UI when player leaves
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            // Reset visual feedback only if not used
            if (boxRenderer != null && !hasBeenUsed)
            {
                boxRenderer.material.color = originalColor;
            }
            
            Debug.Log("Player left reset box area.");
        }
    }

    private void ResetPlayerActions()
    {
        if (playerInteractor != null && !hasBeenUsed)
        {
            playerInteractor.ResetLimits();
            hasBeenUsed = true; // Mark box as used
            
            Debug.Log("✓ Copy/Cut/Paste actions have been reset!");
            
            // Hide UI immediately after use
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            // Change box color to indicate it's been used
            if (boxRenderer != null)
            {
                boxRenderer.material.color = usedColor;
            }
            
            // Optional: Add visual/audio feedback here
            PlayResetEffect();
        }
    }

    private void PlayResetEffect()
    {
        // You can add particle effects or sound effects here
        // Example: Play a particle effect when reset happens
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the trigger area in editor
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        BoxCollider trigger = GetComponent<BoxCollider>();
        if (trigger != null && trigger.isTrigger)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(trigger.center, trigger.size);
        }
    }
}
