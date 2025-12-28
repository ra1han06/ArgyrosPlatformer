using UnityEngine;

/// <summary>
/// SAVE SYSTEM - Static Manager untuk Save/Load Game
/// 
/// Fungsi:
/// - Menyimpan game data ke PlayerPrefs sebagai JSON
/// - Load game data dari PlayerPrefs
/// - Delete save data
/// - Cek apakah save file exists
/// 
/// Cara Pakai:
/// - SaveSystem.SaveGame(data) → simpan data
/// - SaveSystem.LoadGame() → load data
/// - SaveSystem.HasSaveFile() → cek apakah ada save
/// - SaveSystem.DeleteSave() → hapus save
/// 
/// Note: Menggunakan PlayerPrefs untuk simplicity
/// Untuk production, bisa diganti dengan File I/O atau Cloud Save
/// </summary>
public static class SaveSystem
{
    // =====================================================
    // CONSTANTS
    // =====================================================
    
    /// <summary>
    /// Key untuk menyimpan save data di PlayerPrefs
    /// </summary>
    private const string SAVE_KEY = "SAVE_GAME_DATA";

    /// <summary>
    /// Key untuk flag apakah save file exists
    /// </summary>
    private const string SAVE_EXISTS_KEY = "HAS_SAVE_FILE";

    /// <summary>
    /// Enable debug logging
    /// </summary>
    private const bool DEBUG_MODE = true;

    // =====================================================
    // SAVE GAME
    // =====================================================
    
    /// <summary>
    /// Simpan game data ke PlayerPrefs sebagai JSON
    /// </summary>
    /// <param name="data">GameSaveData yang akan disimpan</param>
    public static void SaveGame(GameSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("[SaveSystem] Cannot save null data!");
            return;
        }

        try
        {
            // Convert data object ke JSON string
            string json = JsonUtility.ToJson(data, true); // true = pretty print untuk debugging

            // Simpan JSON ke PlayerPrefs
            PlayerPrefs.SetString(SAVE_KEY, json);

            // Set flag bahwa save file exists
            PlayerPrefs.SetInt(SAVE_EXISTS_KEY, 1);

            // Commit changes ke disk
            PlayerPrefs.Save();

            if (DEBUG_MODE)
            {
                Debug.Log($"[SaveSystem] ✅ Game saved successfully!");
                Debug.Log($"[SaveSystem] Last Played Level: {data.lastPlayedLevel}");
                Debug.Log($"[SaveSystem] Total Play Time: {data.totalPlayTime:F1}s");
                Debug.Log($"[SaveSystem] JSON Data:\n{json}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] ❌ Failed to save game: {e.Message}");
        }
    }

    // =====================================================
    // LOAD GAME
    // =====================================================
    
