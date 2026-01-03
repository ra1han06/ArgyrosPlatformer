# 🎓 VASE TUTORIAL INTERACTION SYSTEM - Setup Guide

## 📋 OVERVIEW

Sistem interaksi Vase untuk Tutorial Scene yang memungkinkan player membuka Tutorial Canvas dengan menekan tombol **E**.

### ✨ Fitur:
- ✅ Interaksi dengan tombol **E** saat player dekat vase
- ✅ World-space UI prompt "Press E" di atas vase
- ✅ Setiap vase bisa menampilkan tutorial berbeda
- ✅ Bisa diinteraksi berkali-kali (tidak disable setelah dipakai)
- ✅ Canvas muncul **instant** (tanpa fade/animasi)
- ✅ **Game tidak pause** - player tetap bisa bergerak
- ✅ Tidak ada perubahan visual pada vase (no highlight/outline)

---

## 📦 FILES YANG DIBUAT

### 1. **VaseTutorialInteract.cs**
**Location:** `Assets/Script/System/VaseTutorialInteract.cs`

**Fungsi:**
- Detect player proximity menggunakan trigger collider
- Tampilkan/sembunyikan interaction prompt "Press E"
- Buka Tutorial Canvas saat E ditekan
- Tidak mengatur Time.timeScale atau pause

**Inspector Fields:**
- `Interact Key` - Tombol untuk interact (default: E)
- `Player Tag` - Tag player (default: "Player")
- `Interaction Radius` - Jarak deteksi player (default: 2)
- `Tutorial Canvas` - GameObject Canvas yang akan ditampilkan
- `Interaction Prompt` - World-space UI "Press E" (child object)

### 2. **CloseTutorialCanvas.cs**
**Location:** `Assets/Script/System/CloseTutorialCanvas.cs`

**Fungsi:**
- Script untuk tombol Close di Tutorial Canvas
- Auto-detect Canvas dari parent hierarchy
- Menutup canvas dengan SetActive(false)

---

## 🎯 YANG SUDAH ADA DI SCENE

**UICanvas** - Canvas utama sudah ada di scene:
- Location: `UICanvas` (root hierarchy)
- Render Mode: Screen Space - Overlay
- Contains: LimitPanel, PauseButton, PauseMenuPanel, GuideButton, dll
- **TutorialCanvas sudah ada** (tapi masih kosong, perlu di-setup)

**Vase Objects** - 4 Vase sudah ada:
- Location: `Platform → Guide`
- Vase_001
- Vase_001 (1)
- Vase_001 (2)
- Vase_001 (3)

**Tutorial Sprites** - 7 tutorial images tersedia:
- Location: `Assets/Sprites/Tutorial/`
- 1.png sampai 7.png (bisa pakai 4 untuk 4 vase)

---

## 🛠️ SETUP STEP-BY-STEP

### PART 1: Setup Tutorial Canvas (One-time untuk semua vase)

#### 1. Buat Tutorial Canvas GameObject (sebagai child dari UICanvas)

```
Hierarchy:
  UICanvas ← Canvas utama yang sudah ada
    ├─ (UI elements lain yang sudah ada...)
    └─ TutorialCanvas_1 ← Tutorial Canvas baru (GameObject/Panel)
       ├─ Panel (Image - background)
       ├─ TutorialImage (Image - untuk tutorial sprite)
       └─ CloseButton (Button)
          └─ Text (TextMeshPro - "Close" atau "X")
```

**Steps:**
1. **Select UICanvas** di Hierarchy (Canvas utama yang sudah ada)
2. **Right-click UICanvas** → UI → Panel → Rename jadi `TutorialCanvas_1`
   - ✅ Dengan cara ini, TutorialCanvas_1 jadi **child dari UICanvas**
   - ✅ Tidak perlu setting Canvas lagi (pakai Canvas parent)
   - ✅ Lebih organized dan clean!
3. **Set size TutorialCanvas_1 Panel:**
   - Anchor: Stretch (full screen)
   - Left: 0, Top: 0, Right: 0, Bottom: 0
   - Color: Hitam semi-transparan (R:0, G:0, B:0, A:200) untuk backdrop
4. **Tambahkan Tutorial Image:**
   - Right-click TutorialCanvas_1 → UI → Image
   - Rename: `TutorialImage`
   - Source Image: **Drag sprite dari Assets/Sprites/Tutorial** (misal: 1.png)
   - Set size sesuai kebutuhan (misal: 800x600)
   - Posisi di center
