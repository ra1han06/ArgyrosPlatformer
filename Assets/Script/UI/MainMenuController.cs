using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MAIN MENU CONTROLLER - Control Main Menu UI State
/// 
/// Fungsi:
/// - Control visibility/state Continue button berdasarkan save file
/// - Disable Continue button jika tidak ada save data
/// - Enable Continue button jika ada save data
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject di MainMenu scene (misal: "MainMenuController")
/// 2. Assign Continue Button di Inspector
/// 3. Script akan otomatis disable/enable button di Start()
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // =====================================================
    // SERIALIZED FIELDS
    // =====================================================
    
    [Header("=== UI REFERENCES ===")]
    [Tooltip("Reference ke Continue Button di MainMenu")]
    [SerializeField] private Button continueButton;

    [Header("=== SETTINGS ===")]
    [Tooltip("Jika true, button akan di-hide (SetActive false). Jika false, hanya disable (interactable false)")]
    [SerializeField] private bool hideInsteadOfDisable = false;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug log messages")]
    [SerializeField] private bool showDebugLog = true;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    
    void Start()
    {
        // Setup Continue button state
        UpdateContinueButtonState();
    }

    // =====================================================
    // UPDATE CONTINUE BUTTON STATE
    // =====================================================
    
    /// <summary>
    /// Update Continue button state berdasarkan save file
    /// </summary>
    private void UpdateContinueButtonState()
    {
        // Validasi reference
        if (continueButton == null)
        {
            Debug.LogError("[MainMenuController] Continue Button belum di-assign di Inspector!");
            return;
        }

        // Cek apakah ada save file
        bool hasSaveFile = SaveSystem.HasSaveFile();

        if (hasSaveFile)
        {
            // Ada save file - enable Continue button
            EnableContinueButton();
        }
        else
        {
            // Tidak ada save file - disable Continue button
            DisableContinueButton();
        }
    }

    // =====================================================
    // ENABLE CONTINUE BUTTON
    // =====================================================
    
    /// <summary>
    /// Enable Continue button
    /// </summary>
    private void EnableContinueButton()
    {
        if (hideInsteadOfDisable)
        {
            // Show button
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            // Enable button
            continueButton.interactable = true;
        }

        if (showDebugLog)
            Debug.Log("[MainMenuController] ✅ Continue Button ENABLED (Save file found)");
    }

    // =====================================================
    // DISABLE CONTINUE BUTTON
    // =====================================================
    
    /// <summary>
    /// Disable Continue button
    /// </summary>
    private void DisableContinueButton()
    {
        if (hideInsteadOfDisable)
        {
            // Hide button
            continueButton.gameObject.SetActive(false);
        }
        else
        {
            // Disable button (gray out)
            continueButton.interactable = false;
        }

        if (showDebugLog)
            Debug.Log("[MainMenuController] ❌ Continue Button DISABLED (No save file)");
    }

    // =====================================================
    // PUBLIC METHODS (untuk debugging/testing)
    // =====================================================
    
    /// <summary>
    /// Force update Continue button state (untuk debugging)
    /// Bisa dipanggil dari Inspector atau script lain
    /// </summary>
    public void RefreshContinueButtonState()
    {
        UpdateContinueButtonState();
    }

    /// <summary>
    /// Delete save dan update UI (untuk New Game button)
    /// CATATAN: Method ini hanya update UI state Continue button
    /// Actual scene loading di-handle oleh SceneController.OnNewGameButtonClicked()
    /// </summary>
    public void OnNewGame()
    {
        if (showDebugLog)
            Debug.Log("[MainMenuController] New Game → Total Reset (called from this script)");

        // TOTAL RESET - Hapus SEMUA data
        SaveSystem.DeleteAllData();

        // Buat save baru dengan default values
        GameSaveData newSave = new GameSaveData();
        SaveSystem.SaveGame(newSave);

        // Update Continue button state (sekarang harusnya ENABLED karena ada save baru)
        UpdateContinueButtonState();

        if (showDebugLog)
            Debug.Log("[MainMenuController] ✅ Fresh save created. Continue button enabled.");
        
        // NOTE: Scene loading ke SelectLevel di-handle oleh SceneController
        // Pastikan New Game button di MainMenu connected ke SceneController.OnNewGameButtonClicked()
    }
}