    /// <summary>
    /// Load game data dari PlayerPrefs
    /// Jika tidak ada save file, return null
    /// </summary>
    /// <returns>GameSaveData jika ada, null jika tidak ada</returns>
    public static GameSaveData LoadGame()
    {
        // Cek apakah save file exists
        if (!HasSaveFile())
        {
            if (DEBUG_MODE)
                Debug.LogWarning("[SaveSystem] ⚠️ No save file found. Returning null.");
            
            return null;
        }

        try
        {
            // Ambil JSON string dari PlayerPrefs
            string json = PlayerPrefs.GetString(SAVE_KEY, "");

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[SaveSystem] ⚠️ Save file exists but JSON is empty!");
                return null;
            }

            // Convert JSON string ke GameSaveData object
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

            if (data == null)
            {
                Debug.LogError("[SaveSystem] ❌ Failed to deserialize save data!");
                return null;
            }

            if (DEBUG_MODE)
            {
                Debug.Log($"[SaveSystem] ✅ Game loaded successfully!");
                Debug.Log($"[SaveSystem] Last Played Level: {data.lastPlayedLevel}");
                Debug.Log($"[SaveSystem] Total Play Time: {data.totalPlayTime:F1}s");
            }

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] ❌ Failed to load game: {e.Message}");
            return null;
        }
    }

    // =====================================================
    // DELETE SAVE
    // =====================================================
    
    /// <summary>
    /// Hapus save data dari PlayerPrefs
    /// Digunakan untuk New Game atau reset progress
    /// </summary>
    public static void DeleteSave()
    {
        // Hapus save data
        PlayerPrefs.DeleteKey(SAVE_KEY);

        // Hapus flag save exists
        PlayerPrefs.DeleteKey(SAVE_EXISTS_KEY);

        // Commit changes
        PlayerPrefs.Save();

        if (DEBUG_MODE)
            Debug.Log("[SaveSystem] 🗑️ Save file deleted successfully!");
    }

    /// <summary>
    /// Hapus SEMUA data save termasuk Best Time, Best Deaths, Achievement
    /// TOTAL RESET - untuk New Game yang benar-benar fresh start
    /// </summary>
    public static void DeleteAllData()
    {
        if (DEBUG_MODE)
            Debug.Log("[SaveSystem] 🔥 Deleting ALL save data (Total Reset)...");

        // Hapus save game data
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey(SAVE_EXISTS_KEY);

        // Hapus SEMUA Best Time & Best Deaths untuk semua level (1-10)
        for (int i = 1; i <= 10; i++)
        {
            PlayerPrefs.DeleteKey($"BestTime_Level{i}");
            PlayerPrefs.DeleteKey($"BestDeaths_Level{i}");
        }

        // Hapus cutscene flags
        for (int i = 1; i <= 10; i++)
        {
            PlayerPrefs.DeleteKey($"CutscenePlayed_Level{i}");
        }

        // Hapus flags lainnya
        PlayerPrefs.DeleteKey("ShouldPlayCutscene");
        PlayerPrefs.DeleteKey("CutsceneLevel");

        // Commit changes
        PlayerPrefs.Save();

        if (DEBUG_MODE)
            Debug.Log("[SaveSystem] ✅ ALL data deleted! Game reset to fresh start.");
    }

    // =====================================================
    // CHECK SAVE FILE EXISTS
    // =====================================================
    
    /// <summary>
    /// Cek apakah save file exists
    /// </summary>
    /// <returns>true jika ada save file, false jika tidak ada</returns>
    public static bool HasSaveFile()
    {
        // Cek flag di PlayerPrefs
        int hasSave = PlayerPrefs.GetInt(SAVE_EXISTS_KEY, 0);
        
        // Double check: pastikan JSON data juga exists
        if (hasSave == 1)
        {
            string json = PlayerPrefs.GetString(SAVE_KEY, "");
            if (string.IsNullOrEmpty(json))
            {
                // Inconsistent state - fix it
                PlayerPrefs.SetInt(SAVE_EXISTS_KEY, 0);
                PlayerPrefs.Save();
                return false;
            }
        }

        return hasSave == 1;
    }

    // =====================================================
    // UTILITY METHODS
    // =====================================================
    
    /// <summary>
    /// Get save file info untuk debugging
    /// </summary>
    public static string GetSaveInfo()
    {
        if (!HasSaveFile())
            return "No save file found.";

        GameSaveData data = LoadGame();
        if (data == null)
            return "Save file exists but failed to load.";

        int completedCount = 0;
        int totalStars = 0;

        for (int i = 0; i < data.completedLevels.Length; i++)
        {
            if (data.completedLevels[i])
                completedCount++;
            
            totalStars += data.levelStars[i];
        }

        return $"Last Played: Level {data.lastPlayedLevel}\n" +
               $"Completed Levels: {completedCount}/{data.completedLevels.Length}\n" +
               $"Total Stars: {totalStars}\n" +
               $"Total Play Time: {FormatTime(data.totalPlayTime)}";
    }

    /// <summary>
    /// Format time to MM:SS
    /// </summary>
    private static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }
}
