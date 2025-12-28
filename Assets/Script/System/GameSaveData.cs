using System;

/// <summary>
/// GAME SAVE DATA - Data Structure untuk Save System
/// 
/// Fungsi:
/// - Container untuk semua data yang perlu disimpan
/// - Serializable agar bisa di-convert ke JSON
/// - Digunakan oleh SaveSystem untuk save/load
/// 
/// Data yang Disimpan:
/// - Level progression (last played, completed, unlocked)
/// - Star rating per level
/// - Total play time
/// - Achievement unlock status
/// </summary>
[Serializable]
public class GameSaveData
{
    // =====================================================
    // LEVEL PROGRESSION DATA
    // =====================================================
    
    /// <summary>
    /// Level terakhir yang dimainkan (1 = level1, 2 = level2, dst)
    /// Digunakan untuk Continue button di MainMenu
    /// </summary>
    public int lastPlayedLevel = 1;

    /// <summary>
    /// Total waktu bermain dalam detik
    /// </summary>
    public float totalPlayTime = 0f;

    /// <summary>
    /// Array boolean untuk tracking level yang sudah diselesaikan
    /// Index 0 = Level 1, Index 1 = Level 2, dst
    /// true = sudah diselesaikan, false = belum
    /// </summary>
    public bool[] completedLevels;

    /// <summary>
    /// Array integer untuk star rating per level (0-3 stars)
    /// Index 0 = Level 1, Index 1 = Level 2, dst
    /// 0 = belum main/completed, 1-3 = jumlah stars
    /// </summary>
    public int[] levelStars;

    /// <summary>
    /// Array boolean untuk tracking level yang sudah unlocked
    /// Index 0 = Level 1 (always true), Index 1 = Level 2, dst
    /// true = unlocked, false = locked
    /// </summary>
    public bool[] unlockedLevels;

    // =====================================================
    // ACHIEVEMENT DATA
    // =====================================================
    
    /// <summary>
    /// Array boolean untuk tracking achievement yang sudah unlocked
    /// Index sesuai dengan Achievement ID (akan didefinisikan di AchievementManager)
    /// true = unlocked, false = locked
    /// </summary>
    public bool[] unlockedAchievements;

    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    
    /// <summary>
    /// Constructor default - initialize semua array dengan ukuran tetap
    /// Asumsi: 10 levels, 20 achievements (bisa disesuaikan)
    /// </summary>
    public GameSaveData()
    {
        // Initialize arrays dengan ukuran default
        completedLevels = new bool[10];      // 10 levels
        levelStars = new int[10];            // 10 levels, 0-3 stars each
        unlockedLevels = new bool[10];       // 10 levels
        unlockedAchievements = new bool[20]; // 20 achievements

        // Level 1 selalu unlocked
        unlockedLevels[0] = true;
        
        // Default values
        lastPlayedLevel = 1;
        totalPlayTime = 0f;
    }

    /// <summary>
    /// Constructor dengan custom array sizes
    /// </summary>
    public GameSaveData(int totalLevels, int totalAchievements)
    {
        completedLevels = new bool[totalLevels];
        levelStars = new int[totalLevels];
        unlockedLevels = new bool[totalLevels];
        unlockedAchievements = new bool[totalAchievements];

        // Level 1 selalu unlocked
        if (totalLevels > 0)
        {
            unlockedLevels[0] = true;
        }
        
        lastPlayedLevel = 1;
        totalPlayTime = 0f;
    }
}
