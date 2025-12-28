using UnityEngine;
using TMPro;

/// <summary>
/// DEATH TIMER UI - Display In-Game
/// 
/// Fungsi:
/// - Menampilkan timer level (format MM:SS:MS)
/// - Menampilkan jumlah kematian player
/// - Update real-time setiap frame
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject di Canvas (misalnya "DeathTimerUI")
/// 2. Assign 2 TextMeshProUGUI di Inspector:
///    - deathCountText (untuk deaths)
///    - timerText (untuk timer)
/// 3. Script akan otomatis update UI setiap frame
/// </summary>
public class DeathTimerUI : MonoBehaviour
{
    // =====================================================
    // SERIALIZED FIELDS
    // =====================================================
    [Header("=== UI REFERENCES ===")]
    [Tooltip("TextMeshProUGUI untuk menampilkan jumlah kematian")]
    [SerializeField] private TextMeshProUGUI deathCountText;

    [Tooltip("TextMeshProUGUI untuk menampilkan timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("=== FORMAT SETTINGS ===")]
    [Tooltip("Format teks untuk death count (contoh: 'Deaths: {0}')")]
    [SerializeField] private string deathFormat = "Deaths: {0}";

    [Tooltip("Format warna teks")]
    [SerializeField] private Color textColor = Color.white;

    [Header("=== DEBUG ===")]
    [Tooltip("Show debug warnings")]
    [SerializeField] private bool showDebugWarnings = true;

    // =====================================================
    // PRIVATE FIELDS
    // =====================================================
    private bool hasWarnedAboutGameManager = false;

    // =====================================================
    // UNITY LIFECYCLE: START
    // =====================================================
    void Start()
    {
        // Setup text color
        if (deathCountText != null)
        {
            deathCountText.color = textColor;
        }

        if (timerText != null)
        {
            timerText.color = textColor;
        }

        // Validasi references
        if (deathCountText == null)
        {
            Debug.LogError("[DeathTimerUI] deathCountText belum di-assign di Inspector!");
        }

        if (timerText == null)
        {
            Debug.LogError("[DeathTimerUI] timerText belum di-assign di Inspector!");
        }
    }

    // =====================================================
    // UNITY LIFECYCLE: UPDATE
    // =====================================================
    void Update()
    {
        UpdateUI();
    }

    // =====================================================
    // UPDATE UI
    // =====================================================
    /// <summary>
    /// Update UI dengan data terbaru dari GameManager
    /// </summary>
    private void UpdateUI()
    {
        // Cek apakah GameManager ada
        if (GameManager.Instance == null)
        {
            if (showDebugWarnings && !hasWarnedAboutGameManager)
            {
                Debug.LogWarning("[DeathTimerUI] GameManager.Instance belum ada! Pastikan GameManager sudah dibuat di MainMenu scene.");
                hasWarnedAboutGameManager = true;
            }
            return;
        }

        // Update death count
        if (deathCountText != null)
        {
            int deaths = GameManager.Instance.deathCount;
            deathCountText.text = string.Format(deathFormat, deaths);
        }

        // Update timer
        if (timerText != null)
        {
            float time = GameManager.Instance.levelTimer;
            timerText.text = FormatTime(time);
        }
    }

    // =====================================================
    // TIME FORMATTING
    // =====================================================
    /// <summary>
    /// Format waktu ke MM:SS:MS
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
