# 🔧 ARGYROS PLATFORMER - FLOW TROUBLESHOOTING

**Common Issues & Solutions** - Debug guide untuk game flow problems

---

## 🐛 COMMON FLOW ISSUES

### **Issue 1: Continue Button Disabled (Gray)**

**Symptom:**
- MainMenu → Continue button tidak bisa diklik
- Button grayed out atau hidden

**Root Cause:**
- Tidak ada save file
- `SaveSystem.HasSaveFile()` returns false

**Solution:**
```
Option 1: Play New Game
  → New Game → SelectLevel → Play level → Win
  → Save file created
  → Continue button enabled

Option 2: Debug - Force create save
  → GameManager or manual script
  → SaveSystem.SaveGame(new GameSaveData())
```

**Prevention:**
- New Game always creates save file
- Complete level saves progress
- Check `MainMenuController` updates button state

---

### **Issue 2: Player Stuck (Time Frozen)**

**Symptom:**
- Player cannot move
- Timer not running
- Everything frozen

**Root Cause:**
- `Time.timeScale = 0f` not reset to 1f
- Cutscene/Pause didn't resume properly

**Solution:**
```
Immediate Fix:
  → Press ESC → Resume
  → Or reload scene

Code Fix (Already Implemented):
  → All LoadScene() calls now reset Time.timeScale = 1f
  → CompleteMenuController resets before scene load
  → WinningTrigger resets before Complete scene
```

**Files with Time.timeScale Reset:**
- SceneController.LoadScene()
- CompleteMenuController (all buttons)
- WinningTrigger.LoadCompleteScene()
- PauseManager.Resume()
- LevelButtonHandler.OnLevelButtonClicked()

---

### **Issue 3: Restart Button Goes to Wrong Level**

**Symptom:**
- Pause → Restart → Always goes to Level 1
- Not restarting current level

**Root Cause:**
- Hardcoded in SceneController: `SceneManager.LoadScene("level1")`

**Current Code:**
```csharp
// Line 345
SceneManager.LoadScene("level1");  // ❌ HARDCODED
```

**Proposed Fix:**
```csharp
// Should be:
int currentLevel = GameManager.Instance.currentLevelIndex;
SceneManager.LoadScene($"level{currentLevel}");
```

**Workaround:**
- Use Complete → Retry instead
- CompleteMenuController.OnRetryClicked() correctly uses currentLevelIndex

---

### **Issue 4: Cutscene Plays Every Time**

**Symptom:**
- Replay level → Cutscene plays again
- Should skip after first play

**Root Cause:**
- `CutscenePlayed_Level{X}` flag not set
- Or flag deleted

**Solution:**
```
Check PlayerPrefs:
  → CutscenePlayed_Level1 = 1 (should be set after first play)

If missing:
  → NovelCutsceneManager should set flag after cutscene ends
  → Check ResumeGameplay() method

Manual Fix:
  → PlayerPrefs.SetInt("CutscenePlayed_Level1", 1);
```

**Trigger Mechanism:**
```
LevelButtonHandler.OnLevelButtonClicked()
  → Check: PlayerPrefs.GetInt(cutscenePlayedKey, 0)
  → If 0 (not played):
      → Set ShouldPlayCutscene = 1
      → Set CutsceneLevel = X
  → If 1 (already played): Skip
```

---

### **Issue 5: All Levels Locked After Continue**

**Symptom:**
- Continue → SelectLevel → Only Level 1 unlocked
- Previously unlocked levels now locked

**Root Cause:**
- Save data corrupted or not loaded
- GameManager.currentSave is null

**Solution:**
```
Check in OnContinueButtonClicked():
  → SaveSystem.LoadGame() returns valid data?
  → GameManager.currentSave is set?

Debug:
  → Add log in LevelButtonHandler to see currentSave.unlockedLevels[]
  → Check SaveSystem.LoadGame() returns non-null

If corrupted:
  → New Game (reset)
  → Or manually fix PlayerPrefs "SAVE_GAME_DATA"
```

**Prevention:**
- Always save after level complete
- GameManager.CompleteLevel() calls SaveSystem.SaveGame()

---

### **Issue 6: Best Time/Deaths Not Showing**

**Symptom:**
- Complete level with good time
- SelectLevel → No best time displayed

