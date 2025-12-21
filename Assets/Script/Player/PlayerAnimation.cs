using UnityEngine;

/// <summary>
/// Controls player animations based on CharacterController state
/// For COMPLETE BEGINNERS - Every line is explained
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    // ========== REFERENCES ==========
    private Animator animator;                    // Komponen Animator (otak animasi)
    private CharacterController characterController; // Untuk cek IsGrounded
    
    // ========== GROUND CHECK SETTINGS ==========
    [Header("Ground Check")]
    [Tooltip("Transform kosong di bawah kaki player")]
    [SerializeField] private Transform groundCheck;
    
    [Tooltip("Jarak check ke tanah (0.1-0.3 biasanya cukup)")]
    [SerializeField] private float groundDistance = 0.2f;
    
    [Tooltip("Layer apa saja yang dianggap tanah")]
    [SerializeField] private LayerMask groundMask;
    
    // ========== ANIMATION SETTINGS ==========
    [Header("Animation Thresholds")]
    [Tooltip("Kecepatan minimum untuk dianggap 'walking' (0.01-0.1)")]
    [SerializeField] private float walkThreshold = 0.1f;
    
    // ========== PRIVATE VARIABLES ==========
    private bool isGrounded;      // Apakah player di tanah?
    private float verticalVelocity; // Kecepatan Y (untuk jump/fall detection)
    private Vector3 lastPosition;   // Posisi frame sebelumnya (untuk hitung velocity)

    // ========== INITIALIZATION ==========
    void Start()
    {
        // Ambil komponen Animator dari GameObject ini
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("❌ ANIMATOR NOT FOUND! Tambahkan Animator component ke " + gameObject.name);
        }
        
        // Ambil CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("❌ CHARACTER CONTROLLER NOT FOUND!");
        }
        
        // Simpan posisi awal
        lastPosition = transform.position;
    }

    // ========== UPDATE EVERY FRAME ==========
    void Update()
    {
        // Jika ada komponen yang hilang, stop
        if (animator == null || characterController == null) return;
        
        // 1. CEK APAKAH DI TANAH (Ground Check)
        CheckGrounded();
        
        // 2. HITUNG VERTICAL VELOCITY (untuk Jump/Fall)
        CalculateVerticalVelocity();
        
        // 3. HITUNG HORIZONTAL SPEED (untuk Idle/Walk)
        CalculateHorizontalSpeed();
        
        // 4. UPDATE ANIMATOR PARAMETERS
        UpdateAnimatorParameters();
        
        // 5. FLIP CHARACTER (Hadap kiri/kanan)
        FlipCharacter();
        
        // Simpan posisi untuk frame berikutnya
        lastPosition = transform.position;
    }

    // ========== 1. GROUND CHECK ==========
    void CheckGrounded()
    {
        // Jika GroundCheck transform belum di-assign, pakai posisi player
        Vector3 checkPosition = groundCheck != null ? groundCheck.position : transform.position;
        
        // Physics.CheckSphere = cek apakah ada collider dalam radius tertentu
        // Ini lebih reliable daripada characterController.isGrounded
        isGrounded = Physics.CheckSphere(checkPosition, groundDistance, groundMask);
        
        // Debug: Lihat di Console
        // Debug.Log("IsGrounded: " + isGrounded);
    }

    // ========== 2. CALCULATE VERTICAL VELOCITY ==========
    void CalculateVerticalVelocity()
    {
        // Hitung selisih posisi Y antara frame ini dan frame sebelumnya
        float deltaY = transform.position.y - lastPosition.y;
        
        // Bagi dengan Time.deltaTime untuk dapat velocity per detik
        verticalVelocity = deltaY / Time.deltaTime;
        
        // CharacterController.velocity juga bisa dipakai, tapi kadang kurang akurat
        // Alternatif: verticalVelocity = characterController.velocity.y;
    }

    // ========== 3. CALCULATE HORIZONTAL SPEED ==========
    void CalculateHorizontalSpeed()
    {
        // Ambil input horizontal (-1 kiri, 0 diam, +1 kanan)
        float horizontalInput = Input.GetAxis("Horizontal");
        
        // Ambil nilai absolut (0 sampai 1)
        float speed = Mathf.Abs(horizontalInput);
        
        // Set parameter Speed di Animator
        animator.SetFloat("Speed", speed);
    }

    // ========== 4. UPDATE ANIMATOR PARAMETERS ==========
    void UpdateAnimatorParameters()
    {
        // Parameter: IsGrounded (bool)
        animator.SetBool("IsGrounded", isGrounded);
        
        // Parameter: YVelocity (float)
        animator.SetFloat("YVelocity", verticalVelocity);
        
        // Parameter Speed sudah di-set di CalculateHorizontalSpeed()
    }

    // ========== 5. FLIP CHARACTER BASED ON INPUT ==========
    void FlipCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        
        // Jika bergerak ke kanan (input > 0)
        if (horizontalInput > 0.01f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Hadap kanan (sesuaikan dengan setup Anda)
        }
        // Jika bergerak ke kiri (input < 0)
        else if (horizontalInput < -0.01f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Hadap kiri
        }
        
        // Note: Nilai 90/-90 mungkin perlu disesuaikan tergantung orientasi model Anda
        // Coba nilai: 0/180, 90/-90, atau 90/270 sampai menghadap dengan benar
    }

    // ========== VISUALISASI GROUND CHECK (EDITOR ONLY) ==========
    void OnDrawGizmosSelected()
    {
        // Gambar sphere di Scene view untuk visualisasi GroundCheck
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
