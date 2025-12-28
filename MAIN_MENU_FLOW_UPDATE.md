# MAIN MENU FLOW UPDATE 🎮

**Status:** ✅ **COMPLETE** - Implemented & Ready to Test

---

## 📋 CHANGES SUMMARY

### **OLD FLOW (Before):**
```
Main Menu
├── New Game → Delete save only → (no scene change)
└── Continue → Load last level DIRECTLY (e.g., level5)
```

### **NEW FLOW (Current):**
```
Main Menu
├── New Game → Total Reset → Create Default Save → SelectLevel (Only Lvl 1 unlocked)
└── Continue → Load Save → SelectLevel (with unlocked levels)
```

---

## 🔄 NEW GAME BUTTON

### What Happens:
1. **Total Data Reset**
   - Deletes save file (`SAVE_GAME_DATA`)
   - Deletes ALL Best Time records (`BestTime_Level1` to `BestTime_Level10`)
   - Deletes ALL Best Deaths records (`BestDeaths_Level1` to `BestDeaths_Level10`)
   - Deletes ALL Cutscene flags (`CutscenePlayed_Level1` to `CutscenePlayed_Level10`)
   - Deletes other flags (`ShouldPlayCutscene`, `CutsceneLevel`)

2. **Create Fresh Save**
   ```csharp
   GameSaveData newSave = new GameSaveData();
   // Constructor automatically sets:
   // - lastPlayedLevel = 1
   // - unlockedLevels[0] = true (only Level 1)
   // - All others false
   ```

3. **Save Default Data**
   - Calls `SaveSystem.SaveGame(newSave)`
   - Creates new save file with fresh data

4. **Navigate to SelectLevel**
   - Loads `SelectLevel` scene
   - Player can only see Level 1 (others locked)

### Code Implementation:
**File:** `SceneController.cs` → `OnNewGameButtonClicked()`

```csharp
public void OnNewGameButtonClicked()
{
    Debug.Log("[SceneController] New Game → TOTAL RESET & SelectLevel");
    
    // HAPUS SEMUA DATA (Total Reset)
    SaveSystem.DeleteAllData();
    
    // Buat save data baru dengan default values
    GameSaveData newSave = new GameSaveData();
    
    // Simpan save baru
    SaveSystem.SaveGame(newSave);
    
    // Load save ke GameManager
    if (GameManager.Instance != null)
    {
        GameManager.Instance.currentSave = newSave;
        GameManager.Instance.currentLevelIndex = 1;
    }
    
    // Pergi ke SelectLevel
    LoadScene(SELECT_LEVEL);
}
```

---

## 🔄 CONTINUE BUTTON

### What Happens:
1. **Load Existing Save**
   - Calls `SaveSystem.LoadGame()`
   - Gets `GameSaveData` with:
     - Last played level
     - Unlocked levels
     - Achievements

2. **Set GameManager State**
   - Sets `GameManager.Instance.currentSave`
   - Preserves progress (unlocked levels, achievements)

3. **Navigate to SelectLevel**
   - Loads `SelectLevel` scene
   - Player sees ALL unlocked levels (with lock/unlock visuals)
   - Can choose any unlocked level to play

### Code Implementation:
**File:** `SceneController.cs` → `OnContinueButtonClicked()`

```csharp
public void OnContinueButtonClicked()
{
    // Main Menu: Continue → Load save dan pergi ke SELECT LEVEL
    Debug.Log("[SceneController] Continue → Load save & go to SelectLevel");
    
    // Load save data
    GameSaveData saveData = SaveSystem.LoadGame();
    
    if (saveData != null)
    {
        // Load save ke GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentSave = saveData;
        }
        
        // Pergi ke SelectLevel (bukan langsung ke level)
        LoadScene(SELECT_LEVEL);
    }
    else
    {
        // Fallback: jika save data tidak ada, pergi ke SelectLevel
        LoadScene(SELECT_LEVEL);
    }
}
```

---

## 🆕 NEW METHOD: SaveSystem.DeleteAllData()

### Purpose:
Total reset untuk New Game yang benar-benar fresh start.

### What It Deletes:
```csharp
// Save data
PlayerPrefs.DeleteKey(SAVE_KEY);
PlayerPrefs.DeleteKey(SAVE_EXISTS_KEY);

// Best records for ALL levels (1-10)
for (int i = 1; i <= 10; i++)
{
    PlayerPrefs.DeleteKey($"BestTime_Level{i}");
    PlayerPrefs.DeleteKey($"BestDeaths_Level{i}");
}

// Cutscene flags for ALL levels (1-10)
for (int i = 1; i <= 10; i++)
{
    PlayerPrefs.DeleteKey($"CutscenePlayed_Level{i}");
}

// Other flags
PlayerPrefs.DeleteKey("ShouldPlayCutscene");
PlayerPrefs.DeleteKey("CutsceneLevel");

// Commit changes
PlayerPrefs.Save();
```

### Code Implementation:
**File:** `SaveSystem.cs`

```csharp
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
```

---

## 🎯 USER EXPERIENCE

### Scenario 1: First Time Player
```
1. Open game → MainMenu
2. Click "New Game"
3. → Total Reset (no data exists yet)
4. → Create default save (Level 1 unlocked)
5. → Go to SelectLevel
6. → See only Level 1 (others locked 🔒)
7. → Click Level 1 → Start playing
```

### Scenario 2: Returning Player (Has Progress)
```
1. Open game → MainMenu
2. Click "Continue"
3. → Load existing save (e.g., unlocked up to Level 5)
4. → Go to SelectLevel
5. → See Levels 1-5 unlocked ✅, 6-10 locked 🔒
6. → Can choose any unlocked level
7. → Can also see Best Times/Deaths for completed levels
```

