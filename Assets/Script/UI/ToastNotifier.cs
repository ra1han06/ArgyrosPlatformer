using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Simple in-game toast notification.
/// Attach this to a GameObject that has a TextMeshProUGUI assigned.
/// </summary>
public class ToastNotifier : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Behavior")]
    [SerializeField] private float showDuration = 1.6f;
    [SerializeField] private float fadeDuration = 0.25f;

    private Coroutine currentRoutine;

    private void Reset()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>(true);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        HideImmediate();
    }

    public void Show(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        if (text != null)
            text.text = message;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    public void HideImmediate()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator ShowRoutine()
    {
        yield return FadeTo(1f);
        yield return new WaitForSeconds(showDuration);
        yield return FadeTo(0f);
        currentRoutine = null;
    }

    private IEnumerator FadeTo(float target)
    {
        if (canvasGroup == null)
            yield break;

        float start = canvasGroup.alpha;
        float t = 0f;

        canvasGroup.interactable = target > 0.99f;
        canvasGroup.blocksRaycasts = false;

        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = target;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(start, target, k);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}
