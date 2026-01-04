# 🔄 LEVEL RESET SYSTEM - SETUP GUIDE

**Implementation Date:** January 4, 2026  
**Purpose:** Full level reset on player death while preserving death count and timer

---

## 📋 OVERVIEW

The new Level Reset System ensures that when a player dies, **EVERYTHING** resets to its initial state:
- ✅ All platforms return to original positions
- ✅ All abilities (copy/cut/paste) reset to default limits
- ✅ All runtime-generated objects are cleared
- ✅ Player position resets to spawn point

**BUT:**
- ✅ Death count **INCREASES by +1** (persists across reset)
- ✅ Timer **CONTINUES** running (does NOT reset to 0)
- ✅ Best records are **NOT** overwritten

---

## 🏗️ ARCHITECTURE

### **New Component: LevelResetManager**
- Location: `Assets/Script/System/LevelResetManager.cs`
- Type: Singleton (DontDestroyOnLoad)
- Responsibilities:
  1. Handle player death event
  2. Save timer and death count before scene reload
  3. Reload current scene
  4. Restore timer and death count after reload
  5. Reset player abilities

### **Modified Components:**

#### **RespawnManager** (`Assets/Script/System/RespawnManager.cs`)
- **OLD:** Player dies → Teleport to spawn point → Continue playing
- **NEW:** Player dies → LevelResetManager.HandlePlayerDeath() → Scene reload

#### **GameManager** (`Assets/Script/System/GameManager.cs`)
- **Added:** Detection for death-reset scenarios
- **Behavior:** Skip initialization coroutine when resetting from death
- **Preserves:** Timer value and death count across reload

---

## 🔧 SETUP INSTRUCTIONS

### **Step 1: Create LevelResetManager GameObject**

1. Open your **MainMenu** scene (or the first scene in your game)
2. Create an **empty GameObject** in the scene
3. Rename it to: **"LevelResetManager"**
4. Add the **LevelResetManager** component:
   - Click "Add Component"
   - Search for "Level Reset Manager"
   - Click to add

### **Step 2: Configure LevelResetManager**

In the Inspector, configure these settings:

| Field | Value | Description |
|-------|-------|-------------|
| **Death Delay Before Reset** | `1.0` | Delay in seconds before scene reloads (gives time for death animation/effect) |
| **Show Debug Log** | `✓ Checked` | Enable to see debug logs (recommended for testing) |

### **Step 3: Verify Integration**

The system is already integrated with:
- ✅ **RespawnManager** - Already modified to use LevelResetManager
- ✅ **GameManager** - Already modified to detect death resets
- ✅ **PlayerPlatformInteractor** - Has `ResetLimits()` method ready

**No additional code changes needed!**

---

## 🎮 HOW IT WORKS

### **Player Death Flow:**

```
1. Player falls into void or touches hazard
   ↓
2. RespawnManager.Die() called
   ↓
3. LevelResetManager.HandlePlayerDeath()
   ├─ Increment death count (+1)
   ├─ Save current timer value (e.g., 45.3s)
   ├─ Save death count (e.g., 3)
   └─ Set flag: isResettingFromDeath = true
   ↓
4. Wait 1 second (death delay)
   ↓
5. SceneManager.LoadScene(currentScene)
   ↓
   [SCENE RELOADS - EVERYTHING RESETS]
   ├─ All platforms reset to original positions
   ├─ All cut/pasted platforms destroyed
   ├─ Player spawns at RespawnPoint
   └─ All scene objects re-initialized
   ↓
6. OnSceneLoaded event fires
   ↓
7. GameManager detects death reset
   └─ SKIP initialization (prevent timer reset)
   ↓
8. LevelResetManager.RestoreSavedDataAfterFrame()
   ├─ Restore timer value (45.3s → continues)
   ├─ Restore death count (3 → persists)
   ├─ Start timer (continues running)
   └─ Reset player abilities (copy/cut/paste limits)
   ↓
9. Player can play with FRESH level + INCREMENTED death count
```

### **Data Preservation:**

