using UnityEngine;

public class PlayerPlatformInteractor : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private LayerMask platformLayer;

    [Header("Paste Settings")]
    [SerializeField] private float pasteOffset = 1.5f;

    [Header("Operation Limits")]
    [SerializeField] private int maxCopyCount = 1;
    [SerializeField] private int maxCutCount = 1;
    [SerializeField] private int maxPasteCount = 3;

    // Current usage counters
    private int currentCopyCount = 0;
    private int currentCutCount = 0;
    private int currentPasteCount = 0;

    // Clipboard data structure to store platform information
    private class ClipboardData
    {
        public GameObject platformPrefab;
        public Vector3 scale;
        public bool isCut;
    }

    private ClipboardData clipboard;
    private bool isFacingRight = true;

    void Update()
    {
        UpdateFacingDirection();

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.C))
                TryCopy();
            else if (Input.GetKeyDown(KeyCode.X))
                TryCut();
            else if (Input.GetKeyDown(KeyCode.V))
                TryPaste();
        }
    }

    private void UpdateFacingDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal > 0) isFacingRight = true;
        else if (horizontal < 0) isFacingRight = false;
    }

    private void TryCopy()
    {
        // Check copy limit
        if (currentCopyCount >= maxCopyCount)
        {
            Debug.LogWarning($"Copy limit reached! ({currentCopyCount}/{maxCopyCount})");
            return;
        }

        Debug.Log("TryCopy() called - attempting to copy platform");
        
        GameObject platform = RaycastPlatform();
        
        if (platform == null)
        {
            Debug.LogWarning("No platform detected by raycast!");
            return;
        }
        
        Debug.Log($"Platform detected: {platform.name}");
        
        clipboard = new ClipboardData
        {
            platformPrefab = platform,
            scale = platform.transform.localScale,
            isCut = false
        };
        currentCopyCount++;
        Debug.Log($"✓ Successfully copied platform: {platform.name} | Scale: {platform.transform.localScale} | Copies used: {currentCopyCount}/{maxCopyCount}");
    }

    private void TryCut()
    {
        // Check cut limit
        if (currentCutCount >= maxCutCount)
        {
            Debug.LogWarning($"Cut limit reached! ({currentCutCount}/{maxCutCount})");
            return;
        }

        GameObject platform = RaycastPlatform();
        if (platform && platform.GetComponent<CutablePlatform>())
        {
            clipboard = new ClipboardData
            {
                platformPrefab = platform,
                scale = platform.transform.localScale,
                isCut = true
            };
            platform.SetActive(false); // Hide instead of destroy immediately
            currentCutCount++;
            Debug.Log($"Cut platform: {platform.name} | Cuts used: {currentCutCount}/{maxCutCount}");
        }
    }

    private void TryPaste()
    {
        // Check paste limit
        if (currentPasteCount >= maxPasteCount)
        {
            Debug.LogWarning($"Paste limit reached! ({currentPasteCount}/{maxPasteCount})");
            return;
        }

        if (clipboard == null || clipboard.platformPrefab == null) 
        {
            Debug.LogWarning("Clipboard is empty or platform reference is null!");
            return;
        }

        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        Vector3 pastePosition = transform.position + direction * pasteOffset;

        GameObject newPlatform = Instantiate(
            clipboard.platformPrefab,
            pastePosition,
            clipboard.platformPrefab.transform.rotation
        );
        newPlatform.transform.localScale = clipboard.scale;
        newPlatform.SetActive(true);

        currentPasteCount++;
        Debug.Log($"Pasted platform at position: {pastePosition} | Pastes used: {currentPasteCount}/{maxPasteCount}");

        // If it was a cut operation, destroy the original
        if (clipboard.isCut && clipboard.platformPrefab != null)
        {
            Destroy(clipboard.platformPrefab);
        }

        // Clear clipboard after paste (for cut) or keep it (for copy)
        if (clipboard.isCut)
        {
            clipboard = null;
        }
    }

    private GameObject RaycastPlatform()
    {
        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        
        Debug.DrawRay(transform.position, direction * raycastDistance, Color.yellow, 0.5f);
        Debug.Log($"Raycasting from {transform.position} in direction {direction} for distance {raycastDistance}");

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, raycastDistance, platformLayer))
        {
            Debug.Log($"Raycast HIT: {hit.collider.gameObject.name} at distance {hit.distance}");
            return hit.collider.gameObject;
        }

        Debug.Log("Raycast MISS: No platform found");
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position, direction * raycastDistance);
        
        // Draw a sphere to show raycast origin
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    /// <summary>
    /// Reset operation counters (call this when starting a new level)
    /// </summary>
    public void ResetLimits()
    {
        currentCopyCount = 0;
        currentCutCount = 0;
        currentPasteCount = 0;
        clipboard = null;
        Debug.Log("Operation limits reset!");
    }

    /// <summary>
    /// Set custom limits for this level (call from LevelManager)
    /// </summary>
    public void SetLimits(int copyLimit, int cutLimit, int pasteLimit)
    {
        maxCopyCount = copyLimit;
        maxCutCount = cutLimit;
        maxPasteCount = pasteLimit;
        ResetLimits();
        Debug.Log($"Limits set: Copy={copyLimit}, Cut={cutLimit}, Paste={pasteLimit}");
    }

    /// <summary>
    /// Get remaining operations for UI display
    /// </summary>
    public (int copy, int cut, int paste) GetRemainingOperations()
    {
        return (
            maxCopyCount - currentCopyCount,
            maxCutCount - currentCutCount,
            maxPasteCount - currentPasteCount
        );
    }

    /// <summary>
    /// Get current usage for UI display
    /// </summary>
    public (int copy, int cut, int paste) GetCurrentUsage()
    {
        return (currentCopyCount, currentCutCount, currentPasteCount);
    }

    /// <summary>
    /// Get max limits for UI display
    /// </summary>
    public (int copy, int cut, int paste) GetMaxLimits()
    {
        return (maxCopyCount, maxCutCount, maxPasteCount);
    }
}
