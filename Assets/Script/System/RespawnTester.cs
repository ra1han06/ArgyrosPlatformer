using UnityEngine;

/// <summary>
/// Helper script untuk testing sistem respawn
/// Attach ke Player untuk test respawn dengan hotkey
/// </summary>
public class RespawnTester : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private KeyCode testDeathKey = KeyCode.K; // Tekan K untuk test death
    
    private RespawnManager respawnManager;

    void Start()
    {
        respawnManager = GetComponent<RespawnManager>();
        
        if (respawnManager == null)
        {
            Debug.LogError("RespawnTester: RespawnManager tidak ditemukan! Pastikan RespawnManager sudah di-attach ke Player.");
        }
        else
        {
            Debug.Log($"RespawnTester siap! Tekan {testDeathKey} untuk test respawn.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(testDeathKey))
        {
            Debug.Log("=== TESTING RESPAWN SYSTEM ===");
            
            if (respawnManager != null)
            {
                respawnManager.TriggerDeath();
            }
        }
    }
}
