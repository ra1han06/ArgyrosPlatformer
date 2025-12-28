# 🏁 Complete Scene Button System - Setup Guide

## ✅ IMPLEMENTATION SUMMARY

Semua script sudah dibuat dan terintegrasi dengan SaveSystem & GameManager:

### 📁 Files Created/Modified:

1. **NEW: CompleteMenuController.cs**
   - Path: `Assets/Script/UI/CompleteMenuController.cs`
   - Function: Handle 3 buttons di Complete scene (Main Menu, Retry, Next Level)

2. **MODIFIED: GameManager.cs**
   - Added: `UnlockNextLevel()` method
   - Updated: `CompleteLevel()` untuk auto-unlock level berikutnya

3. **MODIFIED: LevelButtonHandler.cs**
   - Added: Lock/unlock status display
   - Added: Disable button untuk locked levels
   - Added: Visual locked icon support

---

## 🎮 COMPLETE SCENE SETUP (Unity Editor)

### Step 1: Attach CompleteMenuController Script

1. **Buka scene `Complete.unity`** (Assets/Scenes/UI/Complete.unity)

2. **Buat GameObject baru** (atau gunakan existing GameObject):
   - Klik kanan di Hierarchy → Create Empty
   - Rename menjadi **"CompleteMenuController"**

3. **Attach script CompleteMenuController**:
   - Select GameObject "CompleteMenuController"
   - Add Component → CompleteMenuController

4. **Assign Button References di Inspector**:
   ```
   CompleteMenuController (Script)
   ├─ Main Menu Button   → Drag button "MainMenu" dari scene
   ├─ Retry Button       → Drag button "Retry" dari scene  
   └─ Next Level Button  → Drag button "NextLevel" dari scene
   ```

5. **SAVE SCENE** (Ctrl+S)

---

## 🗺️ SELECT LEVEL SCENE SETUP (Lock/Unlock Visual)

### Step 2: Update Level Buttons dengan Lock Icons

Untuk SETIAP button level (Level 2, Level 3, dst):

1. **Buka scene `SelectLevel.unity`**

2. **Pilih button level** (contoh: "Button Level 2")

3. **Buat Locked Icon** (child GameObject):
   - Klik kanan button → UI → Image
   - Rename menjadi **"LockedIcon"**
   - Set sprite ke ikon gembok/lock
   - Posisikan di tengah button

4. **Update LevelButtonHandler Inspector**:
   ```
   Level Button Handler (Script)
   ├─ Level Scene Name: "level2"
   ├─ Level Number: 2
   ├─ ...
   ├─ Locked Icon        → Drag GameObject "LockedIcon"
   └─ Disable When Locked → ✅ CENTANG
   ```

5. **Repeat untuk semua level buttons** (Level 2 - Level 10)

6. **SAVE SCENE** (Ctrl+S)

---

## 🔧 BUILD SETTINGS CHECKLIST

Pastikan semua scene ada di Build Settings dengan urutan benar:

```
File → Build Settings
├─ 0. level1
├─ 1. MainMenu
├─ 2. SelectLevel
├─ 3. Complete
├─ 4. level2  (pastikan ada!)
└─ ... (level lainnya)
```

**PENTING**: Jika `level2.unity` belum ada, buat dulu atau CompleteMenuController akan fallback ke MainMenu.

---

## 🎯 EXPECTED BEHAVIOR

### ✅ Complete Scene Buttons:

| Button | Action | Result |
|--------|--------|--------|
| **Main Menu** | Load MainMenu | Kembali ke menu utama |
| **Retry** | Reset + Load level1 | Ulang level yang sama |
| **Next Level** | Load level2 (jika ada) | Masuk ke level berikutnya |

### ✅ Level Unlock System:

1. **Saat Level 1 selesai**:
   - Level 2 otomatis **UNLOCKED**
   - Data saved ke `SaveSystem`
   - Button Level 2 di SelectLevel → **ENABLED**
   - Locked icon → **HIDDEN**

2. **Saat Level 2 selesai**:
   - Level 3 otomatis **UNLOCKED**
   - Dan seterusnya...

### ✅ Best Record Display:

