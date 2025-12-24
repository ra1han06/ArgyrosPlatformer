using UnityEngine;

public class AutoAddPlatformHazards : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private string targetParentName = "BossGround";

    void Start()
    {
        if (runOnStart)
        {
            AddHazardsToAllPlatforms();
        }
    }

    [ContextMenu("Add PlatformHazard to All Platforms")]
    public void AddHazardsToAllPlatforms()
    {
        Debug.Log($"[AutoAddPlatformHazards] Starting to search for '{targetParentName}'...");
        
        GameObject targetParent = GameObject.Find(targetParentName);
        if (targetParent == null)
        {
            Debug.LogError($"'{targetParentName}' not found in scene!");
            return;
        }

        Debug.Log($"[AutoAddPlatformHazards] Found '{targetParentName}', scanning children...");
        
        int addedCount = 0;
        int skippedCount = 0;
        int totalChildren = 0;

        // Get all children recursively
        Transform[] allChildren = targetParent.GetComponentsInChildren<Transform>(true);
        Debug.Log($"[AutoAddPlatformHazards] Total transforms found: {allChildren.Length}");
        
        foreach (Transform child in allChildren)
        {
            totalChildren++;
            
            // Skip parent itself
            if (child == targetParent.transform) continue;

            // Check if it has a collider (likely a platform)
            Collider col = child.GetComponent<Collider>();
            if (col != null)
            {
                // Check if already has PlatformHazard
                PlatformHazard existing = child.GetComponent<PlatformHazard>();
                if (existing == null)
                {
                    // Add PlatformHazard component
                    PlatformHazard hazard = child.gameObject.AddComponent<PlatformHazard>();
                    addedCount++;
                    Debug.Log($"✅ Added PlatformHazard to '{child.name}'");
                }
                else
                {
                    skippedCount++;
                    Debug.Log($"⏭️ Skipped '{child.name}' (already has PlatformHazard)");
                }
            }
        }

        Debug.Log($"<color=green>✅ DONE! Scanned {totalChildren} objects. Added PlatformHazard to {addedCount} platforms. Skipped {skippedCount}.</color>");
    }
}
