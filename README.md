# 🎮 Argyros Platformer

<div align="center">

![Unity](https://img.shields.io/badge/Unity-2022.3+-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac%20%7C%20Linux-blue?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**A unique 3D platformer game with innovative Copy-Cut-Paste mechanics and integrated visual novel cutscenes**

[Features](#-features) • [Installation](#-installation) • [Gameplay](#-gameplay) • [Documentation](#-documentation) • [Development](#-development)

</div>

---

## 📋 Table of Contents

- [About](#-about)
- [Features](#-features)
- [Gameplay Mechanics](#-gameplay-mechanics)
- [Installation](#-installation)
- [How to Play](#-how-to-play)
- [Project Structure](#-project-structure)
- [Core Systems](#-core-systems)
- [Documentation](#-documentation)
- [Development](#-development)
- [Contributors](#-contributors)
- [License](#-license)

---

## 🎯 About

**Argyros Platformer** is a 3D platformer game built with Unity that combines traditional platforming challenges with a unique **Copy-Cut-Paste** mechanic. Players can manipulate platforms in the environment to solve puzzles and overcome obstacles, creating their own paths to victory.

The game features:
- 🎨 **Innovative Platform Manipulation** - Copy, cut, and paste platforms to create new paths
- 📖 **Visual Novel Cutscenes** - Story-driven narrative between levels
- 🏆 **Achievement System** - Track your progress and unlock achievements
- ⏱️ **Speedrun Mode** - Compete for the best time with minimal deaths
- 🎵 **Atmospheric Audio** - Immersive background music and sound effects

---

## ✨ Features

### 🎮 Core Gameplay
- **Copy-Cut-Paste Mechanics** - Manipulate platforms using keyboard shortcuts (Ctrl+C, Ctrl+X, Ctrl+V)
- **Ability Limits System** - Strategic resource management with limited uses per level
- **Smart Platform Targeting** - Intelligent raycast detection for platform interaction
- **Respawn System** - Checkpoint-based respawn with full level reset on death

### 🎬 Narrative
- **Visual Novel Integration** - Story cutscenes using novel dialogue system
- **First-Play Cutscenes** - Automatic story progression on first level playthrough
- **Skippable Replays** - Cutscenes only play once, then skip on subsequent attempts

### 📊 Progression
- **10 Planned Levels** - Currently featuring Tutorial and Level 1-2
- **Level Unlock System** - Progressive unlock based on completion
- **Save System** - Auto-save progress with continue/new game options
- **Best Time/Deaths Tracking** - Personal record keeping per level

### 🎨 UI/UX
- **Complete Menu System** - MainMenu, Settings, Achievements, Level Selection
- **In-Game HUD** - Timer, death counter, ability usage indicators
- **Pause Menu** - Resume, restart, or return to main menu
- **Victory Screen** - Stats display with options to retry, next level, or main menu

### 🔧 Technical Features
- **Singleton Architecture** - Persistent GameManager across scenes
- **Level Reset System** - Full scene reload on death while preserving stats
- **PlayerPrefs Integration** - Save data, best times, cutscene flags
- **Modular Component Design** - Reusable scripts and prefabs

---

## 🎮 Gameplay Mechanics

### Copy-Cut-Paste System

The core mechanic revolves around manipulating platforms in the game world:

#### **Copy (Ctrl+C)**
- Creates a duplicate blueprint of the targeted platform
- **Limit:** 1 copy per level (default)
- Original platform remains in place
- Stores platform type, size, and properties

#### **Cut (Ctrl+X)**
- Removes the targeted platform from the scene
- **Limit:** 1 cut per level (default)
- Platform is stored in clipboard for pasting
- Can only cut platforms marked as `CutablePlatform`

#### **Paste (Ctrl+V)**
- Places the clipboard platform in front of the player
- **Limit:** 3 pastes per level (default)
- Direction-based placement (faces player's direction)
- Can paste multiple instances of copied/cut platforms

### Platform Types

| Platform Type | Can Copy | Can Cut | Description |
|--------------|----------|---------|-------------|
| **CopyablePlatform** | ✅ | ❌ | Can be copied but not removed |
| **CutablePlatform** | ✅ | ✅ | Can be both copied and cut |
| **PermanentHazardPlatform** | ❌ | ❌ | Indestructible hazard platforms |
| **Regular Platform** | ❌ | ❌ | Standard non-interactive platforms |

### Death & Respawn System

When the player dies:
1. 💀 **Death Count Increases** (+1)
2. ⏱️ **Timer Continues** (doesn't reset)
3. 🔄 **Level Fully Resets**:
   - All platforms return to original positions
   - Cut platforms respawn
   - Pasted platforms are destroyed
   - Ability limits reset to default
4. 🎯 **Player Respawns** at the level's spawn point

This ensures fair speedrunning while maintaining challenge.

### Ability Limits

Default configuration (can be modified in Inspector):
```
Max Copy: 1 use
Max Cut:  1 use
Max Paste: 3 uses
```

**Cheat Codes** (for testing):
- `godmodeops` - Infinite copy/cut/paste
- `iwantinvinitepaste` - Infinite paste only

---

## 🔧 Installation

### Prerequisites
- **Unity 2022.3+** (LTS recommended)
- **Unity Input System** package
- **TextMeshPro** package
- **High Definition Render Pipeline (HDRP)** or **Universal Render Pipeline (URP)**

### Setup Steps

1. **Clone the Repository**
```bash
git clone https://github.com/yourusername/ArgyrosPlatformer.git
cd ArgyrosPlatformer
```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Add" → Select the `ArgyrosPlatformer` folder
   - Open with Unity 2022.3+ or newer

3. **Configure Build Settings**
   - Go to `File → Build Settings`
   - Ensure all scenes are added (they should be pre-configured):
     ```
     Scenes/UI/MainMenu
     Scenes/UI/SelectLevel
     Scenes/UI/Complete
     Scenes/UI/Settings
     Scenes/UI/Guide
     Scenes/UI/Achievement*
     Scenes/level/tutorial
     Scenes/level/level1
     Scenes/level/level2
     ```

4. **Setup Level Reset Manager** (Required for proper gameplay)
   - Open `MainMenu` scene
   - Create empty GameObject → Name: `LevelResetManager`
   - Add Component: `LevelResetManager`
   - Configure:
     - Death Delay Before Reset: `1.0`
     - Show Debug Log: ✓ Checked
   - See [QUICK_SETUP_LEVEL_RESET.md](QUICK_SETUP_LEVEL_RESET.md) for details

5. **Play the Game**
   - Press Play in Unity Editor
   - Or build and run (`Ctrl+B`)

---

## 🕹️ How to Play

### Controls

| Input | Action |
|-------|--------|
| **W/A/S/D** or **Arrow Keys** | Move player |
| **Space** | Jump |
| **Mouse** | Look around |
| **Ctrl+C** | Copy targeted platform |
| **Ctrl+X** | Cut targeted platform |
| **Ctrl+V** | Paste platform |
| **ESC** | Pause menu |

### Game Flow

```
MainMenu
   ├─ Continue → Resume from last level
   ├─ New Game → Start fresh (Level 1 unlocked only)
   ├─ Achievement → View unlocked achievements
   ├─ Settings → Music toggle, Guide
   └─ Exit → Quit game
        ↓
  SelectLevel
   ├─ Choose unlocked level
   └─ Back to MainMenu
        ↓
  Gameplay (Level X)
   ├─ Cutscene (first time only)
   ├─ Complete level objectives
   ├─ Reach the goal
   └─ ESC to pause
        ↓
  Complete Screen
   ├─ View stats (Time, Deaths)
   ├─ Main Menu
   ├─ Retry (restart level)
   └─ Next Level (if available)
```

### Tips for Success

1. 🎯 **Plan Your Route** - Observe the level before using abilities
2. ⏱️ **Manage Time** - Timer never resets, even on death
3. 💡 **Limited Resources** - Use copy/cut/paste wisely
4. 🔄 **Death Resets Everything** - But death count and timer persist
5. 🏆 **Beat Your Best** - Try to minimize time and deaths

---

## 📁 Project Structure

```
ArgyrosPlatformer/
│
├── Assets/
│   ├── Animation/           # Character and object animations
│   ├── Audio/               # Music and sound effects
│   ├── ModelAsset/          # 3D models and meshes
│   ├── NovelCutscene/       # Visual novel cutscene assets
│   ├── Prefab/              # Reusable game objects
│   │   ├── Player.prefab
│   │   ├── Enemy.prefab
│   │   ├── InteractPlatform/
│   │   └── Hazard/
│   ├── Resources/           # Runtime-loaded assets
│   ├── Scenes/
│   │   ├── UI/              # Menu scenes (MainMenu, Settings, etc.)
│   │   └── level/           # Gameplay levels (tutorial, level1-10)
│   ├── Script/
│   │   ├── Player/          # Player controller, animation, interaction
│   │   ├── System/          # Core game systems (GameManager, SaveSystem)
│   │   ├── UI/              # UI controllers and handlers
│   │   ├── Platforms/       # Platform scripts (Copyable, Cutable, etc.)
│   │   ├── Enemy/           # Enemy AI and behavior
│   │   └── Editor/          # Custom Unity Editor tools
│   ├── Settings/            # Project settings and configurations
│   ├── Sprites/             # 2D textures and UI sprites
│   └── TextMesh Pro/        # TMP fonts and assets
│
├── ProjectSettings/         # Unity project configuration
├── Packages/                # Package dependencies
│
├── README.md                # This file
├── GAME_FLOW_DOCUMENTATION.md      # Complete game flow guide
├── GAME_FLOW_TROUBLESHOOTING.md    # Common issues & solutions
├── LEVEL_RESET_SYSTEM_SETUP.md     # Level reset system details
└── QUICK_SETUP_LEVEL_RESET.md      # Quick setup guide
```

---

## ⚙️ Core Systems

### 1. GameManager (Singleton)
**Location:** `Assets/Script/System/GameManager.cs`

**Responsibilities:**
- Track level timer (starts from 0, never resets)
- Track death count per level
- Manage save data (`GameSaveData`)
- Handle level completion and unlocking
- Persist across scenes (DontDestroyOnLoad)

**Key Methods:**
```csharp
GameManager.Instance.StartTimer()
GameManager.Instance.StopTimer()
GameManager.Instance.CompleteLevel(int levelIndex)
GameManager.Instance.PlayerDied()
```

### 2. SaveSystem
**Location:** `Assets/Script/System/SaveSystem.cs`

**Responsibilities:**
- Save/load game progress to PlayerPrefs
- Store unlocked levels, last played level
- Store best time and death count per level
- Manage cutscene played flags

**Data Stored:**
```json
{
  "lastPlayedLevel": 1,
  "unlockedLevels": [true, false, false, ...],
  "BestTime_Level1": 45.3,
  "BestDeaths_Level1": 2,
  "CutscenePlayed_Level1": 1
}
```

### 3. LevelResetManager (Singleton)
**Location:** `Assets/Script/System/LevelResetManager.cs`

**Responsibilities:**
- Handle player death events
- Save timer/death count before scene reload
- Reload current scene
- Restore saved data after reload
- Reset player abilities

**Setup Required:** See [Quick Setup Guide](QUICK_SETUP_LEVEL_RESET.md)

### 4. PlayerPlatformInteractor
**Location:** `Assets/Script/Player/PlayerPlatformInteractor.cs`

**Responsibilities:**
- Detect targetable platforms via raycast
- Handle copy/cut/paste input
- Manage ability limits and counters
- Spawn pasted platforms
- Provide visual feedback (toast notifications)

**Inspector Configuration:**
```
Raycast Distance: 2.0
Box Cast Size: (0.5, 2.0, 0.3)
Max Copy: 1
Max Cut: 1
Max Paste: 3
```

### 5. RespawnManager
**Location:** `Assets/Script/System/RespawnManager.cs`

**Responsibilities:**
- Detect player death (fall triggers, hazards)
- Call LevelResetManager on death
- Handle death animation/effects

### 6. Scene Flow Controller
**Location:** `Assets/Script/UI/SceneController.cs`

**Responsibilities:**
- Load scenes with proper cleanup
- Reset Time.timeScale before scene loads
- Handle scene transitions

---

## 📚 Documentation

This project includes comprehensive documentation:

### 📖 [GAME_FLOW_DOCUMENTATION.md](GAME_FLOW_DOCUMENTATION.md)
Complete guide to game flow and scene structure:
- Scene overview and purpose
- Detailed scene-by-scene flow diagrams
- Button mappings and handlers
- UI navigation paths
- Level progression system

### 🔧 [GAME_FLOW_TROUBLESHOOTING.md](GAME_FLOW_TROUBLESHOOTING.md)
Common issues and solutions:
- Continue button disabled
- Player stuck (time frozen)
- Restart goes to wrong level
- Cutscene plays every time
- Levels locked after continue
- Best time/deaths not showing

### 🔄 [LEVEL_RESET_SYSTEM_SETUP.md](LEVEL_RESET_SYSTEM_SETUP.md)
Full documentation on the level reset system:
- Architecture overview
- Component responsibilities
- Setup instructions
- How it works (flow diagrams)
- Data preservation rules
- Testing checklist

### ⚡ [QUICK_SETUP_LEVEL_RESET.md](QUICK_SETUP_LEVEL_RESET.md)
5-minute setup guide for level reset functionality

---

## 🛠️ Development

### Unity Version
- **Recommended:** Unity 2022.3 LTS or newer
- **Tested on:** Unity 2022.3.x

### Dependencies

Core packages (auto-installed):
```json
{
  "com.unity.inputsystem": "1.17.0",
  "com.unity.ugui": "2.0.0",
  "com.unity.render-pipelines.high-definition": "17.3.0",
  "com.unity.render-pipelines.universal": "17.3.0",
  "com.unity.textmeshpro": "3.0.0+"
}
```

Optional development tools:
```json
{
  "com.unity.ide.vscode": "1.2.5",
  "com.unity.ide.rider": "3.0.38",
  "com.unity.test-framework": "1.6.0",
  "com.coplaydev.unity-mcp": "github/CoplayDev/unity-mcp"
}
```

### Adding New Levels

1. **Create Scene**
   - Duplicate `Assets/Scenes/level/level1.unity`
   - Rename to `level{X}.unity`

2. **Configure GameObjects**
   - Add platforms (with `CopyablePlatform` or `CutablePlatform` components)
   - Add `RespawnPoint` for player spawn
   - Add `WinningTrigger` for level completion
   - Add hazards/enemies as needed

3. **Update Build Settings**
   - `File → Build Settings`
   - Add new scene to build list

4. **Configure Cutscene (Optional)**
   - Create cutscene in NovelCutscene system
   - Set `needsCutsceneOnFirstPlay` flag in LevelButtonHandler

5. **Update Level Selection UI**
   - Add button to `SelectLevel` scene
   - Configure button handler with level index

### Custom Editor Tools

**HazardPlatformCreator** (`Assets/Script/Editor/HazardPlatformCreator.cs`)
- Creates hazard platforms with automatic component setup
- Menu: `GameObject → 3D Object → Create Hazard Platform`

**PlayerPlatformInteractorEditor** (`Assets/Script/Editor/PlayerPlatformInteractorEditor.cs`)
- Custom inspector for PlayerPlatformInteractor
- Real-time debugging of ability limits

### Debug Features

**Console Logging**
- Enable in GameManager inspector: "Show Debug Log"
- Tracks timer, deaths, level completion, cutscenes

**Cheat Codes** (for testing):
```
godmodeops           → Infinite copy/cut/paste
iwantinvinitepaste   → Infinite paste only
```

**Editor Shortcuts**
- `Ctrl+P` - Play in editor
- `Ctrl+Shift+P` - Pause
- `Ctrl+Shift+F` - Frame selected object

---

## 🎨 Art & Assets

### Visual Style
- **3D Art Style:** Low-poly with smooth shading
- **Color Palette:** Warm tones with contrast for interactive elements
- **UI Design:** Clean, minimalist interface
- **Effects:** Particle systems for hazards, victory, death

### Asset Sources
- **Models:** Custom-created or Unity Asset Store
- **Textures:** Hand-painted and procedural
- **Audio:** Royalty-free or original compositions
- **Fonts:** TextMeshPro default + custom fonts

---

## 🏆 Achievements System

The game includes a multi-page achievement system:

**Achievement Categories:**
1. **Achievement1** - Speed-based achievements
2. **Achievement2** - Death-minimization achievements
3. **Achievement3** - Exploration and secret achievements

**Achievement Structure:**
```csharp
// Each achievement tracks:
- Achievement ID
- Title
- Description
- Unlock condition
- Unlock status (PlayerPrefs)
- Icon/sprite
```

---

## 🐛 Known Issues & Limitations

### Current Limitations
- ⚠️ Only 3 levels implemented (Tutorial, Level 1, Level 2)
- ⚠️ Achievement system UI present but logic incomplete
- ⚠️ Some menu back buttons use hardcoded navigation

### Planned Fixes
- [ ] Complete all 10 levels
- [ ] Implement achievement unlock logic
- [ ] Add dynamic back button navigation
- [ ] Improve platform targeting raycast visualization
- [ ] Add gamepad/controller support

See [GAME_FLOW_TROUBLESHOOTING.md](GAME_FLOW_TROUBLESHOOTING.md) for solutions to common issues.

---

## 🚀 Roadmap

### Version 1.0 (Current)
- ✅ Core copy-cut-paste mechanics
- ✅ Level reset system
- ✅ Save/load system
- ✅ Tutorial and 2 levels
- ✅ Menu system and UI

### Version 1.1 (Planned)
- [ ] Complete levels 3-10
- [ ] Achievement system implementation
- [ ] Audio polish (more SFX, music tracks)
- [ ] Level editor tools

### Version 2.0 (Future)
- [ ] Steam integration
- [ ] Online leaderboards
- [ ] Workshop support for custom levels
- [ ] Speedrun mode with ghost replay

---

## 👥 Contributors

**Project Lead & Developer:** [Your Name/Team]

**Special Thanks:**
- Unity Technologies for the game engine
- TextMeshPro team
- Unity Asset Store creators

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2026 Argyros Platformer Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
```

---

## 📞 Contact & Support

- **GitHub Issues:** [Report bugs or request features](https://github.com/yourusername/ArgyrosPlatformer/issues)
- **Email:** your-email@example.com
- **Discord:** [Join our community](#)

---

## 🙏 Acknowledgments

- Thanks to all playtesters and contributors
- Inspired by classic platformers and puzzle games
- Built with ❤️ using Unity

---

<div align="center">

**⭐ If you enjoy this game, please give it a star! ⭐**

[⬆ Back to Top](#-argyros-platformer)

</div>
