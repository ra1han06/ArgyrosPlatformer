using UnityEngine;

/// <summary>
/// Platform berbahaya permanen yang instant kill player saat disentuh.
/// Platform akan glow merah (emissive) dan memiliki efek api.
/// Compatible dengan sistem copy/paste platform.
/// </summary>
public class PermanentHazardPlatform : MonoBehaviour
{
    [Header("Hazard Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool enableDebugLog = true;
    
    [Header("Visual Settings")]
    [SerializeField] private Color hazardColor = Color.red;
    [SerializeField] private float emissionIntensity = 3f;
    
    [Header("Gizmo Settings")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);

    private Renderer[] renderers;
    private Material[] clonedMaterials;

    private void Start()
    {
        SetupEmissiveMaterials();
        SetupCollider();
    }

    /// <summary>
    /// Setup emissive material untuk semua renderers di platform.
    /// Clone material agar tidak mengubah asset asli.
    /// </summary>
    private void SetupEmissiveMaterials()
    {
        // Ambil semua renderer dari platform (termasuk children)
        renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"[PermanentHazardPlatform] Tidak ada Renderer di '{gameObject.name}'!");
            return;
        }

        // Clone materials untuk setiap renderer
        clonedMaterials = new Material[renderers.Length];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            // Clone material (agar tidak mengubah asset asli)
            Material mat = renderers[i].material; // Ini auto-clone material
            clonedMaterials[i] = mat;

            // Enable emission
            mat.EnableKeyword("_EMISSION");
            
            // Set emissive color
            mat.SetColor("_EmissionColor", hazardColor * emissionIntensity);
            
            // Set base color ke merah
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", hazardColor);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", hazardColor);
            }

            if (enableDebugLog)
            {
                Debug.Log($"[PermanentHazardPlatform] Material '{mat.name}' di '{renderers[i].gameObject.name}' setup dengan emission.");
            }
        }
    }

    /// <summary>
    /// Auto-setup collider jika belum ada.
    /// </summary>
    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        
        if (col == null)
        {
            // Tambahkan BoxCollider jika belum ada
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;
            
            if (enableDebugLog)
            {
                Debug.Log($"[PermanentHazardPlatform] Auto-added BoxCollider (Trigger) to '{gameObject.name}'");
            }
        }
        else
        {
            if (enableDebugLog)
            {
                Debug.Log($"[PermanentHazardPlatform] Collider sudah ada di '{gameObject.name}' (isTrigger={col.isTrigger})");
            }
        }
    }

    /// <summary>
    /// Untuk Trigger mode (isTrigger = true).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (enableDebugLog)
            {
                Debug.Log($"[PermanentHazardPlatform] Player touched hazard platform '{gameObject.name}' (Trigger) → DEATH!");
            }
            KillPlayer(other.gameObject);
        }
    }

    /// <summary>
    /// Untuk Collider mode (isTrigger = false).
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            if (enableDebugLog)
            {
                Debug.Log($"[PermanentHazardPlatform] Player hit hazard platform '{gameObject.name}' (Collision) → DEATH!");
            }
            KillPlayer(collision.gameObject);
        }
    }

    /// <summary>
    /// Instant kill player menggunakan RespawnManager.
    /// </summary>
    private void KillPlayer(GameObject player)
    {
        RespawnManager respawnManager = player.GetComponent<RespawnManager>();
        
        if (respawnManager != null)
        {
            respawnManager.TriggerDeath();
        }
        else
        {
            Debug.LogError($"[PermanentHazardPlatform] Player tidak memiliki RespawnManager! Pastikan RespawnManager sudah di-attach ke Player.");
            // Fallback: destroy player
            Destroy(player);
        }
    }

    /// <summary>
    /// Visualisasi area berbahaya di Scene View.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = gizmoColor;
            
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
            else if (col is MeshCollider meshCol)
            {
                // Untuk mesh collider, gambar wireframe bounds
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }

    /// <summary>
    /// Visualisasi terpilih (selected) di Scene View.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.8f);
            
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
        }
    }
}