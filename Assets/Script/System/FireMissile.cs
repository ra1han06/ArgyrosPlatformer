using UnityEngine;

public class FireMissile : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    
    [Header("Collision Settings")]
    [SerializeField] private LayerMask platformLayer = 1 << 6; // Layer 6 = Platform
    
    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool hasTarget = false;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }

    void Start()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("FireMissile: Rigidbody component missing! Add Rigidbody to prefab.");
            return;
        }
        
        // Set velocity - if has target, aim toward it, otherwise fall down
        if (hasTarget)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            // Default: fall down (for manual spawned missiles)
            rb.linearVelocity = Vector3.down * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ===== KENA PLAYER =====
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player kena api → MATI");
            Destroy(other.gameObject);
            Destroy(gameObject);
            return;
        }

        // ===== KENA PLATFORM (Layer-based detection) =====
        // Check jika object ada di platform layer
        if (((1 << other.gameObject.layer) & platformLayer) != 0)
        {
            // cari PlatformHazard di CHILD platform atau di parent
            PlatformHazard hazard = other.GetComponentInChildren<PlatformHazard>();
            
            // Jika tidak ada di child, coba cari di parent
            if (hazard == null)
            {
                hazard = other.GetComponentInParent<PlatformHazard>();
            }

            if (hazard != null)
            {
                hazard.ActivateDanger();
                Debug.Log($"Platform '{other.gameObject.name}' kena api → jadi berbahaya selama {hazard.dangerTime}s");
            }
            else
            {
                Debug.LogWarning($"PlatformHazard tidak ditemukan di '{other.gameObject.name}'!");
            }

            // hancurkan missile setelah kena platform (tidak tembus)
            Destroy(gameObject);
        }
    }
}
