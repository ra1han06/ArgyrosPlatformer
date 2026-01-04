using UnityEngine;

/// <summary>
/// Relay script untuk forward Animation Events dari child (girl18) ke parent (Enemy).
/// Dibutuhkan karena animasi jalan di child tapi logic ada di parent script.
/// </summary>
public class AnimationEventRelay : MonoBehaviour
{
    private EnemyShooter shooter;

    void Awake()
    {
        // Get EnemyShooter from parent GameObject
        shooter = GetComponentInParent<EnemyShooter>();
        
        if (shooter == null)
        {
            Debug.LogError($"[AnimationEventRelay] EnemyShooter not found in parent of '{gameObject.name}'!");
        }
        else
        {
            Debug.Log($"[AnimationEventRelay] Successfully linked '{gameObject.name}' to EnemyShooter on '{shooter.gameObject.name}'");
        }
    }

    /// <summary>
    /// Called from Animation Event - forwards call to parent EnemyShooter
    /// </summary>
    public void SpawnMissile()
    {
        Debug.Log($"[AnimationEventRelay] SpawnMissile() called on '{gameObject.name}'");
        
        if (shooter != null)
        {
            Debug.Log($"[AnimationEventRelay] Forwarding to shooter.SpawnMissile()");
            shooter.SpawnMissile();
        }
        else
        {
            Debug.LogWarning($"[AnimationEventRelay] Cannot spawn missile - EnemyShooter not found!");
        }
    }
}
