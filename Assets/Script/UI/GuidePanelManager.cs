using UnityEngine;

public class GuidePanelManager : MonoBehaviour
{
    [Header("Guide Panels")]
    [SerializeField] private GameObject[] guidePanels; // 8 panels
    
    [Header("UI Elements to Hide")]
    [SerializeField] private GameObject[] uiElementsToHide; // UI yang di-hide saat guide terbuka
    
    private int currentPanelIndex = 0;
    private bool isGuideOpen = false;

    void Start()
    {
        // Auto-find UI elements jika tidak di-assign di Inspector
        if (uiElementsToHide == null || uiElementsToHide.Length == 0)
        {
            AutoFindUIElements();
        }
        
        // Pastikan semua panels tidak aktif di awal
        HideAllPanels();
    }

    private void AutoFindUIElements()
    {
        var list = new System.Collections.Generic.List<GameObject>();
        
        // Find UI elements yang perlu di-hide
        AddIfFound(list, "DeathTimerUI");
        AddIfFound(list, "GuideButton");
        AddIfFound(list, "MusicButton");
        AddIfFound(list, "PauseButton");
        
        uiElementsToHide = list.ToArray();
        
        Debug.Log($"GuidePanelManager: Auto-found {uiElementsToHide.Length} UI elements to hide");
    }

    private void AddIfFound(System.Collections.Generic.List<GameObject> list, string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            list.Add(obj);
        }
    }

    /// <summary>
    /// Tampilkan guide panel pertama dan hide semua UI lain
    /// </summary>
    public void ShowGuide()
    {
        if (isGuideOpen) return;
        
        // Freeze gameplay
        Time.timeScale = 0f;
        
        // Stop timer
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopTimer();
        }
        
        // Hide other UI elements
        HideOtherUI();
        
        // Show panel pertama (index 0)
        currentPanelIndex = 0;
        ShowCurrentPanel();
        
        isGuideOpen = true;
        
        Debug.Log("Guide Panel opened");
    }

    /// <summary>
    /// Tutup guide panel dan restore semua UI
    /// </summary>
    public void CloseGuide()
    {
        if (!isGuideOpen) return;
        
        // Resume gameplay
        Time.timeScale = 1f;
        
        // Start timer
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartTimer();
        }
        
        // Hide all panels
        HideAllPanels();
        
        // Restore other UI elements
        ShowOtherUI();
        
        isGuideOpen = false;
        
        Debug.Log("Guide Panel closed");
    }

    /// <summary>
    /// Navigasi ke panel selanjutnya (circular: panel 8 -> panel 1)
    /// </summary>
    public void NextPanel()
    {
        if (!isGuideOpen || guidePanels == null || guidePanels.Length == 0) return;
        
        // Hide panel saat ini
        if (guidePanels[currentPanelIndex] != null)
        {
            guidePanels[currentPanelIndex].SetActive(false);
        }
        
        // Circular increment: 0->1->2->...->7->0
        currentPanelIndex = (currentPanelIndex + 1) % guidePanels.Length;
        
        // Show panel baru
        ShowCurrentPanel();
        
        Debug.Log($"Guide Panel: Next -> Page {currentPanelIndex + 1}");
    }

    /// <summary>
    /// Navigasi ke panel sebelumnya (circular: panel 1 -> panel 8)
    /// </summary>
    public void PreviousPanel()
    {
        if (!isGuideOpen || guidePanels == null || guidePanels.Length == 0) return;
        
        // Hide panel saat ini
        if (guidePanels[currentPanelIndex] != null)
        {
            guidePanels[currentPanelIndex].SetActive(false);
        }
        
        // Circular decrement: 7->6->5->...->0->7
        currentPanelIndex = (currentPanelIndex - 1 + guidePanels.Length) % guidePanels.Length;
        
        // Show panel baru
        ShowCurrentPanel();
        
        Debug.Log($"Guide Panel: Previous -> Page {currentPanelIndex + 1}");
    }

    private void ShowCurrentPanel()
    {
        if (guidePanels != null && currentPanelIndex >= 0 && currentPanelIndex < guidePanels.Length)
        {
            if (guidePanels[currentPanelIndex] != null)
            {
                guidePanels[currentPanelIndex].SetActive(true);
            }
        }
    }

    private void HideAllPanels()
    {
        if (guidePanels == null) return;
        
        foreach (GameObject panel in guidePanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    private void HideOtherUI()
    {
        if (uiElementsToHide == null) return;
        
        foreach (GameObject ui in uiElementsToHide)
        {
            if (ui != null && ui.activeSelf)
            {
                ui.SetActive(false);
            }
        }
    }

    private void ShowOtherUI()
    {
        if (uiElementsToHide == null) return;
        
        foreach (GameObject ui in uiElementsToHide)
        {
            if (ui != null && !ui.activeSelf)
            {
                ui.SetActive(true);
            }
        }
    }

    // Utility untuk debugging
    private void OnValidate()
    {
        // Cek di Editor apakah semua panels sudah di-assign
        if (guidePanels != null && guidePanels.Length != 8)
        {
            Debug.LogWarning($"GuidePanelManager: Expected 8 panels, but found {guidePanels.Length}");
        }
    }
}
