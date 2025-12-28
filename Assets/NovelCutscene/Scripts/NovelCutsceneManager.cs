using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NOVEL CUTSCENE MANAGER - Sistem Visual Novel Cutscene
/// 
/// Fungsi:
/// - Mengelola alur cutscene visual novel dengan typing effect
/// - Pause game otomatis (Time.timeScale = 0)
/// - Fade in/out transition
/// - Integrasi dengan AudioManager untuk BGM & SFX
/// - Input handling untuk next dialog / skip typing
/// 
/// Cara Pakai:
/// - Attach ke GameObject "NovelCutsceneManager" atau ke Canvas cutscene
/// - Assign UI references di Inspector
/// - Panggil: NovelCutsceneManager.Instance.PlayCutscene(dialogueData)
/// </summary>
public class NovelCutsceneManager : MonoBehaviour
{
    // =====================================================
    // SINGLETON PATTERN
    // =====================================================
    public static NovelCutsceneManager Instance { get; private set; }

    // =====================================================
    // UI REFERENCES (Assign di Inspector)
    // =====================================================
    [Header("=== UI REFERENCES ===")]
    [Tooltip("Canvas utama cutscene (NovelCutsceneCanvas)")]
    [SerializeField] private GameObject cutsceneCanvas;

    [Tooltip("FadeOverlay - Image hitam dengan CanvasGroup untuk fade")]
    [SerializeField] private CanvasGroup fadeOverlay;

    [Tooltip("CutsceneBackground - Image untuk background sprite")]
    [SerializeField] private Image backgroundImage;

    [Tooltip("DialogueText - TextMeshProUGUI untuk teks dialog")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Tooltip("CharacterNameText - TextMeshProUGUI untuk nama karakter")]
    [SerializeField] private TextMeshProUGUI characterNameText;

    [Tooltip("ContinueIndicator - GameObject untuk icon '►' atau 'Click...'")]
    [SerializeField] private GameObject continueIndicator;

    // =====================================================
    // AUDIO SETTINGS
    // =====================================================
    [Header("=== AUDIO SETTINGS ===")]
    [Tooltip("Volume BGM cutscene (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float cutsceeneBGMVolume = 0.4f;

    [Tooltip("Volume SFX typing (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float typingSFXVolume = 0.3f;

    // =====================================================
    // FADE SETTINGS
    // =====================================================
    [Header("=== FADE SETTINGS ===")]
    [Tooltip("Durasi fade in/out (detik)")]
    [Range(0.5f, 3f)]
    [SerializeField] private float fadeDuration = 1f;

    // =====================================================
    // STATE MANAGEMENT
    // =====================================================
    private bool isCutscenePlaying = false;
    private bool isTyping = false;
    private bool canProceed = false;
    private bool inputReceived = false; // Flag untuk mencegah double input
    private float lastInputTime = 0f; // Waktu input terakhir untuk cooldown
    private const float INPUT_COOLDOWN = 0.25f; // Cooldown 0.25 detik antar input
    
    private bool isCutsceeneBGMPlaying = false; // Flag untuk track apakah BGM cutscene sudah playing

    private DialogueSceneData currentSceneData;
    private int currentDialogueIndex = 0;

    private Coroutine typingCoroutine;
    private Coroutine indicatorCoroutine;

    // =====================================================
    // UNITY LIFECYCLE: AWAKE
    // =====================================================
    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: DontDestroyOnLoad jika cutscene perlu persist antar scene
        // DontDestroyOnLoad(gameObject);

        // Hide canvas di awal
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }

        Debug.Log("[NovelCutsceneManager] Initialized successfully!");
    }

    // =====================================================
    // UNITY LIFECYCLE: UPDATE
    // =====================================================
    void Update()
    {
        // Hanya handle input saat cutscene playing
        if (!isCutscenePlaying) return;

        // Input: Mouse click atau Space untuk next/skip
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            HandleInput();
        }
    }

    // =====================================================
    // MAIN FUNCTION: PLAY CUTSCENE
    // =====================================================
    /// <summary>
    /// Memulai cutscene dengan DialogueSceneData yang diberikan
    /// </summary>
    /// <param name="data">Data cutscene (background, dialog, dll)</param>
    public void PlayCutscene(DialogueSceneData data)
    {
        // Validasi
        if (data == null)
        {
            Debug.LogError("[NovelCutsceneManager] DialogueSceneData is null!");
            return;
        }

        if (isCutscenePlaying)
        {
            Debug.LogWarning("[NovelCutsceneManager] Cutscene already playing!");
            return;
        }

        // PENTING: Aktifkan canvas DULU sebelum StartCoroutine
        // Karena NovelCutsceneManager adalah child dari cutsceneCanvas,
        // jika canvas inactive maka GameObject ini juga inactive → StartCoroutine gagal!
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(true);
        }

        // Set state
        isCutscenePlaying = true;
        currentSceneData = data;
        currentDialogueIndex = 0;

        // Start cutscene sequence
        StartCoroutine(CutsceneSequence());
    }

