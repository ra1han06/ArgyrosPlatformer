using UnityEngine;

/// <summary>
/// VASE TUTORIAL INTERACT - Interactive Vase untuk Tutorial Scene
/// 
/// Fungsi:
/// - Detect player proximity (trigger area)
/// - Tampilkan world-space UI prompt "Press E"
/// - Saat E ditekan, aktifkan Tutorial Canvas
/// - Tidak pause game, player tetap bisa bergerak
/// - Bisa diinteraksi berkali-kali
/// 
/// Setup:
/// 1. Attach script ini ke Vase GameObject
/// 2. Vase harus punya Collider dengan isTrigger = true
/// 3. Assign Tutorial Canvas di Inspector (GameObject dengan Canvas component)
/// 4. Assign Interaction Prompt (world-space UI child object)
/// 
/// Canvas Tutorial:
/// - Setiap vase bisa punya canvas tutorial berbeda
/// - Canvas default inactive, script akan SetActive(true) saat E ditekan
/// - Untuk menutup canvas, gunakan tombol Close di canvas itu sendiri
/// </summary>
public class VaseTutorialInteract : MonoBehaviour
{
    // =====================================================
    // SERIALIZED FIELDS
    // =====================================================
    
    [Header("Interaction Settings")]
    [Tooltip("Tombol untuk interact (default: E)")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Tooltip("Tag untuk player (default: Player)")]
    [SerializeField] private string playerTag = "Player";
    
    [Tooltip("Radius area interaksi (untuk trigger collider)")]
    [SerializeField] private float interactionRadius = 2f;
    
    [Header("Tutorial Canvas")]
    [Tooltip("Canvas Tutorial yang akan ditampilkan saat E ditekan")]
    [SerializeField] private GameObject tutorialCanvas;
    
    [Header("UI Prompt")]
    [Tooltip("World-space UI prompt 'Press E' (child object di vase)")]
    [SerializeField] private GameObject interactionPrompt;
    
    // =====================================================
    // PRIVATE FIELDS
    // =====================================================
    
    private bool playerInRange = false;
    private BoxCollider triggerCollider;
    
    // =====================================================
    // UNITY LIFECYCLE
    // =====================================================
    
    void Start()
    {
        // Setup trigger collider
        SetupTriggerCollider();
        
        // Pastikan interaction prompt hidden di awal
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Pastikan tutorial canvas hidden di awal
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[VaseTutorialInteract] Tutorial Canvas belum di-assign di {gameObject.name}!");
        }
    }
    
    void Update()
    {
        // Cek input E saat player dalam range
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            ToggleTutorial();
        }
        
        // Tutup tutorial dengan Escape key (dari mana saja)
        if (tutorialCanvas != null && tutorialCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            tutorialCanvas.SetActive(false);
            Debug.Log($"[VaseTutorialInteract] Tutorial ditutup dengan Escape.");
        }
    }
    
    // =====================================================
    // TRIGGER DETECTION
    // =====================================================
    
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah yang masuk adalah player
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            
            // Tampilkan interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
            
            Debug.Log($"[VaseTutorialInteract] Player masuk area {gameObject.name}. Press E untuk tutorial!");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Cek apakah yang keluar adalah player
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            
            // Sembunyikan interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            
            Debug.Log($"[VaseTutorialInteract] Player keluar dari area {gameObject.name}.");
        }
    }
    
    // =====================================================
    // TUTORIAL DISPLAY
    // =====================================================
    
    /// <summary>
    /// Toggle tutorial canvas saat E ditekan (buka/tutup)
    /// Canvas muncul instant (SetActive true/false)
    /// Game tidak pause, player tetap bisa bergerak
    /// </summary>
    private void ToggleTutorial()
    {
        if (tutorialCanvas != null)
        {
            // Toggle canvas (buka jika tutup, tutup jika buka)
            bool isActive = tutorialCanvas.activeSelf;
            tutorialCanvas.SetActive(!isActive);
            
            if (!isActive)
            {
                Debug.Log($"[VaseTutorialInteract] Tutorial dari {gameObject.name} ditampilkan! Tekan E lagi atau Escape untuk menutup.");
            }
            else
            {
                Debug.Log($"[VaseTutorialInteract] Tutorial dari {gameObject.name} ditutup.");
            }
        }
        else
        {
            Debug.LogWarning($"[VaseTutorialInteract] Tutorial Canvas tidak di-assign di {gameObject.name}!");
        }
    }
    
    // =====================================================
    // COLLIDER SETUP
    // =====================================================
    
    /// <summary>
    /// Setup atau cari trigger collider di vase
    /// Jika belum ada, buat collider baru dengan isTrigger = true
    /// </summary>
    private void SetupTriggerCollider()
    {
        // Cari box collider yang sudah ada
        BoxCollider[] colliders = GetComponents<BoxCollider>();
        
        // Cari trigger collider
        foreach (BoxCollider col in colliders)
        {
            if (col.isTrigger)
            {
                triggerCollider = col;
                break;
            }
        }
        
        // Kalau tidak ada trigger collider, buat baru
        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            Debug.Log($"[VaseTutorialInteract] Trigger collider otomatis dibuat untuk {gameObject.name}");
        }
        
        // Update ukuran trigger berdasarkan interaction radius
        UpdateTriggerSize();
    }
    
    /// <summary>
    /// Update ukuran trigger collider berdasarkan interaction radius
    /// </summary>
    private void UpdateTriggerSize()
    {
        if (triggerCollider != null)
        {
            // Ukuran default
            Vector3 baseSize = Vector3.one;
            
            // Coba ambil ukuran dari non-trigger collider (collider asli vase)
            BoxCollider[] colliders = GetComponents<BoxCollider>();
            foreach (BoxCollider col in colliders)
            {
                if (!col.isTrigger)
                {
                    baseSize = col.size;
                    break;
                }
            }
            
            // Perbesar trigger berdasarkan interaction radius
            triggerCollider.size = baseSize + Vector3.one * (interactionRadius * 0.5f);
        }
    }
    
    // =====================================================
    // EDITOR GIZMOS (untuk visualisasi area trigger)
    // =====================================================
    
    private void OnDrawGizmosSelected()
    {
        // Visualisasi trigger area di Unity Editor
        Gizmos.color = new Color(0, 0.5f, 1f, 0.3f); // Biru transparan
        
        // Cari trigger collider
        BoxCollider[] colliders = GetComponents<BoxCollider>();
        BoxCollider trigger = null;
        foreach (BoxCollider col in colliders)
        {
            if (col.isTrigger)
            {
                trigger = col;
                break;
            }
        }
        
        if (trigger != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(trigger.center, trigger.size);
            
            // Draw wireframe untuk interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(trigger.center, trigger.size);
        }
    }
    
    /// <summary>
    /// Update trigger size ketika interaction radius berubah di Inspector
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying && triggerCollider != null)
        {
            UpdateTriggerSize();
        }
    }
}
