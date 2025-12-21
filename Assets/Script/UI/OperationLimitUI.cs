using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OperationLimitUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerPlatformInteractor playerInteractor;

    [Header("UI Text Elements")]
    [SerializeField] private TextMeshProUGUI copyText;
    [SerializeField] private TextMeshProUGUI cutText;
    [SerializeField] private TextMeshProUGUI pasteText;

    [Header("UI Settings")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color usedColor = Color.gray;
    [SerializeField] private Color limitReachedColor = Color.red;

    void Start()
    {
        if (playerInteractor == null)
        {
            playerInteractor = FindFirstObjectByType<PlayerPlatformInteractor>();
        }
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerInteractor == null) return;

        var usage = playerInteractor.GetCurrentUsage();
        var limits = playerInteractor.GetMaxLimits();
        var remaining = playerInteractor.GetRemainingOperations();

        // Update Copy
        UpdateTextElement(copyText, "Copy", remaining.copy, usage.copy, limits.copy);

        // Update Cut
        UpdateTextElement(cutText, "Cut", remaining.cut, usage.cut, limits.cut);

        // Update Paste
        UpdateTextElement(pasteText, "Paste", remaining.paste, usage.paste, limits.paste);
    }

    private void UpdateTextElement(TextMeshProUGUI textElement, string operationName, int remaining, int used, int max)
    {
        if (textElement == null) return;

        // Set text without icon
        textElement.text = $"{operationName}: {used}/{max}";

        // Set color based on remaining operations
        if (remaining == 0)
            textElement.color = limitReachedColor;
        else if (used > 0)
            textElement.color = usedColor;
        else
            textElement.color = availableColor;
    }

}