    // =====================================================
    // COROUTINE: CUTSCENE SEQUENCE
    // =====================================================
    /// <summary>
    /// Alur utama cutscene: Pause game → Fade in → Dialog loop → Fade out → Resume
    /// </summary>
    private IEnumerator CutsceneSequence()
    {
        Debug.Log("[NovelCutsceneManager] Starting cutscene sequence...");

        // 1. Pause game
        Time.timeScale = 0f;

        // 2. Stop level BGM dan play cutscene BGM (HANYA JIKA BELUM PLAYING)
        if (AudioManager.Instance != null && !isCutsceeneBGMPlaying)
        {
            AudioManager.Instance.StopBGM();

            if (currentSceneData.cutsceeneBGM != null)
            {
                // Play cutscene BGM via SFX source (karena tidak terpengaruh pause)
                AudioManager.Instance.PlaySFX(currentSceneData.cutsceeneBGM, cutsceeneBGMVolume);
                isCutsceeneBGMPlaying = true; // Set flag agar tidak play ulang di scene berikutnya
                Debug.Log("[NovelCutsceneManager] Cutscene BGM started");
            }
        }
        else if (isCutsceeneBGMPlaying)
        {
            Debug.Log("[NovelCutsceneManager] Cutscene BGM already playing, skip restart");
        }

        // 3. Set background sprite
        if (backgroundImage != null && currentSceneData.backgroundSprite != null)
        {
            backgroundImage.sprite = currentSceneData.backgroundSprite;
        }

        // 4. Fade in
        yield return StartCoroutine(FadeIn());

        // 5. Loop semua dialog di scene ini
        for (int i = 0; i < currentSceneData.dialogues.Length; i++)
        {
            currentDialogueIndex = i;
            yield return StartCoroutine(ShowDialogue(currentSceneData.dialogues[i]));
        }

        // 6. Fade out
        yield return StartCoroutine(FadeOut());

        // 7. Resume gameplay
        ResumeGameplay();

        Debug.Log("[NovelCutsceneManager] Cutscene sequence complete!");
    }

    // =====================================================
    // COROUTINE: SHOW DIALOGUE
    // =====================================================
    /// <summary>
    /// Tampilkan 1 dialog dengan typing effect dan tunggu input player
    /// </summary>
    private IEnumerator ShowDialogue(string dialogue)
    {
        // Reset state
        canProceed = false;
        isTyping = true;
        inputReceived = false;

        // Set character name
        if (characterNameText != null)
        {
            characterNameText.text = currentSceneData.characterName;
        }

        // Hide continue indicator
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }

        // Start typing effect
        typingCoroutine = StartCoroutine(TypeDialogue(dialogue));

        // Wait hingga typing selesai ATAU di-skip
        while (isTyping)
        {
            yield return null;
        }

        // Typing selesai (normal atau di-skip)
        canProceed = true;

