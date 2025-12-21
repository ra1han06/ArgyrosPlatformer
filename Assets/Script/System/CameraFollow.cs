using UnityEngine;

/// <summary>
/// Camera follows player position but keeps its own rotation
/// For 2.5D platformer - camera tidak ikut rotate
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Target yang akan diikuti (Player)")]
    [SerializeField] private Transform target;
    
    [Tooltip("Offset posisi camera dari target")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    [Tooltip("Kecepatan smoothing (0 = instant, 10 = very smooth)")]
    [SerializeField] private float smoothSpeed = 5f;
    
    [Header("Follow Axis")]
    [Tooltip("Ikuti posisi X target?")]
    [SerializeField] private bool followX = true;
    
    [Tooltip("Ikuti posisi Y target?")]
    [SerializeField] private bool followY = true;
    
    [Tooltip("Ikuti posisi Z target? (biasanya false untuk 2.5D)")]
    [SerializeField] private bool followZ = false;

    void LateUpdate()
    {
        if (target == null) return;
        
        // Hitung posisi target + offset
        Vector3 targetPosition = target.position + offset;
        
        // Tentukan posisi mana yang diikuti
        Vector3 desiredPosition = transform.position;
        
        if (followX) desiredPosition.x = targetPosition.x;
        if (followY) desiredPosition.y = targetPosition.y;
        if (followZ) desiredPosition.z = targetPosition.z;
        
        // Smooth movement
        if (smoothSpeed > 0)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = desiredPosition;
        }
        
        // PENTING: Rotation TIDAK berubah - camera tetap hadap satu arah
    }
}