| Data | Behavior on Death |
|------|-------------------|
| **Timer** | ✅ **CONTINUES** - Saved before reload, restored after |
| **Death Count** | ✅ **INCREMENTED** - Increases by +1, persists |
| **Platform Positions** | ❌ **RESET** - All platforms return to original state |
| **Cut Platforms** | ❌ **RESTORED** - Cut platforms reappear |
| **Pasted Platforms** | ❌ **CLEARED** - All pasted platforms destroyed |
| **Ability Limits** | ❌ **RESET** - Copy/cut/paste counters reset to 0 |
| **Player Position** | ❌ **RESET** - Player spawns at RespawnPoint |
| **Best Records** | ✅ **PRESERVED** - Not affected by death |

---

## 🧪 TESTING CHECKLIST

### **Test 1: Basic Death Reset**
1. ✅ Start a level
2. ✅ Note the timer (e.g., 10.5s)
3. ✅ Fall into void (die)
4. ✅ Verify death count increments
5. ✅ Verify level reloads
6. ✅ Verify timer continues (e.g., 11.5s after 1s delay)

### **Test 2: Platform Reset**
1. ✅ Cut a platform (platform disappears)
2. ✅ Paste it somewhere else
3. ✅ Die
4. ✅ Verify ALL platforms are back to original positions
5. ✅ Verify pasted platform is gone
6. ✅ Verify cut platform is restored

### **Test 3: Ability Reset**
1. ✅ Use copy (1/1 used)
2. ✅ Use cut (1/1 used)
3. ✅ Use paste (1/3 used)
4. ✅ Die
5. ✅ Verify copy count = 0/1 (reset)
6. ✅ Verify cut count = 0/1 (reset)
7. ✅ Verify paste count = 0/3 (reset)

### **Test 4: Death Count Persistence**
1. ✅ Start level (deaths = 0)
2. ✅ Die once → deaths = 1
3. ✅ Die again → deaths = 2
4. ✅ Die again → deaths = 3
5. ✅ Complete level
6. ✅ Verify final death count = 3

### **Test 5: Timer Continuity**
1. ✅ Play for 30 seconds
2. ✅ Die (timer should be ~31s after 1s delay)
3. ✅ Play for 20 more seconds
4. ✅ Die again (timer should be ~52s)
5. ✅ Verify timer never resets to 0

---

## 🐛 TROUBLESHOOTING

### **Issue: Level doesn't reset when player dies**

**Symptoms:**
- Player dies but scene doesn't reload
- Platforms don't reset

**Causes:**
1. LevelResetManager GameObject not created
2. LevelResetManager not in DontDestroyOnLoad

**Solution:**
```
1. Check Hierarchy for "LevelResetManager" GameObject
2. Check Console for error: "LevelResetManager not found!"
3. Create LevelResetManager GameObject in MainMenu scene
4. Make sure it has LevelResetManager component attached
```

---

### **Issue: Timer resets to 0 on death**

**Symptoms:**
- Player dies → Timer goes back to 0
- Timer doesn't continue

**Causes:**
1. GameManager's OnSceneLoaded not detecting death reset
2. LevelResetManager not restoring timer value

**Solution:**
```
Check Console for these logs:
- "[LevelResetManager] Saved state for reload: Timer: XX.XXs"
- "[GameManager] Scene loaded from death reset"
- "[LevelResetManager] ✅ Data restored: Timer: XX.XXs (CONTINUING)"

If missing, check:
- LevelResetManager.Instance is not null
- isResettingFromDeath flag is being set
```

---

### **Issue: Death count doesn't increment**

**Symptoms:**
- Player dies multiple times but death count stays 0
- Death count resets on death

**Causes:**
1. IncrementDeaths() not called before reload
2. Death count not saved before reload
3. Death count not restored after reload

**Solution:**
```
Check Console for:
- "[LevelResetManager] Saved state: Death Count: X"
- "[GameManager] 💀 Player died! Total deaths: X"
- "[LevelResetManager] ✅ Data restored: Death Count: X"

Verify in GameManager:
- deathCount field is public (can be modified)
- IncrementDeaths() is being called
```

---

### **Issue: Abilities don't reset**

**Symptoms:**
- Player dies but copy/cut/paste limits stay consumed
- Can't use abilities after death

**Causes:**
1. PlayerPlatformInteractor.ResetLimits() not called
2. PlayerPlatformInteractor not found after reload