5. **Tambahkan Close Button:**
   - Right-click TutorialCanvas_1 → UI → Button
   - Rename: `CloseButton`
   - Posisi di pojok kanan atas tutorial image
   - Tambahkan **CloseTutorialCanvas.cs** component
   - Assign Tutorial Canvas: Drag **TutorialCanvas_1** ke field `Tutorial Canvas`
   - OnClick() → Tambahkan event:
     - Target: CloseButton (self)
     - Function: CloseTutorialCanvas.CloseCanvas()

#### 2. Buat Canvas untuk Tutorial Lain (jika perlu)

**Duplicate TutorialCanvas_1** untuk vase lainnya:
1. Select TutorialCanvas_1 → Ctrl+D
2. Rename jadi `TutorialCanvas_2`, `TutorialCanvas_3`, `TutorialCanvas_4`
3. Ganti sprite di TutorialImage sesuai tutorial (2.png, 3.png, 4.png)
4. Semua tetap sebagai **child dari UICanvas**

**Keuntungan Setup ini:**
- ✅ **Organized**: Semua UI di satu tempat (UICanvas)
- ✅ **Tidak perlu Canvas baru**: TutorialCanvas pakai Canvas parent
- ✅ **Sorting Order konsisten**: Otomatis ikut UICanvas
- ✅ **Event System shared**: Pakai EventSystem yang sama
- ✅ **Performance lebih baik**: Tidak ada multiple Canvas di scene

#### 3. Pastikan Canvas Inactive di awal
- Di Inspector, **uncheck** GameObject active untuk setiap TutorialCanvas
- Script VaseTutorialInteract akan SetActive(true) saat E ditekan
- Game objects yang inactive: TutorialCanvas_1, TutorialCanvas_2, TutorialCanvas_3, TutorialCanvas_4

---

### PART 2: Setup Vase GameObject

#### 1. Pilih Vase GameObject yang Sudah Ada

**Lokasi Vase:** `Platform → Guide → Vase_001` (dan vase lainnya)

```
Hierarchy:
  Platform
    └─ Guide
       ├─ Vase_001 ← Pilih vase ini
       ├─ Vase_001 (1)
       ├─ Vase_001 (2)
       └─ Vase_001 (3)
```

**Steps:**
1. **Select Vase_001** di hierarchy (Platform → Guide → Vase_001)
   - ✅ Vase model 3D sudah ada
   - ✅ Tinggal tambahkan script dan setup interaction

2. **Catatan:**
   - Tidak perlu buat Vase baru, gunakan yang sudah ada
   - Ada 4 vase di scene (bisa untuk 4 tutorial berbeda)

#### 2. Tambahkan VaseTutorialInteract Component

1. **Select Vase_1**
2. **Add Component** → Search "VaseTutorialInteract"
3. **Script akan auto-create Trigger Collider** jika belum ada
   - ✅ Collider otomatis dibuat dengan isTrigger = true
   - ✅ Ukuran disesuaikan dengan Interaction Radius

4. **Inspector Settings:**
   - **Interact Key:** E (default)
   - **Player Tag:** Player
   - **Interaction Radius:** 2 (sesuaikan jarak deteksi)
   - **Tutorial Canvas:** Drag TutorialCanvas_1 dari Hierarchy
   - **Interaction Prompt:** (akan di-setup di step berikutnya)

#### 3. Buat World-Space Interaction Prompt "Press E"

```
Hierarchy:
  Vase_1
    └─ InteractionPrompt ← World-Space UI
       └─ Canvas (World Space)
          └─ PressEText (TextMeshPro)
```

**Steps:**
1. **Right-click Vase_1** → Create Empty → Rename: `InteractionPrompt`
2. **Position InteractionPrompt** di atas vase (Y + 1.5 atau sesuai tinggi vase)

3. **Tambahkan Canvas (World Space):**
   - Right-click InteractionPrompt → UI → Canvas
   - Canvas Settings:
     - **Render Mode:** World Space
     - **Width:** 200, **Height:** 50
     - **Scale:** 0.01, 0.01, 0.01 (agar tidak terlalu besar)
     - **Rotation X:** 0 (menghadap camera)

4. **Tambahkan Text:**
   - Right-click Canvas → UI → Text - TextMeshPro
   - Rename: `PressEText`
   - Text: **"Press E"**
   - Font Size: 24
   - Alignment: Center, Middle
   - Color: Putih atau Kuning

5. **Assign ke Script:**
   - Select **Vase_1**
   - Di VaseTutorialInteract component
   - **Interaction Prompt:** Drag `InteractionPrompt` GameObject

