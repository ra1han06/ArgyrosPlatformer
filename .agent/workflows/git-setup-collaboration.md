---
description: Step-by-step guide untuk push Unity project ke Git dan setup kolaborasi
---

# 🚀 Guide: Push ArgyrosPlatformer ke Git untuk Kolaborasi

## Prerequisites
1. Git sudah terinstall di komputer Anda (cek dengan `git --version`)
2. Akun GitHub/GitLab/Bitbucket sudah dibuat
3. Repository sudah dibuat di platform Git pilihan Anda

---

## 📋 Step-by-Step Process

### Step 1: Verifikasi Git Installation
Pastikan Git sudah terinstall:
```bash
git --version
```
Jika belum, download dari: https://git-scm.com/downloads

---

### Step 2: Konfigurasi Git (Jika Belum)
Set username dan email Anda (hanya perlu dilakukan sekali):
```bash
git config --global user.name "Nama Anda"
git config --global user.email "email@example.com"
```

---

### Step 3: Initialize Git Repository
Navigate ke folder project dan initialize Git:
```bash
cd c:\ArgyrosPlatformer
git init
```

---

### Step 4: Verifikasi .gitignore
File `.gitignore` sudah dibuat dan berisi:
- ✅ Library/ (Unity generated)
- ✅ Temp/ (Unity temporary files)
- ✅ Obj/ (Build artifacts)
- ✅ Builds/ (Compiled builds)
- ✅ Logs/ (Log files)
- ✅ UserSettings/ (User-specific settings)
- ✅ .vs/, .vscode/, .idea/ (IDE files)
- ✅ *.csproj, *.sln, *.slnx (Generated project files)
- ✅ OS-specific files (Thumbs.db, .DS_Store, dll)

**PENTING:** File-file ini akan diabaikan oleh Git, sehingga:
- ✅ Teman Anda tidak akan mendapat file yang tidak diperlukan
- ✅ Unity akan auto-generate ulang file-file ini saat project dibuka
- ✅ Kompatibel untuk Windows & macOS

---

### Step 5: Add All Files to Staging
Add semua files yang akan di-commit (kecuali yang ada di .gitignore):
```bash
git add .
```

Cek status untuk memastikan file yang benar:
```bash
git status
```

**Yang HARUS terlihat:**
- ✅ Assets/ (dan semua isinya termasuk .meta files)
- ✅ ProjectSettings/
- ✅ Packages/
- ✅ .gitignore

**Yang TIDAK BOLEH terlihat:**
- ❌ Library/
- ❌ Temp/
- ❌ Logs/
- ❌ UserSettings/
- ❌ .vs/, .vscode/
- ❌ *.csproj, *.sln files

---

### Step 6: Create First Commit
Buat commit pertama dengan pesan yang jelas:
```bash
git commit -m "Initial commit: ArgyrosPlatformer Unity project"
```

---

### Step 7: Create GitHub Repository
1. Buka https://github.com (atau GitLab/Bitbucket)
2. Login ke akun Anda
3. Klik tombol **"New Repository"** atau **"+"**
4. Isi details:
   - **Repository name:** ArgyrosPlatformer (atau nama lain)
   - **Description:** "2.5D Puzzle Platformer - The Last Light of Argyros"
   - **Visibility:** Public atau Private (pilih Private untuk kolaborasi team)
   - **JANGAN** centang "Add README" atau ".gitignore" (kita sudah punya)
5. Klik **"Create Repository"**

---

### Step 8: Connect Local Repository ke Remote
Copy URL repository Anda (biasanya: `https://github.com/username/ArgyrosPlatformer.git`)

Tambahkan remote origin:
```bash
git remote add origin https://github.com/username/ArgyrosPlatformer.git
```

Ganti `username` dengan username GitHub Anda!

Verifikasi remote sudah terhubung:
```bash
git remote -v
```

---

### Step 9: Rename Branch ke 'main' (Recommended)
GitHub sekarang menggunakan 'main' sebagai default branch:
```bash
git branch -M main
```

---

### Step 10: Push ke GitHub
Push commit pertama Anda ke remote repository:
```bash
git push -u origin main
```

**Note:** 
- Anda mungkin diminta login ke GitHub
- Flag `-u` membuat tracking branch sehingga next time cukup `git push`

---

## 👥 Setup Kolaborasi dengan Teman

### Option 1: GitHub Collaborators (untuk Private Repo)
1. Buka repository di GitHub
2. Klik **Settings**
3. Klik **Collaborators** di sidebar
4. Klik **"Add people"**
5. Masukkan username/email GitHub teman Anda
6. Teman Anda akan dapat email invitation dan bisa accept

