using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    [Header("Animation Settings")]
    [SerializeField] private float walkThreshold = 0.01f;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        if (anim == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (anim == null) return;

        // Get horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(horizontalInput);
        
        // Set Speed parameter for blend tree (0 = idle, 1 = walk)
        anim.SetFloat("Speed", speed);
        
        // Set IsWalking bool parameter for transition
        bool isWalking = speed > walkThreshold;
        anim.SetBool("IsWalking", isWalking);
    }
}