#### 4. Test Vase

1. **Play Mode**
2. **Dekat ke Vase** → "Press E" muncul
3. **Tekan E** → Tutorial Canvas muncul
4. **Klik Close** → Tutorial Canvas hilang
5. **Vase bisa diinteraksi lagi** (berulang kali)

---

### PART 3: Setup Multiple Vases (Optional)

Untuk vase kedua, ketiga, dst:

1. **Duplicate Vase_1:**
   - Select Vase_1 → Ctrl+D
   - Rename: Vase_2, Vase_3, dst
   - Posisikan di lokasi berbeda

2. **Ganti Tutorial Canvas:**
   - Select Vase_2
   - Di VaseTutorialInteract:
     - **Tutorial Canvas:** Drag TutorialCanvas_2 (yang berbeda)

3. **Setiap vase bisa punya tutorial berbeda!**

---

## 🎮 CARA KERJA SISTEM

### Player Mendekati Vase:
1. **Trigger Collider** di vase detect player masuk area
2. **OnTriggerEnter** → playerInRange = true
3. **Interaction Prompt** "Press E" muncul (SetActive true)

### Player Menekan E:
1. **Update()** detect Input.GetKeyDown(KeyCode.E)
2. **ShowTutorial()** dipanggil
3. **Tutorial Canvas** diaktifkan: `tutorialCanvas.SetActive(true)`
4. **Game tetap berjalan** (tidak ada Time.timeScale = 0)
5. Player **tetap bisa bergerak**

### Player Menutup Tutorial:
1. **Klik tombol Close** di canvas
2. **CloseTutorialCanvas.CloseCanvas()** dipanggil
3. **Canvas** di-nonaktifkan: `tutorialCanvas.SetActive(false)`

### Player Menjauh dari Vase:
1. **OnTriggerExit** → playerInRange = false
2. **Interaction Prompt** hilang (SetActive false)

---

## 📌 IMPORTANT NOTES

### ✅ DO's:
- ✅ Pastikan **Player GameObject punya tag "Player"**
- ✅ Tutorial Canvas **default inactive** (unchecked di Inspector)
- ✅ World-space prompt **posisi di atas vase** agar terlihat
- ✅ Test di Play Mode untuk memastikan trigger area cukup besar
- ✅ Setiap vase bisa punya **Tutorial Canvas berbeda**
- ✅ **Setup TutorialCanvas sebagai child dari UICanvas** (lebih organized!)
- ✅ Vase ada di **Platform → Guide** (4 vase tersedia)

### ❌ DON'Ts:
- ❌ Jangan set Time.timeScale = 0 (game tidak pause)
- ❌ Jangan gunakan CanvasGroup untuk fade (instant show/hide)
- ❌ Jangan disable vase setelah dipakai (bisa interact berkali-kali)
- ❌ Jangan ubah material/color vase (no visual feedback pada vase)
- ❌ Jangan buat Canvas baru terpisah (gunakan UICanvas yang sudah ada)

---

## 🐛 TROUBLESHOOTING

### Problem: "Press E" tidak muncul
**Solution:**
- Cek apakah Player punya tag **"Player"**
- Cek Interaction Radius (perbesar jika terlalu kecil)
- Cek apakah InteractionPrompt sudah di-assign di Inspector
- Cek Console untuk debug log

### Problem: Tutorial Canvas tidak muncul saat E ditekan
**Solution:**
- Cek apakah Tutorial Canvas sudah di-assign di Inspector
- Cek Console untuk warning
- Pastikan Canvas ada di scene (tidak dihapus)

### Problem: Tutorial Canvas tidak menutup saat klik Close
**Solution:**
- Cek apakah CloseButton punya **CloseTutorialCanvas** component
- Cek OnClick() event sudah di-setup (CloseCanvas method)
- Cek Console untuk error

### Problem: Trigger tidak detect player
**Solution:**
- Pastikan Vase punya **BoxCollider dengan isTrigger = true**
- Pastikan Player punya **Collider atau Rigidbody**
- Perbesar Interaction Radius di Inspector

### Problem: Canvas terlalu besar/kecil
**Solution:**
- World-space Canvas: Adjust **Scale** (misal 0.01)
- Screen-space Canvas: Adjust **Canvas Scaler** settings
- Adjust **RectTransform** size di tutorial image

---

## 🎯 TUTORIAL SPRITES LOCATION

Sprites tutorial ada di:
```
Assets/Sprites/Tutorial/
  ├─ 1.png
  ├─ 2.png
  ├─ 3.png
  ├─ 4.png
  ├─ 5.png
  ├─ 6.png
  └─ 7.png
```