### Option 2: Buat Organization (untuk Team)
1. Buat GitHub Organization (free untuk public repos)
2. Invite team members
3. Set permissions (Write access untuk collaborators)

---

## 📥 Clone Repository (Untuk Teman Anda)

Teman Anda bisa clone dengan:
```bash
git clone https://github.com/username/ArgyrosPlatformer.git
cd ArgyrosPlatformer
```

**Setelah clone, teman Anda harus:**
1. Buka project dengan Unity Editor (Unity akan auto-generate Library/, Temp/, dll)
2. Tunggu Unity selesai import semua assets
3. Project siap digunakan!

**✅ GUARANTEED:** 
- Tidak ada file mandatory yang hilang
- `.gitignore` sudah memastikan hanya file penting yang di-track
- Working 100% di Windows & macOS

---

## 🔄 Workflow Kolaborasi Sehari-hari

### Sebelum Mulai Coding (Pull Changes)
```bash
git pull origin main
```

### Setelah Selesai Coding (Push Changes)
```bash
git add .
git commit -m "Deskripsi perubahan yang jelas"
git push origin main
```

### Best Practices untuk Commit Message:
- ✅ "Add player double jump mechanic"
- ✅ "Fix raycast detection for Ctrl+C copy platform"
- ✅ "Update UI operation limit display"
- ❌ "update" (terlalu vague)
- ❌ "fix bug" (tidak jelas bug apa)

---

## 🌿 Recommended: Git Branching Strategy

Untuk kolaborasi yang lebih advanced, gunakan branches:

### Create Feature Branch
```bash
git checkout -b feature/player-animations
```

### Work on Feature
```bash
git add .
git commit -m "Add idle and run animations"
git push origin feature/player-animations
```

### Merge via Pull Request
1. Buat Pull Request di GitHub
2. Teman review code Anda
3. Merge setelah approved
4. Delete branch setelah merge

### Back to Main Branch
```bash
git checkout main
git pull origin main
```

---

## 🛠️ Troubleshooting Common Issues

### Issue 1: "Library/ too large" Error
**Solution:** Sudah diatasi! `.gitignore` kita mengecualikan Library/

### Issue 2: Merge Conflicts di .meta files
**Prevention:**
- Selalu `git pull` sebelum mulai kerja
- Jangan edit file yang sama secara bersamaan
- Komunikasi dengan team

**Solution jika terjadi:**
```bash
git status  # lihat conflicted files
# Edit file manually, atau:
git checkout --theirs path/to/file.meta  # gunakan versi mereka
git checkout --ours path/to/file.meta    # gunakan versi kita
git add .
git commit
```

### Issue 3: Unity tidak recognize changes setelah pull
**Solution:**
1. Close Unity Editor
2. `git pull`
3. Buka Unity Editor lagi (akan auto-reimport)

### Issue 4: Accidentally committed Library/ or Temp/
**Solution:**
```bash
git rm -r --cached Library/
git rm -r --cached Temp/
git commit -m "Remove ignored files from tracking"
git push
```

---

## 📝 Files Yang WAJIB Di-Track (Sudah Dijamin oleh .gitignore)

✅ **Assets/** - Semua game assets Anda
✅ **Packages/manifest.json** - Unity package dependencies
✅ **Packages/packages-lock.json** - Lock file untuk packages
✅ **ProjectSettings/** - Project configuration
✅ **.gitignore** - Git ignore rules

---

## 🎯 Summary Checklist

- [ ] Git installed dan configured
- [ ] Repository created di GitHub/GitLab
- [ ] `.gitignore` sudah ada dan benar
- [ ] `git init` executed
- [ ] First commit created
- [ ] Remote origin added
- [ ] Pushed to remote successfully
- [ ] Collaborators invited (jika private repo)
- [ ] Teman sudah bisa clone dan run project

---

## 💡 Pro Tips

1. **Commit Often:** Buat commits kecil dan sering, bukan 1 commit besar
2. **Clear Messages:** Gunakan commit messages yang jelas dan descriptive
3. **Pull Before Push:** Selalu pull changes terbaru sebelum push
4. **Don't Commit Personal Settings:** `.gitignore` sudah handle ini
5. **Use Branches:** Untuk features besar, gunakan branches
6. **Scene Collaboration:** Hati-hati dengan .unity scene files, bisa conflict
7. **Use Git LFS (Optional):** Untuk assets besar (textures, audio, models)

---

## 🔗 Useful Resources

- Git Documentation: https://git-scm.com/doc
- GitHub Guides: https://guides.github.com
- Unity Version Control Best Practices: https://unity.com/how-to/version-control-systems
- Gitignore for Unity: https://github.com/github/gitignore/blob/main/Unity.gitignore

---

**🎉 Selamat! Project Anda sekarang siap untuk kolaborasi!**
