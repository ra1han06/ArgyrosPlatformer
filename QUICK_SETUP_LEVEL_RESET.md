# 🎮 Level Reset System - Quick Setup

## ⚡ Quick Setup (5 Minutes)

### 1. Create LevelResetManager GameObject
1. Open **MainMenu** scene
2. Create empty GameObject → Name: **"LevelResetManager"**
3. Add Component → **LevelResetManager**
4. Configure:
   - Death Delay Before Reset: `1.0`
   - Show Debug Log: ✓ Checked

### 2. Test It!
1. Play any level
2. Fall into void (die)
3. Watch console logs:
   ```
   [LevelResetManager] Player death triggered
   [GameManager] 💀 Player died! Total deaths: 1
   [LevelResetManager] Reloading scene...
   [LevelResetManager] ✅ Data restored
   ```

## ✅ What Happens on Death

| Item | Result |
|------|--------|
| Platforms | ✅ RESET to original positions |
| Cut Platforms | ✅ RESTORED (reappear) |
| Pasted Platforms | ✅ DELETED |
| Abilities | ✅ RESET (copy/cut/paste limits) |
| Death Count | ✅ INCREMENTED (+1) |
| Timer | ✅ CONTINUES (doesn't reset) |
| Best Records | ✅ PRESERVED |

## 📝 Files Modified
- ✅ Created: `Assets/Script/System/LevelResetManager.cs`
- ✅ Modified: `Assets/Script/System/RespawnManager.cs`
- ✅ Modified: `Assets/Script/System/GameManager.cs`

## 📚 Full Documentation
See [LEVEL_RESET_SYSTEM_SETUP.md](LEVEL_RESET_SYSTEM_SETUP.md) for complete details.