**Root Cause:**
- PlayerPrefs keys not saved
- `BestTime_Level{X}` or `BestDeaths_Level{X}` missing

**Solution:**
```
Check in GameManager.CompleteLevel():
  → PlayerPrefs.SetFloat($"BestTime_Level{levelIndex}", time);
  → PlayerPrefs.SetInt($"BestDeaths_Level{levelIndex}", deaths);
  → PlayerPrefs.Save();

Verify in LevelButtonHandler:
  → PlayerPrefs.GetFloat($"BestTime_Level{levelNumber}", -1f);
  → If -1f → No record yet (correct)
```

**Manual Test:**
```
Complete level → Check Console for log
  → "[GameManager] Best time saved: ..."
  → "[GameManager] Best deaths saved: ..."
```

---

### **Issue 7: Back Button Goes to Wrong Scene**

**Symptom:**
- Guide → Back → Goes to MainMenu instead of Settings

**Current Behavior:**
```
Guide → Back → Settings ✅ (correct)
Settings → Back → MainMenu ✅ (correct)
SelectLevel → Back → MainMenu ✅ (correct)
```

**If Wrong:**
- Check SceneController.OnBackButtonClicked() logic
- Scene name detection: `SceneManager.GetActiveScene().name`

---

### **Issue 8: Music Doesn't Resume After Pause**

**Symptom:**
- Pause → Resume → Music still silent

**Root Cause:**
- AudioManager.ResumeBGM() not called
- Or AudioManager instance null

**Solution:**
```
Check PauseManager.Resume():
  → AudioManager.Instance != null?
  → AudioManager.Instance.ResumeBGM() called?

Test AudioManager:
  → Debug.Log in ResumeBGM() method
  → Check BGM AudioSource state
```

---

### **Issue 9: Next Level Button Disabled**

**Symptom:**
- Complete Level 5 → Next Level button grayed out
- Should go to Level 6

**Root Cause:**
- Hardcoded TOTAL_LEVELS = 10
- Scene "level6" not in Build Settings

**Solution:**
```
Check Build Settings:
  → File → Build Settings → Scenes in Build
  → Verify level1, level2, ..., level10 are added

If missing:
  → Add scene to Build Settings
  → Or create level scene

CompleteMenuController check:
  → Application.CanStreamedLevelBeLoaded($"level{nextLevel}")
  → If false → Shows warning, goes to MainMenu
```

---

### **Issue 10: Achievement Navigation Stuck**

**Symptom:**
- AchievementGlobal → Next → Nothing happens
- Or wrong scene loads

**Root Cause:**
- Scene name mismatch
- "Achievement1" vs "Ach1"

**Current Scene Names:**
```
AchievementGlobal ✅
Achievement1 ✅
Achievement2 ✅
Achievement3 ✅
```

**Check SceneController Constants:**
```csharp
public static readonly string ACHIEVEMENTS = "AchievementGlobal";
public static readonly string ACHIEVEMENT_1 = "Achievement1";
public static readonly string ACHIEVEMENT_2 = "Achievement2";
public static readonly string ACHIEVEMENT_3 = "Achievement3";
```

**Solution:**
- Ensure scene files match constant names exactly
- Case-sensitive!

---

## 🔍 DEBUG CHECKLIST

### **When Scene Won't Load:**
- [ ] Check scene name spelling (case-sensitive)
- [ ] Verify scene in Build Settings
- [ ] Check Console for LoadScene errors
- [ ] Verify Time.timeScale = 1f before load

### **When Button Doesn't Work:**
- [ ] Button GameObject name matches SceneController pattern?
- [ ] Button has Button component?
- [ ] Button interactable = true?
- [ ] Check Console for "[SceneController] Auto-setup complete" log

### **When Save Data Issues:**
- [ ] Check PlayerPrefs has "SAVE_GAME_DATA" key
- [ ] Verify JSON format valid
- [ ] Check SaveSystem.LoadGame() returns non-null
- [ ] Verify GameManager.currentSave is set

### **When Pause/Resume Issues:**
- [ ] Check Time.timeScale value in Inspector
- [ ] Verify PauseManager exists in scene
- [ ] Check pauseMenuPanel reference assigned
- [ ] Verify AudioManager instance exists

