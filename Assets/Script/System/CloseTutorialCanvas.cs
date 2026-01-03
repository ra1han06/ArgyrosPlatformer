using UnityEngine;

/// <summary>
/// CLOSE TUTORIAL CANVAS - Script untuk tombol Close di Tutorial Canvas
/// 
/// Fungsi:
/// - Menutup Tutorial Canvas saat tombol diklik
/// - Attach script ini ke tombol Close di dalam Canvas Tutorial
/// 
/// Cara Pakai:
/// 1. Attach script ini ke Button GameObject di Canvas Tutorial
/// 2. Canvas akan di-detect otomatis (parent dari button)
/// 3. Atau assign manual via Inspector
/// 
/// Catatan:
/// - Canvas akan di-SetActive(false) saat tombol diklik
/// - Tidak pause/unpause game karena memang tidak ada pause
/// </summary>
public class CloseTutorialCanvas : MonoBehaviour
{
    [Header("Canvas Reference")]
    [Tooltip("Canvas Tutorial yang akan ditutup. Jika kosong, akan auto-detect dari parent.")]
    [SerializeField] private GameObject tutorialCanvas;
    
    void Start()
    {
        // Auto-detect canvas jika belum di-assign
        if (tutorialCanvas == null)
        {
            // Cari Canvas component di parent hierarchy
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                tutorialCanvas = canvas.gameObject;
                Debug.Log($"[CloseTutorialCanvas] Auto-detected canvas: {tutorialCanvas.name}");
            }
            else
            {
                Debug.LogWarning("[CloseTutorialCanvas] Canvas tidak ditemukan! Assign manual via Inspector.");
            }
        }
    }
    
    /// <summary>
    /// Method ini dipanggil dari Button OnClick event
    /// Menutup Tutorial Canvas dengan SetActive(false)
    /// </summary>
    public void CloseCanvas()
    {
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(false);
            Debug.Log($"[CloseTutorialCanvas] Tutorial Canvas '{tutorialCanvas.name}' ditutup.");
        }
        else
        {
            Debug.LogWarning("[CloseTutorialCanvas] Tutorial Canvas belum di-assign!");
        }
    }
}