        // Show continue indicator dengan animasi
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(true);
            indicatorCoroutine = StartCoroutine(AnimateContinueIndicator());
        }

        // Tunggu input player untuk lanjut (cek flag, bukan Input langsung)
        while (!inputReceived)
        {
            yield return null;
        }

        // Stop indicator animation
        if (indicatorCoroutine != null)
        {
            StopCoroutine(indicatorCoroutine);
        }
    }

    // =====================================================
    // COROUTINE: TYPE DIALOGUE
    // =====================================================
    /// <summary>
    /// Typing effect - karakter muncul satu per satu
    /// </summary>
    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";

        // Play typing SFX HANYA SEKALI di awal
        if (currentSceneData.typingSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(currentSceneData.typingSFX, typingSFXVolume);
        }

        foreach (char c in dialogue)
        {
            dialogueText.text += c;

            // Wait dengan WaitForSecondsRealtime (tidak terpengaruh Time.timeScale)
            yield return new WaitForSecondsRealtime(currentSceneData.typingSpeed);
        }
    }

    // =====================================================
    // INPUT HANDLER
    // =====================================================
    /// <summary>
    /// Handle input player: Skip typing atau next dialog
    /// </summary>
    private void HandleInput()
    {
        // Cooldown check - cegah spam klik
        if (Time.realtimeSinceStartup - lastInputTime < INPUT_COOLDOWN)
        {
            return; // Masih dalam cooldown, abaikan input
        }

        if (isTyping)
        {
            // Skip typing - langsung tampilkan full text
            if (typingCoroutine != null)
            {                StopCoroutine(typingCoroutine);
            }

            // Set full text langsung
            if (currentDialogueIndex < currentSceneData.dialogues.Length)
            {
                dialogueText.text = currentSceneData.dialogues[currentDialogueIndex];
            }

            isTyping = false; // PENTING: Set false agar ShowDialogue while loop berhenti
            canProceed = true;

            // Show continue indicator
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(true);
                if (indicatorCoroutine == null)
                {
                    indicatorCoroutine = StartCoroutine(AnimateContinueIndicator());
                }
            }

            // Play SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonSFX(false);
            }

            // Update last input time
            lastInputTime = Time.realtimeSinceStartup;
        }
        else if (canProceed && !inputReceived)
        {
            // Next dialog - set flag untuk ShowDialogue coroutine
            inputReceived = true;
            
            // Play SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonSFX(false);
            }

            // Update last input time
            lastInputTime = Time.realtimeSinceStartup;
        }
    }

    // =====================================================
    // COROUTINE: ANIMATE CONTINUE INDICATOR
    // =====================================================
    /// <summary>
    /// Animasi fade in-out untuk continue indicator (► atau "Click...")
    /// </summary>
    private IEnumerator AnimateContinueIndicator()
    {
        if (continueIndicator == null) yield break;

        CanvasGroup indicatorGroup = continueIndicator.GetComponent<CanvasGroup>();
        if (indicatorGroup == null)
        {
            indicatorGroup = continueIndicator.AddComponent<CanvasGroup>();
        }

        // Loop fade in-out
        while (true)
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.unscaledDeltaTime;
                indicatorGroup.alpha = Mathf.Lerp(0.3f, 1f, elapsed / 0.5f);
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.unscaledDeltaTime;
                indicatorGroup.alpha = Mathf.Lerp(1f, 0.3f, elapsed / 0.5f);
                yield return null;
            }
        }
    }

    // =====================================================
    // COROUTINE: FADE IN
    // =====================================================
    /// <summary>
    /// Fade in - dari hitam penuh ke transparan (reveal background)
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (fadeOverlay == null) yield break;

        float elapsed = 0f;
        fadeOverlay.alpha = 1f; // Mulai dari hitam penuh

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeOverlay.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = 0f;
    }

    // =====================================================
    // COROUTINE: FADE OUT
    // =====================================================
    /// <summary>
    /// Fade out - dari transparan ke hitam penuh
    /// </summary>
    private IEnumerator FadeOut()
    {
        if (fadeOverlay == null) yield break;

        float elapsed = 0f;
        fadeOverlay.alpha = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeOverlay.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = 1f;
    }

    // =====================================================
    // RESUME GAMEPLAY
    // =====================================================
    /// <summary>
    /// Resume gameplay setelah cutscene selesai
    /// </summary>
    private void ResumeGameplay()
    {
        // Resume time
        Time.timeScale = 1f;

        // STOP semua audio cutscene (BGM dan SFX) agar tidak double
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSFX();
            
            // Resume level BGM
            AudioManager.Instance.PlayLevelMusic();
        }
        
        // Reset flag agar cutscene berikutnya bisa play BGM lagi dari awal
        isCutsceeneBGMPlaying = false;
        Debug.Log("[NovelCutsceneManager] Cutscene BGM flag reset");

        // Hide canvas
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }

        // Clear UI state
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (characterNameText != null)
        {
            characterNameText.text = "";
        }

        if (backgroundImage != null)
        {
            backgroundImage.sprite = null;
        }

        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }

        // Reset state
        isCutscenePlaying = false;
        currentSceneData = null;
        currentDialogueIndex = 0;

        Debug.Log("[NovelCutsceneManager] Gameplay resumed!");
    }

    // =====================================================
    // PUBLIC PROPERTY
    // =====================================================
    /// <summary>
    /// Cek apakah cutscene sedang playing
    /// </summary>
    public bool IsCutscenePlaying => isCutscenePlaying;
}
