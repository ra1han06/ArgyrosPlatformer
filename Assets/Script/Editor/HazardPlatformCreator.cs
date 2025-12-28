using UnityEngine;
using UnityEditor;
using System.IO;

public class HazardPlatformCreator : EditorWindow
{
    [MenuItem("Tools/Create Hazard Platforms")]
    public static void CreateAllHazardPlatforms()
    {
        string[] platformPaths = new string[]
        {
            "Assets/ModelAsset/Inferno_World_Free/Prefabs/Platforms/PlatformSmall_001.prefab",
            "Assets/ModelAsset/Inferno_World_Free/Prefabs/Platforms/PlatformSmall_002.prefab",
            "Assets/ModelAsset/Inferno_World_Free/Prefabs/Platforms/PlatformSmall_003.prefab",
            "Assets/ModelAsset/Inferno_World_Free/Prefabs/Platforms/PlatformSmall_004.prefab",
            "Assets/ModelAsset/Inferno_World_Free/Prefabs/Platforms/Platform_003.prefab"
        };

        string[] hazardNames = new string[]
        {
            "Hazard_PlatformSmall_001",
            "Hazard_PlatformSmall_002",
            "Hazard_PlatformSmall_003",
            "Hazard_PlatformSmall_004",
            "Hazard_Platform_003"
        };

        string outputFolder = "Assets/Prefab/Hazard";
        
        // Create folder if doesn't exist
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }

        for (int i = 0; i < platformPaths.Length; i++)
        {
            CreateHazardPlatform(platformPaths[i], hazardNames[i], outputFolder);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"[HazardPlatformCreator] Successfully created {platformPaths.Length} hazard platforms!");
    }

    private static void CreateHazardPlatform(string sourcePrefabPath, string hazardName, string outputFolder)
    {
        // Load source prefab
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
        if (sourcePrefab == null)
        {
            Debug.LogError($"[HazardPlatformCreator] Source prefab not found: {sourcePrefabPath}");
            return;
        }

        // Instantiate in scene
        GameObject instance = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;
        instance.name = hazardName;
        instance.transform.position = new Vector3(0, -100, 0); // Off-screen

        // Disable MeshCollider (jika ada) karena akan pakai BoxCollider
        MeshCollider meshCol = instance.GetComponent<MeshCollider>();
        if (meshCol != null)
        {
            meshCol.enabled = false;
            Debug.Log($"[HazardPlatformCreator] Disabled MeshCollider on '{hazardName}'");
        }

        // Calculate proper bounds from MeshRenderer
        MeshRenderer meshRenderer = instance.GetComponent<MeshRenderer>();
        Bounds platformBounds;
        
        if (meshRenderer != null)
        {
            platformBounds = meshRenderer.localBounds;
        }
        else if (meshCol != null && meshCol.sharedMesh != null)
        {
            platformBounds = meshCol.sharedMesh.bounds;
        }
        else
        {
            // Fallback bounds
            platformBounds = new Bounds(Vector3.zero, new Vector3(10, 1, 10));
            Debug.LogWarning($"[HazardPlatformCreator] Using fallback bounds for '{hazardName}'");
        }

        // Add BoxCollider SOLID (untuk player berdiri)
        BoxCollider solidCollider = instance.AddComponent<BoxCollider>();
        solidCollider.isTrigger = false;
        solidCollider.center = platformBounds.center;
        solidCollider.size = platformBounds.size;

        // Add BoxCollider TRIGGER (untuk kill detection)
        BoxCollider triggerCollider = instance.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = platformBounds.center;
        triggerCollider.size = platformBounds.size;

        // Add PermanentHazardPlatform script
        PermanentHazardPlatform hazardScript = instance.AddComponent<PermanentHazardPlatform>();

        // Add CutablePlatform component so it can be cut/paste
        instance.AddComponent<CutablePlatform>();

        // Create Fire Particle System as child
        GameObject fireEffect = new GameObject("FireEffect");
        fireEffect.transform.SetParent(instance.transform);
        fireEffect.transform.localPosition = new Vector3(0, 2, 0);

        ParticleSystem ps = fireEffect.AddComponent<ParticleSystem>();
        
        // Configure particle system for fire effect
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.8f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360f * Mathf.Deg2Rad);
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        
        // Color gradient (orange to red)
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(1f, 0.5f, 0f), 0.0f),  // Orange
                new GradientColorKey(Color.red, 1.0f)  // Red
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Emission
        var emission = ps.emission;
        emission.rateOverTime = 30;

        // Shape - Box to cover platform surface
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(
            platformBounds.size.x * 0.8f,
            0.1f,
            platformBounds.size.z * 0.8f
        );

        // Renderer
        var psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        psRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        psRenderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");

        // Save as prefab
        string outputPath = $"{outputFolder}/{hazardName}.prefab";
        
        // Disconnect from source prefab first
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        
        // Save as new prefab
        GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(instance, outputPath);
        
        if (newPrefab != null)
        {
            Debug.Log($"[HazardPlatformCreator] Created: {outputPath}");
        }
        else
        {
            Debug.LogError($"[HazardPlatformCreator] Failed to create: {outputPath}");
        }

        // Clean up scene instance
        DestroyImmediate(instance);
    }
}