---

## 🛠️ TESTING FLOWS

### **Test 1: New Player Journey**
```
1. Delete PlayerPrefs (Edit → Clear All PlayerPrefs)
2. Play → MainMenu
3. Continue button should be DISABLED ✅
4. Click New Game → SelectLevel
5. Only Level 1 unlocked ✅
6. Click Level 1 → Cutscene plays ✅
7. Play → Win → Complete
8. Next Level → Level 2 unlocked ✅
9. Main Menu → Continue button ENABLED ✅
```

### **Test 2: Continue Flow**
```
1. Ensure save exists (complete at least 1 level)
2. Main Menu → Continue
3. SelectLevel shows unlocked levels ✅
4. Best times visible ✅
5. Can choose any unlocked level ✅
6. Replay Level 1 → NO cutscene ✅
```

### **Test 3: Pause/Resume**
```
1. Play level
2. ESC → Pause menu ✅
3. DeathTimerUI hidden ✅
4. Time.timeScale = 0f ✅
5. Resume → Gameplay continues ✅
6. DeathTimerUI shown ✅
7. Time.timeScale = 1f ✅
```

### **Test 4: Complete Flow**
```
1. Win level
2. Complete scene loads ✅
3. Stats displayed ✅
4. Retry → Level reloads ✅
5. Next Level → Next level loads ✅
6. Main Menu → MainMenu ✅
```

---

## 📋 COMMON CONSOLE LOGS

### **Normal Flow:**
```
[SaveSystem] 💾 Game saved successfully!
[GameManager] ✅ Fresh save created - Only Level 1 unlocked
[SceneController] Loading scene: SelectLevel (Time.timeScale reset to 1f)
[LevelButtonHandler] Loading scene: level1
[NovelCutsceneManager] Starting cutscene sequence...
[GameManager] Cutscene finished!
[GameManager] ⏱️ Timer started
[WinningTrigger] Player menyentuh winning platform!
[GameManager] 🎉 Level complete!
```

### **Error Indicators:**
```
❌ "[SceneController] Scene name is empty!"
❌ "[SaveSystem] Failed to load game data!"
❌ "[GameManager] GameManager.Instance tidak ditemukan!"
❌ "[CompleteMenuController] GameManager.Instance is null!"
❌ "[PauseManager] PauseMenuPanel not found!"
```

---

## 🎯 QUICK FIXES

### **Reset Everything:**
```csharp
// In Unity Editor or script
PlayerPrefs.DeleteAll();
PlayerPrefs.Save();
// Reload game
```

### **Force Unlock All Levels:**
```csharp
GameSaveData save = new GameSaveData();
for (int i = 0; i < save.unlockedLevels.Length; i++)
{
    save.unlockedLevels[i] = true;
}
SaveSystem.SaveGame(save);
```

### **Reset Time.timeScale:**
```csharp
Time.timeScale = 1f;
```

### **Check Current Save:**
```csharp
GameSaveData save = SaveSystem.LoadGame();
if (save != null)
{
    Debug.Log($"Last played: Level {save.lastPlayedLevel}");
    Debug.Log($"Unlocked: {string.Join(", ", save.unlockedLevels)}");
}
```

---

## 📞 SUPPORT RESOURCES

**Documentation:**
- GAME_FLOW_DOCUMENTATION.md - Complete flow details
- GAME_FLOW_QUICK_REFERENCE.md - Quick navigation guide
- TIMESCALE_BUG_FIX.md - Time.timeScale issue fix
- MAIN_MENU_FLOW_UPDATE.md - New Game/Continue system

**Key Scripts:**
- SceneController.cs - Universal scene navigation
- GameManager.cs - Level progression & save
- SaveSystem.cs - Data persistence
- PauseManager.cs - Pause/Resume system
- CompleteMenuController.cs - Complete scene logic
- NovelCutsceneManager.cs - Cutscene system

**Debug Tools:**
- Unity Console - Check for errors/warnings
- PlayerPrefs - Edit → Clear All PlayerPrefs
- Build Settings - Verify scenes included
- Inspector - Check Time.timeScale, component references

---

**Last Updated:** 2025-12-30  
**Version:** Troubleshooting Guide v1.0  
**Project:** ArgyrosPlatformer
