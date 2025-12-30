# 🎮 ARGYROS PLATFORMER - COMPLETE GAME FLOW DOCUMENTATION

**Generated:** December 30, 2025  
**Project:** ArgyrosPlatformer  
**Type:** 3D Platformer with Novel Cutscene System

---

## 📊 TABLE OF CONTENTS
1. [Scene Overview](#scene-overview)
2. [Main Flow Diagram](#main-flow-diagram)
3. [Detailed Scene-by-Scene Flow](#detailed-scene-by-scene-flow)
4. [Button Mapping Reference](#button-mapping-reference)
5. [Special Systems](#special-systems)
6. [Level Progression Flow](#level-progression-flow)

---

## 🗺️ SCENE OVERVIEW

### **UI Scenes** (13 scenes)
Located in: `Assets/Scenes/UI/`

| Scene Name | Type | Purpose |
|------------|------|---------|
| **MainMenu** | Entry Point | Main menu dengan Continue/New Game/Achievement/Settings/Exit |
| **SelectLevel** | Level Hub | Pilih level untuk dimainkan (Level 1-10) |
| **Settings** | Configuration | Music toggle, Guide button |
| **Guide** | Information | Tutorial/panduan cara bermain |
| **Complete** | Victory Screen | Tampil setelah menang - Main Menu/Retry/Next Level |
| **AchievementGlobal** | Achievement Hub | Achievement overview |
| **Achievement1** | Achievement Page | Achievement kategori 1 |
| **Achievement2** | Achievement Page | Achievement kategori 2 |
| **Achievement3** | Achievement Page | Achievement kategori 3 |
| **Restart** | Confirmation | Konfirmasi restart level (Yes/No) |
| **Exit** | Confirmation | Konfirmasi exit game (Yes/No) |
| **TutorialCompleted** | Info Screen | (Purpose TBD) |

### **Level Scenes** (2 scenes + 10 levels planned)
Located in: `Assets/Scenes/level/`

| Scene Name | Type | Purpose |
|------------|------|---------|
| **tutorial** | Tutorial Level | Level tutorial untuk first-time players |
| **level1** | Gameplay Level | Level 1 (implemented) |
| **level2-10** | Gameplay Levels | Levels 2-10 (planned in Build Settings) |

### **Dynamic Scene**
| Scene Name | Type | Purpose |
|------------|------|---------|
| **Pause** | Overlay | Pause menu (loaded additively during gameplay) |

**Total Scenes:** 15+ scenes

---

## 🎯 MAIN FLOW DIAGRAM

```
┌─────────────────────────────────────────────────────────────────────┐
│                        GAME START                                    │
│                            ↓                                         │
│                     ┌──────────────┐                                │
│                     │  MainMenu    │ ← Entry Point                  │
│                     └──────────────┘                                │
│                            │                                         │
│         ┌──────────────────┼──────────────────┬──────────────┐     │
│         ↓                  ↓                  ↓              ↓      │
│   [Continue]          [New Game]        [Achievement]  [Settings]   │
│         │                  │                  │              │      │
│         ↓                  ↓                  ↓              ↓      │
│   Load Save          Delete All       AchievementGlobal  Settings   │
│         │             Create Save            │              │      │
│         └──────────┬───────┘                 │              │      │
│                    ↓                          │              │      │
│              ┌─────────────┐                 │              │      │
│              │ SelectLevel │ ← Level Hub     │              │      │
│              └─────────────┘                 │              │      │
│                    │                          │              │      │
│         ┌──────────┼──────────┐              │              │      │
│         ↓          ↓          ↓              ↓              ↓      │
│    [Level 1] [Level 2] ... [Level 10]   [Ach1-3]      [Guide]     │
│         │          │          │              │              │      │
│         └──────────┼──────────┘              │              │      │
│                    ↓                          │              │      │
│            ┌───────────────┐                 │              │      │
│            │  GAMEPLAY     │                 │              │      │
│            │  (Level X)    │                 │              │      │
│            └───────────────┘                 │              │      │
│                    │                          │              │      │
│         ┌──────────┼──────────┐              │              │      │
│         ↓          ↓          ↓              ↓              ↓      │
│     [ESC]       [Die]      [Win]         [Back]        [Back]     │
│         │          │          │              │              │      │
│         ↓          ↓          ↓              └──────┬───────┘      │
│      Pause    Respawn    Complete                   │              │
│         │          │          │                      │              │
│         ↓          │          ↓                      │              │
│  [Resume/Restart/  └──→   [MainMenu]                │              │
│   MainMenu]               [Retry]                    │              │
│         │                 [Next Level]               │              │
│         └─────────────────────┼───────────────────────┘             │
│                               ↓                                     │
│                          MainMenu                                   │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 📖 DETAILED SCENE-BY-SCENE FLOW

### 🏠 **1. MainMenu Scene**

**Purpose:** Entry point game, pilih mode bermain

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **ContinueButton** | `OnContinueButtonClicked()` | → **SelectLevel** | Load save data, enabled jika ada save file |
| **NewgameButton** | `OnNewGameButtonClicked()` | → **SelectLevel** | Delete all data, create fresh save, only Level 1 unlocked |
| **AchievementButton** | `OnAchievementButtonClicked()` | → **AchievementGlobal** | View achievements |
| **SettingsButton** | `OnSettingsButtonClicked()` | → **Settings** | Open settings menu |
| **ExitgameButton** | `OnExitGameButtonClicked()` | → **Quit Game** | Exit application (direct quit, no confirmation) |

**Flow Details:**

**Continue Button:**
```
MainMenu → Continue
  ↓
Load SaveSystem.LoadGame()
  ↓
Set GameManager.currentSave
  ↓
Go to SelectLevel (NOT last level anymore!)
  ↓
Player chooses level
```

**New Game Button:**
```
MainMenu → New Game
  ↓
SaveSystem.DeleteAllData() (TOTAL RESET)
  ├─ Delete save file
  ├─ Delete BestTime_Level1-10
  ├─ Delete BestDeaths_Level1-10
  └─ Delete all cutscene flags
  ↓
Create new GameSaveData
  ├─ lastPlayedLevel = 1
  └─ unlockedLevels[0] = true (only Level 1)
  ↓
SaveSystem.SaveGame(newSave)
  ↓
Go to SelectLevel
```

**Special Behaviors:**
- Continue button state managed by `MainMenuController.cs`
- Button disabled (or hidden) jika tidak ada save file
- Audio: Background music plays via AudioManager

---

### 🎮 **2. SelectLevel Scene**

**Purpose:** Hub untuk memilih level mana yang ingin dimainkan

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **BackButton** | `OnBackButtonClicked()` | → **MainMenu** | Return to main menu |
| **Level1-10 Buttons** | `LevelButtonHandler.OnLevelButtonClicked()` | → **level{X}** | Load selected level |

**Level Button Logic:**
```
SelectLevel → Click Level Button
  ↓
Check if level unlocked (GameManager.currentSave.unlockedLevels[])
  ├─ If LOCKED → Button disabled, lock icon shown
  └─ If UNLOCKED → Proceed
      ↓
  Check if level needs cutscene (needsCutsceneOnFirstPlay)
      ├─ If YES & NOT played before
      │   └─ Set PlayerPrefs flags (ShouldPlayCutscene, CutsceneLevel)
      └─ If NO or already played → Skip
      ↓
  Set GameManager.currentLevelIndex
      ↓
  Time.timeScale = 1f (safety reset)
      ↓
  SceneManager.LoadScene("level{X}")
```

**Visual Feedback:**
- Locked levels: Lock icon visible, button interactable = false
- Unlocked levels: Lock icon hidden, button interactable = true
- Best Time/Deaths displayed below level buttons (from PlayerPrefs)

**Special Features:**
- `LevelButtonHandler.cs` manages each level button individually
- Auto-reads save data untuk unlock status
- Displays best records (time & deaths) untuk completed levels

---

### 🎬 **3. Level Scene (level1, level2, etc.)**

**Purpose:** Main gameplay - platforming, collecting, winning

**Entry Flow:**
```
Level Load
  ↓
GameManager.Awake() - DontDestroyOnLoad
  ↓
Check PlayerPrefs: ShouldPlayCutscene?
  ├─ YES → NovelCutsceneManager.PlayCutscene()
  │         ├─ Time.timeScale = 0f (pause game)
  │         ├─ Play cutscene BGM
  │         ├─ Display dialogue
  │         └─ On finish: Time.timeScale = 1f, resume level BGM
  │         └─ Set PlayerPrefs: CutscenePlayed_Level{X} = 1
  └─ NO → Skip cutscene
  ↓
GameManager.WaitForCutsceneEnd()
  ↓
GameManager.StartTimer() (auto-start after cutscene or 0.5s delay)
  ↓
GAMEPLAY ACTIVE
```

**During Gameplay:**

**Input: ESC Key**
```
ESC pressed
  ↓
PauseManager.Pause()
  ├─ Time.timeScale = 0f
  ├─ Stop timer
  ├─ Pause BGM
  ├─ Hide UI (GuideButton, MusicButton, DeathTimerUI)
  └─ Show Pause Menu (load additively? or panel SetActive)
```

**Player Dies (Fall, Enemy, Hazard):**
```
Death Trigger
  ↓
GameManager.PlayerDeath()
  ├─ Increment deathCount
  ├─ Update DeathTimerUI
  ├─ Play death animation/effect
  └─ Respawn at checkpoint
  ↓
Continue gameplay (timer keeps running)
```

**Player Reaches Goal (WinningTrigger):**
```
Player collides with WinningTrigger
  ↓
WinningTrigger.OnTriggerEnter()
  ├─ Freeze player (canMove = false)
  ├─ Stop timer
  ├─ Play win animation
  ├─ Play win audio
  ├─ Camera zoom effect
  ├─ Glow effect on platform
  └─ Wait {transitionDelay} seconds
  ↓
GameManager.CompleteLevel()
  ├─ Save time & deaths to PlayerPrefs
  ├─ Check if new best record
  ├─ Unlock next level (currentSave.unlockedLevels[nextLevel] = true)
  ├─ Save progress via SaveSystem.SaveGame()
  └─ Load Complete scene
  ↓
WinningTrigger.LoadCompleteScene()
  ├─ Time.timeScale = 1f (safety reset)
  └─ SceneManager.LoadScene("Complete")
```

**Special Systems Active:**
- **DeathTimerUI:** Display current time & death count (top-left corner)
- **GuideButton:** Quick access to guide (top-right)
- **MusicButton:** Toggle music on/off (top-right)
- **PauseManager:** ESC to pause, manage pause state
- **GameManager:** Track timer, deaths, level progression
- **AudioManager:** Background music & SFX

---

### ⏸️ **4. Pause Menu (Overlay)**

**Purpose:** Pause game, provide options untuk resume/restart/exit

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **Resume** / **ContinueButton** | `OnContinueButtonClicked()` (Pause context) | → Resume Gameplay | Unload Pause scene |
| **RestartButton** | `OnRestartButtonClicked()` (Pause context) | → Reload Current Level | Reset timer & deaths |
| **SettingsButton** | `OnSettingsButtonClicked()` | → **Settings** | Open settings |
| **MainmenuButton** / **HomeButton** | `OnMainMenuButtonClicked()` | → **MainMenu** | Return to main menu |
| **ExitgameButton** | `OnExitGameButtonClicked()` | → **Exit** (confirmation) | NOT direct quit from pause |

**Flow Details:**

**Resume:**
```
Pause → Resume
  ↓
PauseManager.Resume()
  ├─ Time.timeScale = 1f
  ├─ Resume timer
  ├─ Resume BGM
  ├─ Show UI (GuideButton, MusicButton, DeathTimerUI)
  └─ Hide Pause Menu
  ↓
Return to gameplay
```

**Restart:**
```
Pause → Restart
  ↓
SceneController.OnRestartButtonClicked()
  ├─ Time.timeScale = 1f
  └─ SceneManager.LoadScene("level1") ← HARDCODED level1!
  ↓
Level reloads (timer & deaths reset)
```

**Main Menu:**
```
Pause → Main Menu
  ↓
SceneController.OnMainMenuButtonClicked()
  ├─ Time.timeScale = 1f
  └─ LoadScene("MainMenu")
```

**Exit (from Pause):**
```
Pause → Exit
  ↓
LoadScene("Exit") ← Confirmation scene
  ↓
[Yes/No buttons in Exit scene]
```

**Special Behaviors:**
- UI elements hidden during pause (via PauseManager)
- BGM paused (not stopped, can resume)
- Time.timeScale = 0f (freezes game)

---

### 🏆 **5. Complete Scene**

**Purpose:** Tampil setelah player win level, show stats & provide next actions

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **MainmenuButton** | `CompleteMenuController.OnMainMenuClicked()` | → **MainMenu** | Return to main menu |
| **RetryButton** | `CompleteMenuController.OnRetryClicked()` | → **level{current}** | Retry same level |
| **NextlevelButton** | `CompleteMenuController.OnNextLevelClicked()` | → **level{current+1}** or **MainMenu** | Next level or end if last level |

**Flow Details:**

**Main Menu Button:**
```
Complete → Main Menu
  ↓
CompleteMenuController.OnMainMenuClicked()
  ├─ Time.timeScale = 1f (safety reset)
  └─ SceneManager.LoadScene("MainMenu")
```

**Retry Button:**
```
Complete → Retry
  ↓
CompleteMenuController.OnRetryClicked()
  ├─ GameManager.ResetLevel() (reset timer & deaths)
  ├─ Time.timeScale = 1f
  └─ SceneManager.LoadScene("level{currentLevel}")
  ↓
Level reloads (fresh attempt)
```

**Next Level Button:**
```
Complete → Next Level
  ↓
Check currentLevel < TOTAL_LEVELS (10)
  ├─ YES → Proceed
  │   ├─ Set currentLevelIndex = nextLevel
  │   ├─ GameManager.ResetLevel()
  │   ├─ Time.timeScale = 1f
  │   └─ SceneManager.LoadScene("level{nextLevel}")
  └─ NO (last level) → Go to MainMenu
```

**UI Display:**
- **CurrentTimeText:** Time penyelesaian level saat ini
- **CurrentDeathsText:** Jumlah kematian saat ini
- **BestTimeText:** Best time dari PlayerPrefs (jika ada)
- **BestDeathsText:** Best deaths dari PlayerPrefs (jika ada)
- **New Record Indicator:** Jika current stats lebih baik dari best

**Data Saved:**
- PlayerPrefs: `BestTime_Level{X}`, `BestDeaths_Level{X}`
- SaveSystem: `currentSave.unlockedLevels[]`, `lastPlayedLevel`

---

### ⚙️ **6. Settings Scene**

**Purpose:** Configure game options (music toggle, access guide)

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **BackButton** | `OnBackButtonClicked()` | → **MainMenu** | Return to main menu |
| **GuideButton** | `OnGuideButtonClicked()` | → **Guide** | Open guide |
| **MusicButton** | `MusicToggleButton.OnMusicButtonClicked()` | (Toggle State) | Toggle music on/off |

**Flow:**
```
Settings → Back → MainMenu
Settings → Guide → Guide scene
Settings → Music → (Toggle audio state, stay in Settings)
```

**Special Features:**
- `MusicToggleButton.cs` manages music toggle independently
- Visual feedback: Icon changes based on music state
- Persists across scenes (likely via PlayerPrefs or AudioManager)

---

### 📚 **7. Guide Scene**

**Purpose:** Tutorial/panduan cara bermain

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **BackButton** | `OnBackButtonClicked()` | → **Settings** | Return to settings (NOT MainMenu!) |

**Flow:**
```
Guide → Back → Settings
```

---

### 🏅 **8. Achievement Scenes (AchievementGlobal, Achievement1-3)**

**Purpose:** Display player achievements

**Navigation Pattern (Circular):**
```
AchievementGlobal ⇄ Achievement1 ⇄ Achievement2 ⇄ Achievement3 ⇄ (loop)
```

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **BackButton** | `OnBackButtonClicked()` | → **MainMenu** | Exit achievements |
| **Next** (or arrow) | `OnNextAchievementClicked()` | → Next achievement page | Circular navigation |
| **Back** (navigation) | `OnBackAchievementClicked()` | → Previous achievement page | Circular navigation |

**Navigation Logic:**

**Next Button:**
```
AchievementGlobal → Next → Achievement1
Achievement1      → Next → Achievement2
Achievement2      → Next → Achievement3
Achievement3      → Next → AchievementGlobal (loop back)
```

**Back Button (navigation, NOT exit):**
```
AchievementGlobal → Back → Achievement3
Achievement3      → Back → Achievement2
Achievement2      → Back → Achievement1
Achievement1      → Back → AchievementGlobal (loop)
```

**Exit:**
```
Any Achievement → BackButton → MainMenu
```

---

### ❌ **9. Exit Confirmation Scene**

**Purpose:** Confirm player wants to exit to main menu (from Pause)

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **YesButton** | `OnYesButtonClicked()` | → **MainMenu** | Confirm exit |
| **NoButton** | `OnNoButtonClicked()` | → **Pause** | Cancel, back to pause |

**Flow:**
```
Exit → Yes → MainMenu
Exit → No → Pause
```

**Context:**
- Only accessible FROM Pause menu (ExitgameButton in Pause)
- MainMenu has direct quit (no confirmation)

---

### 🔄 **10. Restart Confirmation Scene**

**Purpose:** Confirm player wants to restart level (currently NOT IMPLEMENTED)

**Buttons Available:**
| Button | Handler | Destination | Notes |
|--------|---------|-------------|-------|
| **YesButton** | `OnYesButtonClicked()` | → (TODO) Restart level | Not implemented |
| **NoButton** | `OnNoButtonClicked()` | → **Pause** | Cancel |

**Current Status:** Scene exists but logic incomplete

---

## 🔘 BUTTON MAPPING REFERENCE

### **Universal Buttons (SceneController Auto-Detection)**

| Button GameObject Name | Method Handler | Available In Scenes |
|------------------------|---------------|---------------------|
| `ContinueButton` | `OnContinueButtonClicked()` | MainMenu, Pause |
| `NewgameButton` | `OnNewGameButtonClicked()` | MainMenu |
| `AchievementButton` | `OnAchievementButtonClicked()` | MainMenu, Pause |
| `SettingsButton` | `OnSettingsButtonClicked()` | MainMenu, Pause |
| `ExitgameButton` | `OnExitGameButtonClicked()` | MainMenu, Pause |
| `BackButton` | `OnBackButtonClicked()` | Settings, Guide, SelectLevel, Achievements |
| `MainmenuButton` / `HomeButton` | `OnMainMenuButtonClicked()` | Complete, Pause, (others) |
| `RestartButton` | `OnRestartButtonClicked()` | Pause |
| `NextlevelButton` | `OnNextLevelButtonClicked()` | Complete |
| `GuideButton` | `OnGuideButtonClicked()` | Settings |
| `YesButton` | `OnYesButtonClicked()` | Exit, Restart |
| `NoButton` | `OnNoButtonClicked()` | Exit, Restart |
| `Next` | `OnNextAchievementClicked()` | Achievements |
| `Back` (achievement nav) | `OnBackAchievementClicked()` | Achievements |

### **Special Buttons (Custom Controllers)**

| Button | Controller | Method | Scene |
|--------|-----------|--------|-------|
| Main Menu Button | `CompleteMenuController` | `OnMainMenuClicked()` | Complete |
| Retry Button | `CompleteMenuController` | `OnRetryClicked()` | Complete |
| Next Level Button | `CompleteMenuController` | `OnNextLevelClicked()` | Complete |
| Level 1-10 Buttons | `LevelButtonHandler` | `OnLevelButtonClicked()` | SelectLevel |
| Music Toggle | `MusicToggleButton` | `OnMusicButtonClicked()` | Settings, Gameplay |

---

## ⚙️ SPECIAL SYSTEMS

### **1. Save System**

**Purpose:** Persist player progress across sessions

**Data Stored:**
- `lastPlayedLevel` (int)
- `unlockedLevels[]` (bool array, 10 levels)
- `completedLevels[]` (bool array)
- `achievements[]` (bool array, 20 achievements)

**Key Methods:**
- `SaveSystem.SaveGame(GameSaveData)` - Save to PlayerPrefs (JSON)
- `SaveSystem.LoadGame()` - Load from PlayerPrefs
- `SaveSystem.DeleteSave()` - Delete save file only
- `SaveSystem.DeleteAllData()` - TOTAL RESET (save + best records + flags)
- `SaveSystem.HasSaveFile()` - Check if save exists

**PlayerPrefs Keys:**
- `SAVE_GAME_DATA` - Main save file (JSON)
- `SAVE_EXISTS_KEY` - Quick check flag
- `BestTime_Level{X}` - Best time for each level (1-10)
- `BestDeaths_Level{X}` - Best deaths for each level (1-10)
- `CutscenePlayed_Level{X}` - Flag if cutscene already played
- `ShouldPlayCutscene` - Trigger flag for next scene
- `CutsceneLevel` - Which level's cutscene to play

---

### **2. Novel Cutscene System**

**Purpose:** Display visual novel style cutscenes before levels

**Flow:**
```
Level Load
  ↓
Check PlayerPrefs: ShouldPlayCutscene == 1
  ├─ YES → Get CutsceneLevel number
  │   ↓
  │   NovelCutsceneManager.PlayCutscene(DialogueSceneData)
  │   ├─ Time.timeScale = 0f (freeze game)
  │   ├─ Stop level BGM
  │   ├─ Play cutscene BGM (via SFX channel)
  │   ├─ Show cutscene canvas
  │   ├─ Display background sprite
  │   ├─ Typing effect for dialogue text
  │   ├─ Wait for player input (click/space)
  │   └─ On finish:
  │       ├─ Time.timeScale = 1f
  │       ├─ Resume level BGM
  │       ├─ Hide cutscene canvas
  │       └─ Set CutscenePlayed_Level{X} = 1
  └─ NO → Skip cutscene, start level normally
```

**Key Features:**
- Typing animation for dialogue
- Background images
- Character names
- SFX for typing sound
- Input handling (click/space to advance)
- Auto-pause game during cutscene

**Scripts:**
- `NovelCutsceneManager.cs` - Main controller
- `DialogueSceneData.cs` - Data structure for cutscene
- Integration with GameManager for auto-start timer

---

### **3. Audio System**

**Purpose:** Manage background music & sound effects

**Components:**
- **AudioManager.cs** - Singleton, manages all audio
- **MusicToggleButton.cs** - UI control for music on/off

**Key Features:**
- Level BGM (loops)
- Cutscene BGM (separate channel)
- SFX (unaffected by pause/timescale)
- Pause/Resume BGM capability
- Persist across scenes (DontDestroyOnLoad)

**Usage in Flow:**
- Level start → Play level BGM
- Cutscene start → Stop level BGM, play cutscene BGM
- Cutscene end → Stop cutscene BGM, resume level BGM
- Pause → Pause BGM
- Resume → Resume BGM
- Complete → (BGM behavior TBD)

---

### **4. Timer & Death Counter**

**Purpose:** Track player performance metrics

**Display:**
- `DeathTimerUI.cs` - Top-left corner display
  - Timer format: MM:SS (minutes:seconds)
  - Death count: Number

**Behavior:**
- Timer starts: After cutscene ends (or 0.5s delay if no cutscene)
- Timer stops: When player reaches WinningTrigger
- Deaths increment: Each time player dies (respawn)
- Data saved: On level complete to PlayerPrefs

**Auto-Hide on Pause:**
- DeathTimerUI hidden when pause menu active
- Shown again on resume

---

### **5. Level Unlock System**

**Purpose:** Gate level access based on progression

**Logic:**
```
New Game:
  ├─ Only Level 1 unlocked
  └─ All others locked

Complete Level:
  ├─ Unlock next level (currentLevel + 1)
  └─ Save to GameSaveData.unlockedLevels[]

SelectLevel Display:
  ├─ Locked levels: Button disabled, lock icon visible
  └─ Unlocked levels: Button enabled, lock icon hidden
```

**Visual Feedback:**
- Lock icon overlay on locked level buttons
- Grayed out or disabled state
- Best records only shown for completed levels

**Script:** `LevelButtonHandler.cs`

---

### **6. Pause System**

**Purpose:** Freeze game, provide options

**Trigger:** ESC key during gameplay

**Effects:**
- `Time.timeScale = 0f` - Freeze physics/animations
- Stop timer
- Pause BGM
- Hide UI elements (Guide, Music, DeathTimer)
- Show pause menu panel/scene

**Resume:**
- `Time.timeScale = 1f`
- Resume timer
- Resume BGM
- Show UI elements
- Hide pause menu

**Script:** `PauseManager.cs`

---

## 🎮 LEVEL PROGRESSION FLOW

### **Complete Player Journey (First-Time)**

```
┌─────────────────────────────────────────────────────────────────────┐
│ DAY 1: First Time Playing                                           │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│ 1. Launch Game → MainMenu                                          │
│    ├─ Continue button: DISABLED (no save file)                     │
│    └─ New Game button: ENABLED                                     │
│                                                                      │
│ 2. Click "New Game"                                                │
│    ├─ Delete all data (nothing exists yet)                         │
│    ├─ Create fresh save (Level 1 only unlocked)                    │
│    └─ Go to SelectLevel                                            │
│                                                                      │
│ 3. SelectLevel Scene                                               │
│    ├─ Level 1: UNLOCKED ✅                                         │
│    ├─ Level 2-10: LOCKED 🔒                                        │
│    └─ Click Level 1                                                │
│                                                                      │
│ 4. Level 1 Loads                                                   │
│    ├─ Check cutscene flag: NOT played yet                          │
│    ├─ Play Level 1 cutscene                                        │
│    │   ├─ Time.timeScale = 0f (game frozen)                        │
│    │   ├─ Display story/tutorial dialogue                          │
│    │   └─ Player clicks through dialogue                           │
│    ├─ Cutscene ends                                                │
│    │   ├─ Time.timeScale = 1f                                      │
│    │   ├─ Set CutscenePlayed_Level1 = 1                           │
│    │   └─ Timer auto-starts                                        │
│    └─ GAMEPLAY BEGINS                                              │
│                                                                      │
│ 5. Playing Level 1                                                 │
│    ├─ DeathTimerUI shows: 00:00 timer, 0 deaths                   │
│    ├─ Player jumps, platforms, avoids hazards                      │
│    ├─ Player dies 3 times → Death count: 3                         │
│    ├─ Player reaches goal in 02:45 (2m 45s)                        │
│    └─ WinningTrigger activates                                     │
│                                                                      │
│ 6. Level Complete                                                  │
│    ├─ GameManager.CompleteLevel()                                  │
│    │   ├─ Save time: 02:45 → PlayerPrefs BestTime_Level1          │
│    │   ├─ Save deaths: 3 → PlayerPrefs BestDeaths_Level1          │
│    │   ├─ Unlock Level 2 → unlockedLevels[1] = true              │
│    │   └─ Save progress → SaveSystem.SaveGame()                    │
│    └─ Load Complete scene                                          │
│                                                                      │
│ 7. Complete Scene                                                  │
│    ├─ Display: Current Time: 02:45, Deaths: 3                     │
│    ├─ Display: Best Time: 02:45 (NEW!), Best Deaths: 3 (NEW!)     │
│    ├─ Buttons available:                                           │
│    │   ├─ [Main Menu] → MainMenu                                  │
│    │   ├─ [Retry] → Reload Level 1                                │
│    │   └─ [Next Level] → Load Level 2                             │
│    └─ Player clicks "Next Level"                                   │
│                                                                      │
│ 8. Level 2 Loads                                                   │
│    ├─ Check cutscene flag: NOT played yet                          │
│    ├─ Play Level 2 cutscene (if configured)                        │
│    └─ ... (repeat gameplay cycle)                                 │
│                                                                      │
│ 9. After Playing 3 Levels                                          │
│    ├─ Player clicks Main Menu from Complete                        │
│    └─ Go to MainMenu                                               │
│                                                                      │
│ 10. MainMenu (Save Exists Now)                                    │
│     ├─ Continue button: ENABLED ✅                                 │
│     ├─ Save file exists (Level 1-3 unlocked)                       │
│     └─ Player closes game                                          │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│ DAY 2: Returning Player                                             │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│ 1. Launch Game → MainMenu                                          │
│    ├─ Continue button: ENABLED ✅ (save exists)                    │
│    └─ Click "Continue"                                             │
│                                                                      │
│ 2. Continue Flow                                                    │
│    ├─ Load save data                                               │
│    │   ├─ lastPlayedLevel = 3                                      │
│    │   └─ unlockedLevels = [1,2,3 unlocked, 4-10 locked]          │
│    ├─ Set GameManager.currentSave                                  │
│    └─ Go to SelectLevel (NOT level 3 directly!)                    │
│                                                                      │
│ 3. SelectLevel Scene                                               │
│    ├─ Level 1: UNLOCKED, Best Time: 02:45, Deaths: 3              │
│    ├─ Level 2: UNLOCKED, Best Time: 03:12, Deaths: 5              │
│    ├─ Level 3: UNLOCKED, Best Time: 04:01, Deaths: 7              │
│    ├─ Level 4-10: LOCKED 🔒                                        │
│    └─ Player can choose ANY unlocked level                         │
│        ├─ Want to replay Level 1? ✅ Allowed                       │
│        ├─ Want to try Level 2 again? ✅ Allowed                    │
│        └─ Want to continue Level 4? ❌ Locked (must complete 3)    │
│                                                                      │
│ 4. Player Replays Level 1                                          │
│    ├─ NO cutscene (already played, flag set)                       │
│    ├─ Timer starts immediately                                     │
│    ├─ Player completes in 02:20 with 1 death                       │
│    └─ Complete scene:                                              │
│        ├─ Current: 02:20, 1 death                                  │
│        ├─ Best: 02:20 (NEW RECORD!), 1 death (NEW RECORD!)         │
│        └─ PlayerPrefs updated with new best                        │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

---

### **Speedrun / Challenge Flow**

```
Player wants to speedrun Level 5 for best time:

SelectLevel → Level 5
  ↓
Gameplay (try 1): 03:45, 8 deaths
  ↓
Complete → Retry
  ↓
Gameplay (try 2): 03:20, 5 deaths (NEW BEST TIME!)
  ↓
Complete → Retry
  ↓
Gameplay (try 3): 03:15, 2 deaths (NEW BEST TIME & DEATHS!)
  ↓
Complete → Main Menu
  ↓
MainMenu → Continue → SelectLevel
  ↓
Level 5 button now shows: Best Time 03:15, Deaths: 2
```

---

### **Achievement Hunting Flow**

```
MainMenu → Achievement
  ↓
AchievementGlobal (overview)
  ├─ "Complete all levels": 3/10 ⏳
  ├─ "Speedrun master": 0/10 ❌
  └─ "No death challenge": 1/10 ⏳
  ↓
Click Next → Achievement1 (detail page)
  ├─ Achievement unlocked ✅
  ├─ Achievement locked 🔒
  └─ Progress bars, icons, descriptions
  ↓
Click Next → Achievement2
  ↓
Click Next → Achievement3
  ↓
Click Next → AchievementGlobal (loop back)
  ↓
BackButton → MainMenu
```

---

## 🔄 SPECIAL FLOWS

### **Pause During Gameplay**

```
Level X (Playing)
  ↓
ESC pressed
  ↓
PauseManager.Pause()
  ├─ Time.timeScale = 0f
  ├─ Timer stops
  ├─ BGM pauses
  ├─ Hide DeathTimerUI, GuideButton, MusicButton
  └─ Show Pause Menu
  ↓
Pause Menu Options:
  ├─ [Resume] → Resume gameplay
  ├─ [Restart] → Reload level (fresh attempt)
  ├─ [Settings] → Settings scene (can access Guide)
  ├─ [Main Menu] → MainMenu (exit level)
  └─ [Exit] → Exit confirmation → Yes: MainMenu, No: Back to Pause
```

---

### **Death & Respawn**

```
Player falls off platform
  ↓
Death trigger (fall zone, hazard, enemy)
  ↓
GameManager.PlayerDeath()
  ├─ Increment deathCount
  ├─ Update DeathTimerUI (show new death count)
  ├─ Play death animation/effect
  └─ Respawn at checkpoint (or start)
  ↓
Gameplay continues (timer keeps running)
```

---

### **New Game (Total Reset)**

```
MainMenu → New Game (with existing save)
  ↓
SceneController.OnNewGameButtonClicked()
  ├─ SaveSystem.DeleteAllData()
  │   ├─ Delete SAVE_GAME_DATA
  │   ├─ Delete BestTime_Level1-10
  │   ├─ Delete BestDeaths_Level1-10
  │   ├─ Delete CutscenePlayed_Level1-10
  │   └─ Delete all flags
  ├─ Create fresh GameSaveData
  │   ├─ lastPlayedLevel = 1
  │   └─ unlockedLevels[0] = true (only Level 1)
  ├─ SaveSystem.SaveGame(newSave)
  └─ Load SelectLevel
  ↓
SelectLevel
  ├─ Only Level 1 unlocked
  ├─ No best times shown (all deleted)
  └─ Fresh start experience
```

---

## 📝 NOTES & OBSERVATIONS

### **Strengths:**
✅ Clean scene architecture with clear separation  
✅ Universal SceneController auto-detects buttons (low maintenance)  
✅ Robust save system with multiple data points  
✅ Novel cutscene system with good UX (auto-pause, skip for replay)  
✅ Complete level progression with unlock gates  
✅ Circular achievement navigation (good UX)  
✅ Time.timeScale safety nets to prevent stuck game bug  
✅ Best record tracking for speedrun/challenge players  
✅ Audio system with pause/resume capability  

### **Potential Improvements:**
⚠️ Restart button in Pause hardcoded to "level1" (should use currentLevelIndex)  
⚠️ Restart confirmation scene exists but not fully implemented  
⚠️ TutorialCompleted scene purpose unclear  
⚠️ Only 2 levels implemented (tutorial + level1), 10 planned  
⚠️ No in-game settings during pause (must go Settings scene)  
⚠️ Achievement system structure exists but implementation details unknown  

### **Flow Consistency:**
✅ Main Menu → Continue → **SelectLevel** (consistent with New Game)  
✅ Complete → Next Level → **level{X}** (direct load, efficient)  
✅ All scene transitions reset Time.timeScale (prevents bugs)  
✅ Back buttons consistently return to expected scenes  

---

## 🎯 BEST PRACTICES OBSERVED

1. **Universal Button System:**
   - GameObject naming convention (ContinueButton, BackButton, etc.)
   - Auto-detection reduces manual setup
   - Single SceneController for all UI scenes

2. **Save System Architecture:**
   - Centralized SaveSystem class
   - Multiple data persistence (save file + PlayerPrefs for records)
   - DeleteAllData for true reset (important for New Game)

3. **Time Management:**
   - Safety net: Time.timeScale = 1f before ALL scene loads
   - Prevents stuck time bug from cutscene/pause

4. **Audio Management:**
   - Separate channels for BGM vs cutscene music
   - Pause/Resume capability
   - SFX unaffected by Time.timeScale

5. **Level Progression:**
   - SelectLevel as central hub (not direct level loading)
   - Unlock system with visual feedback
   - Best record tracking per level

---

## 📊 SCENE TRANSITION MATRIX

| From Scene | Button/Action | To Scene | Notes |
|------------|--------------|----------|-------|
| **MainMenu** | Continue | SelectLevel | Load save, show unlocked levels |
| **MainMenu** | New Game | SelectLevel | Total reset, only Level 1 |
| **MainMenu** | Achievement | AchievementGlobal | View achievements |
| **MainMenu** | Settings | Settings | Configure options |
| **MainMenu** | Exit | Quit App | Direct quit (no confirmation) |
| **SelectLevel** | Level Button | level{X} | Load chosen level |
| **SelectLevel** | Back | MainMenu | Return to main menu |
| **level{X}** | Win | Complete | Level complete screen |
| **level{X}** | ESC | Pause | Pause menu |
| **level{X}** | Die | (Respawn) | Same level, continue |
| **Pause** | Resume | (Resume) | Unload pause, continue gameplay |
| **Pause** | Restart | level1 | ⚠️ Hardcoded! Should be currentLevel |
| **Pause** | Settings | Settings | Configure options |
| **Pause** | Main Menu | MainMenu | Exit to main menu |
| **Pause** | Exit | Exit | Confirmation scene |
| **Complete** | Main Menu | MainMenu | Exit to main menu |
| **Complete** | Retry | level{current} | Retry same level |
| **Complete** | Next Level | level{next} or MainMenu | Next level or end |
| **Settings** | Back | MainMenu | Return to main menu |
| **Settings** | Guide | Guide | View guide |
| **Settings** | Music | (Toggle) | Stay in Settings |
| **Guide** | Back | Settings | Return to settings |
| **AchievementGlobal** | Next | Achievement1 | Navigate achievements |
| **AchievementGlobal** | Back (nav) | Achievement3 | Circular navigation |
| **AchievementGlobal** | Back (exit) | MainMenu | Exit achievements |
| **Achievement1** | Next | Achievement2 | Next page |
| **Achievement1** | Back (nav) | AchievementGlobal | Previous page |
| **Achievement2** | Next | Achievement3 | Next page |
| **Achievement2** | Back | Achievement1 | Previous page |
| **Achievement3** | Next | AchievementGlobal | Loop back |
| **Achievement3** | Back | Achievement2 | Previous page |
| **Exit** | Yes | MainMenu | Confirm exit |
| **Exit** | No | Pause | Cancel, return to pause |

---

**END OF DOCUMENTATION**

---

**Generated by:** GitHub Copilot  
**Date:** December 30, 2025  
**Version:** 1.0 - Complete Game Flow Analysis  
**Project:** ArgyrosPlatformer by Argyros Team

This documentation is based on actual code analysis and should reflect the current state of the project. Update as needed when game systems change.
