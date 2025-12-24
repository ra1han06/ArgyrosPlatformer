    using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoRadius = 1f;
    [SerializeField] private Vector3 gizmoOffset = Vector3.zero; // Offset dari posisi GameObject

    public Vector3 GetRespawnPosition()
    {
        return transform.position + gizmoOffset;
    }

    public Quaternion GetRespawnRotation()
    {
        return transform.rotation;
    }

    // Visualisasi di Scene View
    private void OnDrawGizmos()
    {
        Vector3 gizmoPosition = transform.position + gizmoOffset;
        
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(gizmoPosition, gizmoRadius);
        
        // Draw arrow untuk show direction
        Gizmos.color = Color.blue;
        Vector3 direction = transform.forward * gizmoRadius;
        Gizmos.DrawRay(gizmoPosition, direction);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 gizmoPosition = transform.position + gizmoOffset;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gizmoPosition, gizmoRadius * 0.5f);
    }
}
