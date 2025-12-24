using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private float shootDelay = 0.5f;
    
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 50f; // Increased range
    [SerializeField] private float targetHeightOffset = 1.5f; // Offset ke atas untuk target ke body/head
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = false; // Turn off by default
    
    private float nextShootTime;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log($"Enemy '{gameObject.name}' found player!");
            }
            else
            {
                Debug.LogWarning($"Enemy '{gameObject.name}': Player not found!");
            }
        }

        if (shootPoint == null)
        {
            // Try to find existing ShootPoint first
            Transform existingShootPoint = transform.Find("ShootPoint");
            if (existingShootPoint != null)
            {
                shootPoint = existingShootPoint;
                Debug.Log($"Enemy '{gameObject.name}': Using existing ShootPoint at position {shootPoint.localPosition}");
            }
            else
            {
                // Create ShootPoint tanpa set position - bisa diatur manual di hierarchy
                GameObject sp = new GameObject("ShootPoint");
                sp.transform.SetParent(transform);
                // Position default (0,0,0) - atur manual di Inspector/Scene view
                shootPoint = sp.transform;
                Debug.Log($"Enemy '{gameObject.name}': ShootPoint created. Atur posisinya di hierarchy!");
            }
        }

        if (missilePrefab == null)
        {
            // Try to load prefab from Resources or Assets
            #if UNITY_EDITOR
            missilePrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/FlamesParticleEffect.prefab");
            if (missilePrefab != null)
            {
                Debug.Log($"Enemy '{gameObject.name}': FlamesParticleEffect Prefab auto-loaded!");
            }
            #endif
            
            if (missilePrefab == null)
            {
                Debug.LogError($"Enemy '{gameObject.name}': Missile Prefab not assigned!");
                enabled = false;
                return;
            }
        }

        nextShootTime = Time.time + shootDelay;
    }

    void Update()
    {
        if (target == null || missilePrefab == null) return;

        float dist = Vector3.Distance(transform.position, target.position);
        
        Debug.Log($"Enemy: Distance to player = {dist:F1}, Detection Range = {detectionRange}");
        
        if (dist <= detectionRange && Time.time >= nextShootTime)
        {
            ShootMissile();
            nextShootTime = Time.time + shootInterval;
        }
    }

    void ShootMissile()
    {
        if (missilePrefab == null || shootPoint == null) return;
        
        GameObject missile = Instantiate(missilePrefab, shootPoint.position, Quaternion.identity);
        
        // Pass target position to missile with height offset
        FireMissile fireMissile = missile.GetComponent<FireMissile>();
        if (fireMissile != null && target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * targetHeightOffset;
            fireMissile.SetTarget(targetPos);
        }
        
        Debug.Log($"Enemy '{gameObject.name}': Missile spawned at {shootPoint.position} targeting {target.position + Vector3.up * targetHeightOffset}");
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Detection range
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Shoot point
        if (shootPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(shootPoint.position, 0.3f);
            
            // Line to target
            if (target != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 targetPos = target.position + Vector3.up * targetHeightOffset;
                Gizmos.DrawLine(shootPoint.position, targetPos);
                
                // Draw sphere at target point
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(targetPos, 0.2f);
            }
        }
    }

    // Public methods untuk control
    public void EnableShooting() { enabled = true; }
    public void DisableShooting() { enabled = false; }
    public void SetTarget(Transform newTarget) { target = newTarget; }
    public void ShootNow() 
    { 
        ShootMissile();
        nextShootTime = Time.time + shootInterval;
    }
}
