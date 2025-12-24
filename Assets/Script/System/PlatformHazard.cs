using UnityEngine;

public class PlatformHazard : MonoBehaviour
{
    [Header("Danger Settings")]
    public float dangerTime = 3f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    
    [Header("Collision Settings")]
    [SerializeField] private bool useActiveDetection = true; // Deteksi aktif jika collision passive gagal
    [SerializeField] private float detectionInterval = 0.1f; // Check setiap 0.1 detik
    [SerializeField] private LayerMask playerLayer = 1 << 0; // Layer 0 = Default (Player)
    
    private bool isDanger = false;
    private Renderer platformRenderer;
    private Material platformMaterial;
    private Color originalColor;
    private Collider platformCollider;
    private float nextDetectionTime;

    void Start()
    {
        // Get renderer dari GameObject ini ATAU dari parent platform
        platformRenderer = GetComponent<Renderer>();
        
        if (platformRenderer == null)
        {
            // Coba cari di parent (karena PlatformHazard biasanya child dari platform)
            platformRenderer = GetComponentInParent<Renderer>();
        }
        
        if (platformRenderer != null)
        {
            // Clone material supaya tidak affect semua platform yang sama
            platformMaterial = platformRenderer.material; // Unity auto-clone saat akses .material
            originalColor = platformMaterial.color;
            Debug.Log($"PlatformHazard '{gameObject.name}': Renderer ditemukan! Original color: {originalColor}");
        }
        else
        {
            Debug.LogError($"PlatformHazard di '{gameObject.name}': Renderer tidak ditemukan di GameObject atau Parent!");
        }

        // Get collider untuk deteksi bounds
        platformCollider = GetComponent<Collider>();
        if (platformCollider == null)
        {
            platformCollider = GetComponentInParent<Collider>();
        }
    }

    void Update()
    {
        // Active detection: Check area di atas platform untuk player
        if (isDanger && useActiveDetection && Time.time >= nextDetectionTime)
        {
            nextDetectionTime = Time.time + detectionInterval;
            CheckForPlayerOnPlatform();
        }
    }

    public void ActivateDanger()
    {
        if (isDanger) return;
        if (platformRenderer == null || platformMaterial == null) return;

        isDanger = true;
        platformMaterial.color = dangerColor;
        
        Debug.Log($"PlatformHazard '{gameObject.name}' aktif selama {dangerTime}s");

        Invoke(nameof(DeactivateDanger), dangerTime);
    }

    void DeactivateDanger()
    {
        if (platformRenderer == null || platformMaterial == null) return;
        
        isDanger = false;
        platformMaterial.color = originalColor; // Restore ke warna original
        
        Debug.Log($"PlatformHazard '{gameObject.name}' kembali normal");
    }

    // Method untuk aktif detect player di area platform
    private void CheckForPlayerOnPlatform()
    {
        if (platformCollider == null) return;

        // Get bounds dari platform
        Bounds bounds = platformCollider.bounds;
        
        // Expand bounds sedikit ke atas untuk detect player yang berdiri di atas
        Vector3 center = bounds.center;
        center.y += bounds.extents.y; // Geser ke atas permukaan platform
        
        Vector3 halfExtents = new Vector3(
            bounds.extents.x,
            bounds.extents.y * 2, // Extend vertical untuk cover player height
            bounds.extents.z
        );

        // Check apakah ada collider player di area ini
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, playerLayer);
        
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"Active Detection: Player terdeteksi di platform berbahaya '{gameObject.name}' → MATI");
                Destroy(hit.gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDanger && other.CompareTag("Player"))
        {
            Debug.Log("Player injak platform panas (TriggerEnter) → MATI");
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Untuk CharacterController yang tidak trigger OnTriggerEnter
        if (isDanger && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player injak platform panas (CollisionEnter) → MATI");
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Continuous check saat player stay di platform
        if (isDanger && other.CompareTag("Player"))
        {
            Debug.Log("Player masih di platform panas (TriggerStay) → MATI");
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Continuous check untuk CharacterController
        if (isDanger && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player masih di platform panas (CollisionStay) → MATI");
            Destroy(collision.gameObject);
        }
    }
}