**Solution:**
```
Check Console for:
- "[LevelResetManager] ✅ Player abilities reset"

If warning appears:
- "PlayerPlatformInteractor not found - abilities not reset!"

Solution:
- Verify Player GameObject has PlayerPlatformInteractor component
- Component must be on scene player, not prefab
```

---

### **Issue: Double initialization or timer starts late**

**Symptoms:**
- Timer starts from 0 after death
- Intro camera plays again after death
- Cutscene plays again after death

**Causes:**
1. GameManager not detecting death reset
2. InitializeTimer coroutine running when it shouldn't

**Solution:**
```
GameManager.OnSceneLoaded() should detect death reset:

Check this log appears:
"[GameManager] Scene loaded from death reset: levelX - LevelResetManager will handle initialization"

If you see:
"[GameManager] Scene loaded: levelX - hasInitialized reset to false"

Then the detection is failing. Verify:
- LevelResetManager.Instance exists
- IsResettingFromDeath() returns true during reload
```

---

## 📊 DEBUG LOGS REFERENCE

### **Normal Death Reset Sequence:**

```
[LevelResetManager] Player death triggered. Death count before increment: 0
[GameManager] 💀 Player died! Total deaths: 1
[LevelResetManager] Saved state for reload:
  - Timer: 15.43s
  - Death Count: 1
[LevelResetManager] Reloading scene 'level1' to reset level...
[GameManager] Scene loaded from death reset: level1 - LevelResetManager will handle initialization
[LevelResetManager] Scene 'level1' loaded after death. Restoring saved data...
[LevelResetManager] ✅ Data restored after level reset:
  - Timer: 15.43s (CONTINUING)
  - Death Count: 1
  - All platforms, abilities, and objects RESET to initial state
Operation limits reset!
[LevelResetManager] ✅ Player abilities reset (copy/cut/paste limits cleared)
```

---

## 🎯 DESIGN DECISIONS

### **Why Scene Reload Instead of Manual Reset?**

**Advantages:**
- ✅ **Complete reset** - No need to track every object manually
- ✅ **Foolproof** - New objects added to level auto-reset
- ✅ **Consistent state** - Identical to first level load
- ✅ **No bugs** - Avoids "forgot to reset X" issues
- ✅ **Simple code** - Less complex than manual tracking

**Disadvantages:**
- ⚠️ Brief loading time (minimal, ~0.1-0.5s for small levels)
- ⚠️ Requires careful data preservation (timer, deaths)

**Conclusion:** Scene reload is the **safest and most reliable** approach for full level reset.

---

### **Why Use Static Variables for Saved Data?**

**Alternative Options:**
1. **PlayerPrefs** - Too slow, overkill for temporary data
2. **Scriptable Object** - Unnecessary overhead, needs asset creation
3. **Static variables** - ✅ **CHOSEN** - Fast, simple, perfect for temporary data

**Important:** Static data survives scene reloads but NOT application quit. This is EXACTLY what we need.

---

## ✅ FINAL CHECKLIST

Before considering the system complete:

- [x] LevelResetManager created and configured
- [x] RespawnManager updated to use LevelResetManager
- [x] GameManager updated to detect death resets
- [x] PlayerPlatformInteractor has ResetLimits() method
- [ ] Tested basic death reset (platforms reset)
- [ ] Tested ability reset (copy/cut/paste limits)
- [ ] Tested death count increment (increases by 1)
- [ ] Tested timer continuity (doesn't reset)
- [ ] Tested multiple deaths in a row
- [ ] Tested level completion after deaths
- [ ] Verified best records are not affected

---

## 🎓 CONCLUSION

The Level Reset System provides a **clean slate** experience on every death while **preserving progress** (timer and death count). This creates a fair and consistent gameplay experience where players can retry levels without carrying over mistakes, but their performance metrics (time and deaths) are accurately tracked.

**Key Benefits:**
1. ✅ Players get a fresh level on every death
2. ✅ Death count accurately reflects total attempts
3. ✅ Timer accurately reflects total time spent
4. ✅ No weird bugs from incomplete resets
5. ✅ Easy to maintain and extend

---

**Questions?** Check the troubleshooting section or review the code comments in [LevelResetManager.cs](Assets/Script/System/LevelResetManager.cs).
