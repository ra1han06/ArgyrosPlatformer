using System.Collections;
using UnityEngine;

/// <summary>
/// CUTSCENE TRIGGER - Script untuk Men-trigger Cutscene di Awal Level
/// 
/// Fungsi:
/// - Membaca flag dari PlayerPrefs untuk cek apakah cutscene harus dimainkan
/// - Men-trigger NovelCutsceneManager untuk play cutscene
/// - Menandai cutscene sudah dimainkan (agar tidak dimainkan lagi)
/// 
/// Cara Pakai:
/// 1. Attach script ini ke GameObject baru bernama "CutsceneTrigger" di scene level1
/// 2. Isi field di Inspector:
///    - levelNumber = 1 (untuk Level 1)
///    - cutsceneData = Drag & drop DialogueSceneData asset yang sudah dibuat
/// 3. Script akan otomatis cek flag saat scene dimulai
/// 4. Jika flag "ShouldPlayCutscene" = true, cutscene akan dimainkan
/// 
/// PENTING untuk Pemula:
/// - Script ini HARUS ada di scene level (level1.unity)
/// - NovelCutsceneManager HARUS sudah di-setup dengan benar di scene
/// - DialogueSceneData asset HARUS sudah dibuat lewat Unity Editor
/// </summary>
public class CutsceneTrigger : MonoBehaviour
{
    // =====================================================
    // SETTINGS
    // =====================================================
    [Header("=== Level Settings ===")]
    [Tooltip("Nomor level ini (1, 2, 3, dll)")]
    public int levelNumber = 1;
    
    [Header("=== Cutscene Data ===")]
    [Tooltip("Drag & drop DialogueSceneData asset di sini (scene pertama)")]
    public DialogueSceneData firstCutsceneData;
    
    [Header("=== Optional: Multi-Scene Cutscene ===")]
    [Tooltip("Jika level ini punya 4 cutscene berbeda, isi semua. Jika tidak, biarkan kosong.")]
    public DialogueSceneData[] allCutsceneScenes;
    
    [Header("=== Debug ===")]
    [Tooltip("Centang ini untuk SELALU memainkan cutscene (untuk testing)")]
    public bool alwaysPlayCutscene = false;

    // =====================================================
    // UNITY LIFECYCLE: AWAKE & START
    // =====================================================
    void Awake()
    {
        Debug.Log($"[CutsceneTrigger] ✅ Awake() dipanggil! GameObject: {gameObject.name}, Active: {gameObject.activeInHierarchy}");
    }
    
    void Start()
    {
        Debug.Log($"[CutsceneTrigger] ✅ Start() dipanggil! Akan mulai CheckAndPlayCutscene...");
        // Tunggu 1 frame agar semua GameObject di scene sudah siap
        StartCoroutine(CheckAndPlayCutscene());
    }

    // =====================================================
    // COROUTINE: CHECK AND PLAY CUTSCENE
    // =====================================================
    /// <summary>
    /// Cek flag dan play cutscene jika diperlukan.
    /// </summary>
    private IEnumerator CheckAndPlayCutscene()
    {
        Debug.Log("[CutsceneTrigger] 🔍 CheckAndPlayCutscene coroutine dimulai...");
        
        // Tunggu 1 frame agar semua script di scene sudah Awake/Start
        yield return null;
        
        // Cek apakah harus play cutscene
        bool shouldPlay = false;
        
        if (alwaysPlayCutscene)
        {
            // Mode debug: selalu play
            shouldPlay = true;
            Debug.Log("[CutsceneTrigger] DEBUG MODE: Cutscene akan dimainkan (alwaysPlayCutscene = true)");
        }
        else
        {
            // Mode normal: cek flag dari PlayerPrefs
            int shouldPlayFlag = PlayerPrefs.GetInt("ShouldPlayCutscene", 0); // 0 = tidak, 1 = ya
            int flagLevel = PlayerPrefs.GetInt("CutsceneLevel", 0);
            
            // Cek apakah flag di-set DAN untuk level ini
            if (shouldPlayFlag == 1 && flagLevel == levelNumber)
            {
                shouldPlay = true;
                Debug.Log($"[CutsceneTrigger] Flag cutscene ditemukan untuk Level {levelNumber}");
            }
            else
            {
                Debug.Log($"[CutsceneTrigger] Tidak ada flag cutscene untuk Level {levelNumber} → skip");
            }
        }
        
        // Jika tidak perlu play cutscene, stop di sini
        if (!shouldPlay)
        {
            yield break;
        }
        
        // ===== PLAY CUTSCENE =====
        
        // Cek apakah NovelCutsceneManager ada
        if (NovelCutsceneManager.Instance == null)
        {
            Debug.LogError("[CutsceneTrigger] NovelCutsceneManager tidak ditemukan di scene! " +
                          "Pastikan NovelCutsceneManager sudah di-attach ke GameObject dan di-setup dengan benar.");
            yield break;
        }
        
        // PENTING: Jika ada multi-scene cutscene, mainkan SEMUA scene secara berurutan
        if (allCutsceneScenes != null && allCutsceneScenes.Length > 0)
        {
            Debug.Log($"[CutsceneTrigger] Akan memainkan {allCutsceneScenes.Length} cutscene scenes secara berurutan...");
            
            for (int i = 0; i < allCutsceneScenes.Length; i++)
            {
                if (allCutsceneScenes[i] == null)
                {
                    Debug.LogWarning($"[CutsceneTrigger] Scene ke-{i + 1} null, skip!");
                    continue;
                }
                
                Debug.Log($"[CutsceneTrigger] ▶ Memainkan cutscene Scene {i + 1}/{allCutsceneScenes.Length}...");
                
                // Play cutscene
                NovelCutsceneManager.Instance.PlayCutscene(allCutsceneScenes[i]);
                
                // Tunggu sampai cutscene selesai (cek property IsCutscenePlaying)
                while (NovelCutsceneManager.Instance.IsCutscenePlaying)
                {
                    yield return null;
                }
                
                Debug.Log($"[CutsceneTrigger] ✓ Scene {i + 1} selesai!");
            }
            
            Debug.Log("[CutsceneTrigger] ✓ Semua cutscene scenes selesai!");
        }
        else if (firstCutsceneData != null)
        {
            // Jika tidak ada array, gunakan single cutscene
            Debug.Log("[CutsceneTrigger] Menggunakan single cutscene data");
            NovelCutsceneManager.Instance.PlayCutscene(firstCutsceneData);
            
            // Tunggu sampai cutscene selesai
            while (NovelCutsceneManager.Instance.IsCutscenePlaying)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("[CutsceneTrigger] Tidak ada DialogueSceneData yang di-assign! " +
                          "Drag & drop DialogueSceneData asset ke Inspector.");
            yield break;
        }
        
        // Tandai cutscene sudah dimainkan (agar tidak dimainkan lagi)
        if (!alwaysPlayCutscene)
        {
            string cutscenePlayedKey = $"CutscenePlayed_Level{levelNumber}";
            PlayerPrefs.SetInt(cutscenePlayedKey, 1); // 1 = sudah dimainkan
            PlayerPrefs.SetInt("ShouldPlayCutscene", 0); // Reset flag global
            PlayerPrefs.Save();
            
            Debug.Log($"[CutsceneTrigger] ✓ Cutscene Level {levelNumber} ditandai sudah dimainkan");
        }
    }
}
