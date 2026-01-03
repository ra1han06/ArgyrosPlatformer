 using UnityEngine;

/// <summary>
/// Handles sequential tutorial displays for vase interactions.
/// Each press of interact key shows the next tutorial in sequence.
/// </summary>
public class VaseSequentialTutorial : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float interactionRadius = 2f;

    [Header("Tutorial Sequence")]
    [Tooltip("Tutorial panels to show in sequence. Will loop through these in order.")]
    [SerializeField] private GameObject[] tutorialPanels;

    [Header("UI References")]
    [SerializeField] private GameObject interactionPrompt;

    private bool playerInRange = false;
    private int currentTutorialIndex = -1; // -1 means no tutorial active
    private bool tutorialActive = false;

    private void Start()
    {
        // Setup trigger collider for interaction
        SetupTriggerCollider();

        // Hide interaction prompt at start
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Ensure all tutorial panels are inactive at start
        foreach (var panel in tutorialPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Check for interact key press when player is in range
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            HandleInteraction();
        }

        // Allow closing tutorial with Escape key
        if (tutorialActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllTutorials();
        }
    }

    private void HandleInteraction()
    {
        if (tutorialPanels == null || tutorialPanels.Length == 0)
        {
            Debug.LogWarning("[VaseSequentialTutorial] No tutorial panels assigned!");
            return;
        }

        // If no tutorial is active, start from beginning
        if (!tutorialActive)
        {
            currentTutorialIndex = 0;
            ShowTutorial(currentTutorialIndex);
            tutorialActive = true;
        }
        else
        {
            // Hide current tutorial
            if (currentTutorialIndex >= 0 && currentTutorialIndex < tutorialPanels.Length)
            {
                tutorialPanels[currentTutorialIndex].SetActive(false);
            }

            // Move to next tutorial
            currentTutorialIndex++;

            // If we've reached the end, close all and reset
            if (currentTutorialIndex >= tutorialPanels.Length)
            {
                CloseAllTutorials();
            }
            else
            {
                // Show next tutorial
                ShowTutorial(currentTutorialIndex);
            }
        }
    }

    private void ShowTutorial(int index)
    {
        if (index >= 0 && index < tutorialPanels.Length && tutorialPanels[index] != null)
        {
            tutorialPanels[index].SetActive(true);
        }
    }

    private void CloseAllTutorials()
    {
        // Hide all tutorial panels
        foreach (var panel in tutorialPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        currentTutorialIndex = -1;
        tutorialActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    private void SetupTriggerCollider()
    {
        // Check if there's already a trigger collider
        BoxCollider existingTrigger = null;
        foreach (var col in GetComponents<BoxCollider>())
        {
            if (col.isTrigger)
            {
                existingTrigger = col;
                break;
            }
        }

        // If no trigger exists, create one
        if (existingTrigger == null)
        {
            BoxCollider triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            
            // Copy size from existing collider if available
            BoxCollider regularCollider = GetComponent<BoxCollider>();
            if (regularCollider != null && !regularCollider.isTrigger)
            {
                triggerCollider.center = regularCollider.center;
                triggerCollider.size = regularCollider.size;
            }

            existingTrigger = triggerCollider;
        }

        // Update trigger size based on interaction radius
        UpdateTriggerSize(existingTrigger);
    }

    private void UpdateTriggerSize(BoxCollider triggerCollider)
    {
        if (triggerCollider != null)
        {
            Vector3 newSize = triggerCollider.size;
            newSize.x = Mathf.Max(newSize.x, interactionRadius);
            newSize.z = Mathf.Max(newSize.z, interactionRadius);
            triggerCollider.size = newSize;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw interaction range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