**Cara Pakai:**
1. Buat Canvas untuk setiap tutorial (TutorialCanvas_1 sampai TutorialCanvas_7)
2. Setiap Canvas punya Image component
3. Drag sprite yang sesuai ke Source Image
4. Assign Canvas ke Vase yang berbeda

---

## ✅ CHECKLIST SETUP

### Per Vase:
- [ ] Vase GameObject created
- [ ] VaseTutorialInteract.cs attached
- [ ] Trigger Collider (isTrigger = true) - auto created atau manual
- [ ] InteractionPrompt (world-space UI) dibuat sebagai child
- [ ] Tutorial Canvas di-assign di Inspector
- [ ] Tutorial Canvas default inactive
- [ ] Test di Play Mode - "Press E" muncul
- [ ] Test E ditekan - Canvas muncul
- [ ] Test Close button - Canvas hilang

### Canvas Tutorial:
- [ ] Canvas GameObject created (Screen Space Overlay)
- [ ] Panel background added
- [ ] Tutorial Image added dengan sprite dari Assets/Sprites/Tutorial
- [ ] Close Button added
- [ ] CloseTutorialCanvas.cs attached to CloseButton
- [ ] OnClick event setup (CloseCanvas method)
- [ ] Canvas default inactive (unchecked)

---

## 🎓 EXAMPLE SCENE STRUCTURE

```
Tutorial Scene
├─ Player (Tag: Player)
│
├─ UICanvas ← Canvas utama (Screen Space - Overlay)
│  ├─ (UI elements lain: LimitPanel, PauseButton, dll)
│  ├─ TutorialCanvas_1 (inactive) ← Tutorial untuk Vase 1
│  │  ├─ Panel (background)
│  │  ├─ TutorialImage (Sprite: 1.png)
│  │  └─ CloseButton → CloseTutorialCanvas.cs
│  │
│  ├─ TutorialCanvas_2 (inactive) ← Tutorial untuk Vase 2
│  │  ├─ Panel (background)
│  │  ├─ TutorialImage (Sprite: 2.png)
│  │  └─ CloseButton → CloseTutorialCanvas.cs
│  │
│  ├─ TutorialCanvas_3 (inactive) ← Tutorial untuk Vase 3
│  └─ TutorialCanvas_4 (inactive) ← Tutorial untuk Vase 4
│
└─ Platform
   └─ Guide
      ├─ Vase_001
      │  ├─ (Vase 3D Model sudah ada)
      │  ├─ BoxCollider (isTrigger = true) ← Auto-created
      │  ├─ VaseTutorialInteract.cs
      │  │  → Tutorial Canvas: UICanvas/TutorialCanvas_1
      │  │  → Interaction Prompt: InteractionPrompt
      │  └─ InteractionPrompt (World-space UI)
      │     └─ Canvas (World Space)
      │        └─ PressEText ("Press E")
      │
      ├─ Vase_001 (1)
      │  ├─ (Vase 3D Model)
      │  ├─ BoxCollider (isTrigger = true)
      │  ├─ VaseTutorialInteract.cs
      │  │  → Tutorial Canvas: UICanvas/TutorialCanvas_2
      │  └─ InteractionPrompt
      │
      ├─ Vase_001 (2)
      │  └─ (Setup sama, Tutorial Canvas 3)
      │
      └─ Vase_001 (3)
         └─ (Setup sama, Tutorial Canvas 4)
```

---

## 🚀 QUICK START (TL;DR)

1. **Buat Tutorial Canvas di UICanvas:**
   - Select UICanvas → Right-click → UI → Panel → Rename "TutorialCanvas_1"
   - Tambahkan Image (drag sprite dari Sprites/Tutorial/1.png)
   - Tambahkan Button (Close) + CloseTutorialCanvas.cs
   - Set inactive (uncheck GameObject)

2. **Setup Vase (di Platform → Guide):**
   - Select Vase_001 (sudah ada di Platform/Guide)
   - Add Component → VaseTutorialInteract.cs
   - Assign Tutorial Canvas: UICanvas/TutorialCanvas_1
   - Buat InteractionPrompt (world-space UI "Press E")

3. **Test:**
   - Play → Dekat vase → Press E → Tutorial muncul → Close

4. **Repeat untuk 3 vase lainnya** (Vase_001 (1), (2), (3))

---

**Created:** January 3, 2026  
**Game:** ArgyrosPlatformer  
**Scene:** Tutorial  
**Version:** 1.0
