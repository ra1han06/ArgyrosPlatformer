using UnityEngine;

/// <summary>
/// Trigger untuk lava - player yang menyentuh akan langsung mati dan respawn
/// Attach script ini ke GameObject lava (child dari Terrain)
/// </summary>
public class LavaTrigger : MonoBehaviour
{
    [Header("Lava Settings")]
    [SerializeField] private bool useCollider = true; // True = Collider, False = Trigger
    [SerializeField] private string playerTag = "Player";
    
    [Header("Visual Feedback (Optional)")]
    [SerializeField] private Color lavaColor = new Color(1f, 0.3f, 0f, 1f); // Orange/Red
    [SerializeField] private bool showGizmo = true;

    private void Start()
    {
        // Auto-setup collider/trigger jika belum ada
        SetupCollider();
    }

    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        
        if (col == null)
        {
            // Tambahkan BoxCollider jika belum ada
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.isTrigger = !useCollider;
            Debug.Log($"[LavaTrigger] Auto-added BoxCollider to '{gameObject.name}'");
        }
        else
        {
            // Set trigger mode
            col.isTrigger = !useCollider;
        }
    }

    // Untuk Trigger mode (isTrigger = true)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log($"[LavaTrigger] Player touched lava '{gameObject.name}' → Death!");
            KillPlayer(other.gameObject);
        }
    }

    // Untuk Collider mode (isTrigger = false)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log($"[LavaTrigger] Player hit lava '{gameObject.name}' → Death!");
            KillPlayer(collision.gameObject);
        }
    }

    private void KillPlayer(GameObject player)
    {
        RespawnManager respawnManager = player.GetComponent<RespawnManager>();
        
        if (respawnManager != null)
        {
            respawnManager.TriggerDeath();
        }
        else
        {
            Debug.LogError("[LavaTrigger] Player tidak memiliki RespawnManager! Pastikan RespawnManager sudah di-attach ke Player.");
            // Fallback: destroy player
            Destroy(player);
        }
    }

    // Visualisasi di Scene View
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(lavaColor.r, lavaColor.g, lavaColor.b, 0.3f);
            
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = lavaColor;
            
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
        }
    }
}