Di SelectLevel scene, setiap button level akan tampilkan:
- **Best Time** (format MM:SS:MS)
- **Best Deaths**
- **"Not Completed"** jika belum pernah selesai
- **Button disabled** jika locked
- **Locked icon visible** jika locked

---

## 🐛 TROUBLESHOOTING

### ❌ Problem: Button tidak berfungsi

**Solution**:
- Check apakah CompleteMenuController sudah di-attach
- Check apakah button references sudah di-assign di Inspector
- Check console untuk error messages

### ❌ Problem: Level 2 tidak unlock

**Solution**:
- Check apakah `GameManager.CompleteLevel()` dipanggil saat finish
- Check console untuk log: `"🔓 Level 2 UNLOCKED!"`
- Check save data di PlayerPrefs (menu Tools → Clear PlayerPrefs)

### ❌ Problem: Next Level button error "Scene not found"

**Solution**:
- Pastikan scene level2.unity sudah dibuat
- Pastikan level2 ada di Build Settings
- Atau akan otomatis fallback ke MainMenu

### ❌ Problem: Button level masih enabled padahal locked

**Solution**:
- Check `Disable When Locked` di LevelButtonHandler Inspector
- Check `IsLevelUnlocked()` method di console logs

---

## 📊 SAVE DATA STRUCTURE

Data yang disimpan otomatis di `SaveSystem`:

```csharp
GameSaveData {
    lastPlayedLevel: int           // Level terakhir dimainkan
    totalPlayTime: float          // Total waktu bermain
    completedLevels: bool[]       // Level mana saja yang sudah selesai
    unlockedLevels: bool[]        // Level mana saja yang unlocked
    levelStars: int[]             // (Reserved untuk fitur stars)
    unlockedAchievements: bool[]  // (Reserved untuk achievements)
}
```

**PlayerPrefs Keys (Best Records)**:
- `BestTime_Level1`, `BestTime_Level2`, ... → float
- `BestDeaths_Level1`, `BestDeaths_Level2`, ... → int

---

## 🧪 TESTING FLOW

### Test 1: Complete Scene Buttons

1. Play level1 → finish
2. Di Complete scene:
   - ✅ Click **"Retry"** → level1 restart, timer & deaths reset
   - ✅ Click **"Next Level"** → level2 load (atau MainMenu jika scene tidak ada)
   - ✅ Click **"Main Menu"** → kembali ke MainMenu

### Test 2: Level Unlock

1. New game → Only Level 1 unlocked
2. Finish Level 1 → Check console: `"🔓 Level 2 UNLOCKED!"`
3. Buka SelectLevel → Button Level 2 **ENABLED**, locked icon **HIDDEN**
4. Click Level 2 → bisa masuk ke level2

### Test 3: Best Record Display

1. Finish Level 1 dengan time 02:30:50, deaths 5
2. Buka SelectLevel → Button Level 1 tampilkan:
   - Best: 02:30:50
   - Deaths: 5
3. Finish Level 1 lagi dengan time 01:45:20, deaths 3
4. Buka SelectLevel → Button Level 1 tampilkan (updated):
   - Best: 01:45:20
   - Deaths: 3

---

## ✅ COMPLETION CHECKLIST

- [ ] CompleteMenuController script created ✅
- [ ] GameManager.UnlockNextLevel() implemented ✅
- [ ] LevelButtonHandler lock/unlock logic added ✅
- [ ] CompleteMenuController attached ke Complete scene
- [ ] Button references assigned di Inspector
- [ ] Locked icons created untuk level buttons
- [ ] LevelButtonHandler updated dengan locked icon references
- [ ] All scenes ada di Build Settings dengan urutan benar
- [ ] Testing: Complete scene buttons berfungsi
- [ ] Testing: Level unlock system berfungsi
- [ ] Testing: Best record display berfungsi

---

## 🎉 NEXT STEPS (FASE 2 & 3)

Setelah Complete Scene buttons berfungsi, lanjut ke:

1. **FASE 2: Level Progression System**
   - Star rating (1-3 stars based on time/deaths)
   - Level difficulty settings
   - Progress percentage display

2. **FASE 3: Achievement System**
   - Achievement unlock triggers
   - Achievement popup UI
   - Achievement list display

---

**Author**: GitHub Copilot  
**Date**: December 29, 2025  
**Version**: 1.0
