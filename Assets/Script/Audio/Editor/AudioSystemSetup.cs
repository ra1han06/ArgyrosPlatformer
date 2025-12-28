using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/// <summary>
/// AUDIO SYSTEM SETUP - Automated Setup Tool
/// 
/// Tool untuk setup audio system secara otomatis ke semua scene.
/// 
/// Cara Pakai:
/// 1. Buka Unity Editor
/// 2. Menu: Tools → Setup Audio System
/// 3. Pilih opsi yang ingin disetup
/// 4. Klik tombol untuk execute
/// </summary>
public class AudioSystemSetup : EditorWindow
{
    private bool setupSceneAudio = true;
    private bool setupButtonSounds = true;
    private bool setupUIScenes = true;
    private bool setupLevelScenes = true;

    [MenuItem("Tools/Setup Audio System")]
    public static void ShowWindow()
    {
        GetWindow<AudioSystemSetup>("Audio System Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("=== AUDIO SYSTEM SETUP ===", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Setup Options:");
        setupSceneAudio = EditorGUILayout.Toggle("Add SceneAudioInitializer", setupSceneAudio);
        setupButtonSounds = EditorGUILayout.Toggle("Add ButtonSound to Buttons", setupButtonSounds);

        GUILayout.Space(10);
        GUILayout.Label("Target Scenes:");
        setupUIScenes = EditorGUILayout.Toggle("Setup UI Scenes", setupUIScenes);
        setupLevelScenes = EditorGUILayout.Toggle("Setup Level Scenes", setupLevelScenes);

        GUILayout.Space(20);

        if (GUILayout.Button("SETUP CURRENT SCENE", GUILayout.Height(40)))
        {
            SetupCurrentScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("SETUP ALL SCENES IN BUILD", GUILayout.Height(40)))
        {
            SetupAllScenesInBuild();
        }

        GUILayout.Space(20);
        GUILayout.Label("Progress:", EditorStyles.boldLabel);
        GUILayout.TextArea("Lihat Console untuk progress detail.");
    }

    // =====================================================
    // SETUP CURRENT SCENE
    // =====================================================
    private void SetupCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"[AudioSystemSetup] Memulai setup untuk scene: {currentScene.name}");

        // Determine scene type
        AudioManager.SceneType sceneType = DetermineSceneType(currentScene.path);

        if (setupSceneAudio)
        {
            AddSceneAudioInitializer(currentScene, sceneType);
        }

        if (setupButtonSounds)
        {
            AddButtonSoundComponents(sceneType);
        }

        // Save scene
        EditorSceneManager.SaveScene(currentScene);
        Debug.Log($"[AudioSystemSetup] ✅ Setup selesai untuk {currentScene.name}!");
    }

    // =====================================================
    // SETUP ALL SCENES IN BUILD
    // =====================================================
    private void SetupAllScenesInBuild()
    {
        // Save current scene first
        EditorSceneManager.SaveOpenScenes();

        int processedCount = 0;
        int totalScenes = EditorBuildSettings.scenes.Length;

        foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
        {
            if (!buildScene.enabled) continue;

            string scenePath = buildScene.path;
            AudioManager.SceneType sceneType = DetermineSceneType(scenePath);

            // Skip jika tidak match filter
            if (sceneType == AudioManager.SceneType.UI && !setupUIScenes) continue;
            if (sceneType == AudioManager.SceneType.Level && !setupLevelScenes) continue;

            // Load scene
            Scene scene = EditorSceneManager.OpenScene(scenePath);
            Debug.Log($"[AudioSystemSetup] Processing {processedCount + 1}/{totalScenes}: {scene.name}");

            if (setupSceneAudio)
            {
                AddSceneAudioInitializer(scene, sceneType);
            }

            if (setupButtonSounds)
            {
                AddButtonSoundComponents(sceneType);
            }

            // Save scene
            EditorSceneManager.SaveScene(scene);
            processedCount++;
        }

        Debug.Log($"[AudioSystemSetup] ✅ SELESAI! Total {processedCount} scene berhasil di-setup!");
    }

    // =====================================================
    // DETERMINE SCENE TYPE
    // =====================================================
    private AudioManager.SceneType DetermineSceneType(string scenePath)
    {
        // Deteksi berdasarkan path
        scenePath = scenePath.ToLower();

        if (scenePath.Contains("/ui/") || 
            scenePath.Contains("menu") || 
            scenePath.Contains("settings") ||
            scenePath.Contains("achievement") ||
            scenePath.Contains("guide") ||
            scenePath.Contains("pause") ||
            scenePath.Contains("restart") ||
            scenePath.Contains("exit") ||
            scenePath.Contains("complete"))
        {
            return AudioManager.SceneType.UI;
        }

        if (scenePath.Contains("/level/") || scenePath.Contains("level"))
        {
            return AudioManager.SceneType.Level;
        }

        // Default ke UI
        return AudioManager.SceneType.UI;
    }

    // =====================================================
    // ADD SCENE AUDIO INITIALIZER
    // =====================================================
    private void AddSceneAudioInitializer(Scene scene, AudioManager.SceneType sceneType)
    {
        // Cek apakah sudah ada SceneAudio GameObject
        GameObject[] rootObjects = scene.GetRootGameObjects();
        GameObject sceneAudio = null;

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == "SceneAudio")
            {
                sceneAudio = obj;
                break;
            }
        }

        // Jika belum ada, buat baru
        if (sceneAudio == null)
        {
            sceneAudio = new GameObject("SceneAudio");
            Debug.Log($"[AudioSystemSetup] ✨ Created SceneAudio GameObject in {scene.name}");
        }

        // Add atau get component
        SceneAudioInitializer initializer = sceneAudio.GetComponent<SceneAudioInitializer>();
        if (initializer == null)
        {
            initializer = sceneAudio.AddComponent<SceneAudioInitializer>();
            Debug.Log($"[AudioSystemSetup] ➕ Added SceneAudioInitializer component");
        }

        // Set scene type
        initializer.sceneType = sceneType;
        Debug.Log($"[AudioSystemSetup] 🎵 Set Scene Type = {sceneType} for {scene.name}");

        // Mark dirty untuk Unity save changes
        EditorUtility.SetDirty(sceneAudio);
    }

    // =====================================================
    // ADD BUTTON SOUND COMPONENTS
    // =====================================================
    private void AddButtonSoundComponents(AudioManager.SceneType sceneType)
    {
        // Find all Button components in scene
        Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);
        int addedCount = 0;

        foreach (Button button in allButtons)
        {
            // Skip jika sudah punya ButtonSound
            if (button.GetComponent<ButtonSound>() != null)
            {
                continue;
            }

            // Add ButtonSound component
            ButtonSound buttonSound = button.gameObject.AddComponent<ButtonSound>();
            buttonSound.sceneType = sceneType;

            Debug.Log($"[AudioSystemSetup] 🔘 Added ButtonSound to: {button.gameObject.name}");
            addedCount++;

            // Mark dirty
            EditorUtility.SetDirty(button.gameObject);
        }

        if (addedCount > 0)
        {
            Debug.Log($"[AudioSystemSetup] ✅ Added ButtonSound to {addedCount} buttons!");
        }
        else
        {
            Debug.Log($"[AudioSystemSetup] ℹ️ All buttons already have ButtonSound component");
        }
    }
}