### Scenario 3: Player Wants Fresh Start
```
1. Open game → MainMenu
2. Click "New Game" (even if has progress)
3. → Total Reset (deletes ALL progress, achievements, best records)
4. → Create fresh save (back to Level 1 only)
5. → Go to SelectLevel
6. → See only Level 1 again (complete reset)
```

---

## 📦 FILES MODIFIED

### 1. **SaveSystem.cs**
- ✅ Added `DeleteAllData()` method
- Total reset for New Game functionality

### 2. **SceneController.cs**
- ✅ Updated `OnNewGameButtonClicked()`
  - Calls `DeleteAllData()` instead of `DeleteSave()`
  - Creates default save
  - Goes to SelectLevel

- ✅ Updated `OnContinueButtonClicked()`
  - Loads save
  - Goes to SelectLevel (NOT last level)

### 3. **MainMenuController.cs**
- ✅ Updated `OnNewGame()`
  - Calls `DeleteAllData()`
  - Creates default save
  - Updates Continue button state (now enabled after fresh save)

---

## ✅ TESTING CHECKLIST

### Test 1: New Game (Fresh Start)
- [ ] Click "New Game" from MainMenu
- [ ] Verify SelectLevel scene loads
- [ ] Verify only Level 1 is unlocked
- [ ] Verify no Best Time/Deaths shown
- [ ] Complete Level 1 → Check unlock Level 2
- [ ] Return to MainMenu → Continue button should be ENABLED

### Test 2: Continue (With Progress)
- [ ] Have existing save with multiple levels unlocked
- [ ] Click "Continue" from MainMenu
- [ ] Verify SelectLevel scene loads (NOT last level)
- [ ] Verify all previously unlocked levels are visible
- [ ] Verify Best Times/Deaths are shown for completed levels
- [ ] Can select any unlocked level

### Test 3: Total Reset
- [ ] Have existing progress (unlocked up to Level 5)
- [ ] Click "New Game"
- [ ] Verify ALL data deleted (console log should show)
- [ ] Verify only Level 1 unlocked again
- [ ] Verify Best Times/Deaths cleared
- [ ] Previous progress completely gone

### Test 4: Continue Button State
- [ ] Fresh install → Continue button DISABLED (no save)
- [ ] After New Game → Continue button ENABLED (has fresh save)
- [ ] After playing some levels → Continue button ENABLED (has progress)

---

## 🐛 POTENTIAL ISSUES & SOLUTIONS

### Issue 1: Continue Button Still Disabled After New Game
**Cause:** MainMenuController not refreshing state after save creation  
**Solution:** MainMenuController.OnNewGame() calls UpdateContinueButtonState()  
**Status:** ✅ Already handled in code

### Issue 2: GameManager.Instance Null in SelectLevel
**Cause:** GameManager not persisting between scenes  
**Solution:** Ensure GameManager has DontDestroyOnLoad  
**Status:** ⚠️ Check GameManager implementation

### Issue 3: SelectLevel Shows All Levels as Locked
**Cause:** GameManager.currentSave not set before loading SelectLevel  
**Solution:** SceneController sets currentSave BEFORE LoadScene  
**Status:** ✅ Already handled in code

---

## 🎮 INTEGRATION WITH EXISTING SYSTEMS

### LevelButtonHandler (SelectLevel Scene)
- ✅ Already implemented lock/unlock visuals
- ✅ Reads `GameManager.Instance.currentSave.unlockedLevels[]`
- ✅ Shows/hides lock icons based on unlock status
- ✅ Displays Best Time/Deaths from PlayerPrefs
- **No changes needed** - works automatically with new flow

### CompleteMenuController (Complete Scene)
- ✅ "Main Menu" button → MainMenu
- ✅ "Retry" button → Reload current level
- ✅ "Next Level" button → Next level (or MainMenu if last level)
- **No changes needed** - independent system

### GameManager
- ✅ CompleteLevel() unlocks next level
- ✅ Saves progress automatically after completion
- ✅ currentSave property tracks player progress
- **No changes needed** - works with new flow

---

## 🚀 DEPLOYMENT

### Step 1: Verify Scene Setup
```
MainMenu scene:
- New Game button → SceneController.OnNewGameButtonClicked()
- Continue button → SceneController.OnContinueButtonClicked()
- MainMenuController updates Continue button state
```

### Step 2: Build Settings
```
Ensure scenes are in Build Settings:
0. MainMenu
1. SelectLevel
2. level1, level2, ..., level10
3. Complete
4. Pause
```

### Step 3: Test Flow
```
1. Build project
2. Test New Game flow
3. Test Continue flow
4. Test Total Reset
5. Verify lock/unlock mechanics work
```

---

## 📝 NOTES

### Design Decision: Why SelectLevel Instead of Direct Level Load?

**OLD:** Continue → Last played level  
**Problem:** Player might want to replay earlier levels or select different level

**NEW:** Continue → SelectLevel  
**Benefits:**
- Player has freedom to choose any unlocked level
- Can see progress at a glance (unlocked levels)
- Can check Best Times/Deaths before selecting
- More user-friendly UX

### SelectLevel as Central Hub
SelectLevel scene now serves as the **central navigation hub**:
- New Game → SelectLevel (fresh start)
- Continue → SelectLevel (with progress)
- After completing a level → SelectLevel (choose next action)
- Unified entry point for all level selection

---

## 📞 SUPPORT

Jika ada issue atau pertanyaan:
1. Check Console log untuk debug messages
2. Verify GameManager exists di scene
3. Check SaveSystem debug mode untuk detailed logs
4. Pastikan semua scenes ada di Build Settings

**Status:** ✅ Ready for Testing  
**Date:** 2024 (Current Session)  
**Version:** v2.0 - SelectLevel Flow Update
