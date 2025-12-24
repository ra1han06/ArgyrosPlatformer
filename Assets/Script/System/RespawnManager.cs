using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private RespawnPoint respawnPoint;
    [SerializeField] private float respawnDelay = 1f;
    
    [Header("Death Detection")]
    [SerializeField] private float deathY = -10f; // Jika player jatuh ke bawah threshold ini
    
    [Header("Visual Settings")]
    [SerializeField] private bool hidePlayerOnDeath = true;
    
    private CharacterController characterController;
    private PlayerController playerController;
    private Renderer[] playerRenderers;
    private Vector3 lastSafePosition;
    private bool isDead = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        playerRenderers = GetComponentsInChildren<Renderer>();
        
        // Cari RespawnPoint jika belum di-assign
        if (respawnPoint == null)
        {
            respawnPoint = FindFirstObjectByType<RespawnPoint>();
            
            if (respawnPoint == null)
            {
                Debug.LogWarning("RespawnManager: Tidak menemukan RespawnPoint! Menggunakan posisi awal player.");
            }
            else
            {
                Debug.Log($"RespawnManager: RespawnPoint ditemukan di {respawnPoint.transform.position}");
            }
        }
        
        // Set starting position sebagai safe position
        lastSafePosition = transform.position;
    }

    void Update()
    {
        // Check apakah player jatuh ke void
        if (!isDead && transform.position.y < deathY)
        {
            Debug.Log("Player jatuh ke void!");
            Die();
        }
        
        // Update safe position saat di ground
        if (characterController != null && characterController.isGrounded)
        {
            lastSafePosition = transform.position;
        }
    }

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player mati! Respawn dalam " + respawnDelay + " detik...");
        
        // Disable player control
        if (playerController != null)
        {
            playerController.canMove = false;
        }
        
        // Hide player visual
        if (hidePlayerOnDeath)
        {
            SetPlayerVisible(false);
        }
        
        // Respawn setelah delay
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        isDead = false;
        
        // Disable CharacterController untuk teleport
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        
        // Tentukan posisi respawn
        Vector3 respawnPosition;
        Quaternion respawnRotation;
        
        if (respawnPoint != null)
        {
            respawnPosition = respawnPoint.GetRespawnPosition();
            respawnRotation = respawnPoint.GetRespawnRotation();
            Debug.Log($"Respawn ke RespawnPoint: {respawnPosition}");
        }
        else
        {
            respawnPosition = lastSafePosition;
            respawnRotation = transform.rotation;
            Debug.Log($"Respawn ke last safe position: {respawnPosition}");
        }
        
        // Teleport player
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;
        
        // Re-enable CharacterController
        if (characterController != null)
        {
            characterController.enabled = true;
        }
        
        // Show player visual
        SetPlayerVisible(true);
        
        // Re-enable player control
        if (playerController != null)
        {
            playerController.canMove = true;
        }
        
        Debug.Log("Player respawned!");
    }

    private void SetPlayerVisible(bool visible)
    {
        if (playerRenderers == null || playerRenderers.Length == 0) return;
        
        foreach (Renderer renderer in playerRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }

    // Method public untuk dipanggil dari script lain (seperti PlatformHazard)
    public void TriggerDeath()
    {
        Die();
    }
}
