using UnityEngine;

/// <summary>
/// AUDIO MANAGER - Sistem Audio Utama untuk Game
/// 
/// Fungsi:
/// - Mengelola BGM (Background Music) untuk UI dan Level
/// - Mengelola SFX (Sound Effects) untuk tombol
/// - Singleton pattern (hanya ada 1 instance)
/// - Tidak hancur saat pindah scene (DontDestroyOnLoad)
/// 
/// Cara Pakai:
/// - Buat Empty GameObject di scene pertama, beri nama "AudioManager"
/// - Attach script ini
/// - Isi AudioClip di Inspector
/// - Panggil dari script lain: AudioManager.Instance.PlayUIMusic();
/// </summary>
public class AudioManager : MonoBehaviour
{
    // =====================================================
    // SINGLETON PATTERN
    // =====================================================
    // Instance adalah satu-satunya AudioManager yang ada di game
    public static AudioManager Instance { get; private set; }

    // =====================================================
    // AUDIO CLIPS (Isi di Inspector)
    // =====================================================
    [Header("=== BACKGROUND MUSIC ===")]
    [Tooltip("BGM untuk semua scene UI (menu, settings, dll)")]
    public AudioClip greekMusic;

    [Tooltip("BGM untuk semua scene Level (gameplay)")]
    public AudioClip levelMusic;

    [Header("=== SOUND EFFECTS ===")]
    [Tooltip("SFX untuk tombol di scene UI")]
    public AudioClip gameMenuButton;

    [Tooltip("SFX untuk tombol di scene Level")]
    public AudioClip buttonClick;

    [Tooltip("SFX untuk kemenangan (winning trigger)")]
    public AudioClip gameWin;

    // =====================================================
    // AUDIO SOURCES (Dibuat otomatis)
    // =====================================================
    [Header("=== AUDIO SOURCES (Otomatis) ===")]
    [Tooltip("AudioSource untuk memutar BGM")]
    public AudioSource bgmSource;

    [Tooltip("AudioSource untuk memutar SFX")]
    public AudioSource sfxSource;

    // =====================================================
    // STATUS TRACKER
    // =====================================================
    private SceneType currentSceneType = SceneType.UI;
    private bool isBGMPaused = false;
    
    // PlayerPrefs key untuk simpan state musik
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";

    // =====================================================
    // ENUM - Tipe Scene
    // =====================================================
    public enum SceneType
    {
        UI,     // Scene menu / UI
        Level   // Scene gameplay
    }

    // =====================================================
    // UNITY LIFECYCLE: AWAKE
    // =====================================================
    /// <summary>
    /// Dipanggil pertama kali saat script dijalankan.
    /// Setup singleton dan AudioSource.
    /// </summary>
    void Awake()
    {
        // Cek apakah sudah ada AudioManager lain
        if (Instance != null && Instance != this)
        {
            // Jika ada, hancurkan yang baru (karena yang lama sudah ada)
            Destroy(gameObject);
            return;
        }

        // Set sebagai Instance utama
        Instance = this;

        // Jangan hancurkan saat pindah scene
        DontDestroyOnLoad(gameObject);

        // Setup AudioSource
        SetupAudioSources();
        
        // Load music state dari PlayerPrefs (default: ON = 1)
        int musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1);
        if (musicEnabled == 0)
        {
            // Musik di-set OFF, pause BGM
            isBGMPaused = true;
        }

        Debug.Log("[AudioManager] Audio Manager berhasil diinisialisasi!");
    }

    // =====================================================
    // SETUP AUDIO SOURCES
    // =====================================================
    /// <summary>
    /// Membuat 2 AudioSource:
    /// 1. BGM Source - untuk background music
    /// 2. SFX Source - untuk sound effects
    /// </summary>
    private void SetupAudioSources()
    {
        // Buat AudioSource untuk BGM
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;  // BGM selalu loop
            bgmSource.playOnAwake = false;  // Jangan auto-play
            bgmSource.volume = 0.5f;  // Volume 50%
        }

        // Buat AudioSource untuk SFX
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;  // SFX tidak loop
            sfxSource.playOnAwake = false;
            sfxSource.volume = 0.7f;  // Volume 70%

            // PENTING: SFX tidak terpengaruh Time.timeScale (tetap bunyi saat pause)
            sfxSource.ignoreListenerPause = true;
        }
    }

    // =====================================================
    // FUNGSI: PLAY UI MUSIC
    // =====================================================
    /// <summary>
    /// Memutar musik untuk scene UI.
    /// Tidak restart jika sudah sedang memutar greekMusic.
    /// Respect PlayerPrefs music state.
    /// </summary>
    public void PlayUIMusic()
    {
        // Cek apakah greekMusic tersedia
        if (greekMusic == null)
        {
            Debug.LogWarning("[AudioManager] greekMusic belum di-assign di Inspector!");
            return;
        }

        // Update tipe scene
        currentSceneType = SceneType.UI;

        // Cek apakah musik di-enable dari PlayerPrefs
        int musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1);
        
        // Jika sudah memutar greekMusic dan masih playing, tidak perlu apa-apa
        if (bgmSource.clip == greekMusic && bgmSource.isPlaying)
        {
            Debug.Log("[AudioManager] UI Music sudah berjalan, tidak restart.");
            return;
        }
        
        // Jika clip sudah benar tapi di-pause, dan musik enabled, unpause saja
        if (bgmSource.clip == greekMusic && isBGMPaused && musicEnabled == 1)
        {
            bgmSource.UnPause();
            isBGMPaused = false;
            Debug.Log("[AudioManager] UI Music di-unpause.");
            return;
        }

        // Set clip
        bgmSource.clip = greekMusic;
        
        // Play hanya jika musik enabled
        if (musicEnabled == 1)
        {
            bgmSource.Play();
            isBGMPaused = false;
            Debug.Log("[AudioManager] Memutar UI Music (greekMusic)");
        }
        else
        {
            // Musik disabled, pastikan pause/stop
            if (bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
            isBGMPaused = true;
            Debug.Log("[AudioManager] UI Music di-set tapi tidak play (musik OFF)");
        }
    }

    // =====================================================
    // FUNGSI: PLAY LEVEL MUSIC
    // =====================================================
    /// <summary>
    /// Memutar musik untuk scene Level.
    /// Restart hanya jika belum memutar levelMusic.
    /// Respect PlayerPrefs music state.
    /// </summary>
    public void PlayLevelMusic()
    {
        // Cek apakah levelMusic tersedia
        if (levelMusic == null)
        {
            Debug.LogWarning("[AudioManager] levelMusic belum di-assign di Inspector!");
            return;
        }

        // Update tipe scene
        currentSceneType = SceneType.Level;
        
        // Cek apakah musik di-enable dari PlayerPrefs
        int musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1);

        // Jika sudah memutar levelMusic dan masih playing, tidak restart
        if (bgmSource.clip == levelMusic && bgmSource.isPlaying)
        {
            Debug.Log("[AudioManager] Level Music sudah berjalan, tidak restart.");
            return;
        }
        
        // Jika sudah memutar levelMusic DAN sedang pause, dan musik enabled, unpause saja
        if (bgmSource.clip == levelMusic && isBGMPaused && musicEnabled == 1)
        {
            bgmSource.UnPause();
            isBGMPaused = false;
            Debug.Log("[AudioManager] Level Music di-unpause.");
            return;
        }

        // Set clip
        bgmSource.clip = levelMusic;
        
        // Play hanya jika musik enabled
        if (musicEnabled == 1)
        {
            bgmSource.Play();
            isBGMPaused = false;
            Debug.Log("[AudioManager] Memutar Level Music (levelMusic) dari awal");
        }
        else
        {
            // Musik disabled, pastikan pause/stop
            if (bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
            isBGMPaused = true;
            Debug.Log("[AudioManager] Level Music di-set tapi tidak play (musik OFF)");
        }
    }

    // =====================================================
    // FUNGSI: PAUSE BGM
    // =====================================================
    /// <summary>
    /// Pause BGM (dipakai saat game di-pause).
    /// BGM akan berhenti TANPA restart.
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
            isBGMPaused = true;
            Debug.Log("[AudioManager] BGM di-pause");
        }
    }

    // =====================================================
    // FUNGSI: RESUME BGM
    // =====================================================
    /// <summary>
    /// Resume BGM dari posisi terakhir (dipakai saat game di-resume).
    /// BGM TIDAK restart, melanjutkan dari posisi terakhir.
    /// </summary>
    public void ResumeBGM()
    {
        if (isBGMPaused)
        {
            bgmSource.UnPause();
            isBGMPaused = false;
            Debug.Log("[AudioManager] BGM di-resume");
        }
    }

    // =====================================================
    // FUNGSI: STOP BGM
    // =====================================================
    /// <summary>
    /// Stop BGM sepenuhnya.
    /// Jika di-play lagi, akan restart dari awal.
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
        isBGMPaused = false;
        Debug.Log("[AudioManager] BGM di-stop");
    }

    // =====================================================
    // FUNGSI: TOGGLE BGM ON/OFF
    // =====================================================
    /// <summary>
    /// Toggle BGM on/off.
    /// Jika BGM sedang playing, akan pause.
    /// Jika BGM sedang pause, akan unpause dan play.
    /// 
    /// Return: true jika BGM enabled (playing), false jika disabled (paused)
    /// </summary>
    public bool ToggleBGM()
    {
        // Safeguard: jika AudioSource belum ada, return false
        if (bgmSource == null)
        {
            Debug.LogWarning("[AudioManager] BGM AudioSource belum ada!");
            return false;
        }

        bool isCurrentlyPlaying = bgmSource.isPlaying;
        
        if (isCurrentlyPlaying)
        {
            // User ingin MATIKAN musik
            // Simpan state ke PlayerPrefs DULU
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, 0);
            PlayerPrefs.Save();
            
            // Pause BGM
            bgmSource.Pause();
            isBGMPaused = true;
            
            Debug.Log("[AudioManager] BGM dimatikan");
            return false;
        }
        else
        {
            // User ingin NYALAKAN musik
            // Simpan state ke PlayerPrefs DULU
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, 1);
            PlayerPrefs.Save();
            
            // PENTING: Gunakan currentSceneType yang sudah di-set oleh SceneAudioInitializer
            // Ini memastikan musik yang dimainkan sesuai scene saat ini, bukan clip lama
            if (currentSceneType == SceneType.UI)
            {
                PlayUIMusic();
                Debug.Log("[AudioManager] BGM dinyalakan (UI Music)");
            }
            else
            {
                PlayLevelMusic();
                Debug.Log("[AudioManager] BGM dinyalakan (Level Music)");
            }
            
            return true;
        }
    }

    // =====================================================
    // FUNGSI: SET BGM ENABLED
    // =====================================================
    /// <summary>
    /// Set BGM enabled/disabled.
    /// 
    /// Parameter:
    /// - enabled: true = nyalakan BGM, false = matikan BGM
    /// </summary>
    public void SetBGMEnabled(bool enabled)
    {
        if (enabled)
        {
            if (!bgmSource.isPlaying)
            {
                if (bgmSource.clip != null)
                {
                    // Jika ada clip, unpause
                    bgmSource.UnPause();
                }
                else
                {
                    // Jika belum ada clip, play musik sesuai scene type
                    if (currentSceneType == SceneType.UI)
                        PlayUIMusic();
                    else
                        PlayLevelMusic();
                }
            }
        }
        else
        {
            if (bgmSource.isPlaying)
            {
                bgmSource.Pause();
            }
        }
    }

    // =====================================================
    // FUNGSI: IS BGM ENABLED
    // =====================================================
    /// <summary>
    /// Cek apakah BGM sedang enabled (playing).
    /// 
    /// Return: true jika BGM enabled (playing), false jika disabled (paused/stopped)
    /// </summary>
    public bool IsBGMEnabled()
    {
        return bgmSource.isPlaying;
    }

    // =====================================================
    // FUNGSI: PLAY BUTTON SFX
    // =====================================================
    /// <summary>
    /// Memutar SFX tombol.
    /// 
    /// Parameter:
    /// - isUI: true = tombol di scene UI, false = tombol di scene Level
    /// </summary>
    public void PlayButtonSFX(bool isUI)
    {
        AudioClip clipToPlay = isUI ? gameMenuButton : buttonClick;

        if (clipToPlay == null)
        {
            Debug.LogWarning($"[AudioManager] SFX untuk {(isUI ? "UI" : "Level")} button belum di-assign!");
            return;
        }

        // PlayOneShot = play SFX tanpa mengganggu SFX lain
        sfxSource.PlayOneShot(clipToPlay);
    }

    // =====================================================
    // FUNGSI: PLAY SFX (CUSTOM)
    // =====================================================
    /// <summary>
    /// Memutar SFX custom (bisa untuk efek lain selain tombol).
    /// 
    /// Parameter:
    /// - clip: AudioClip yang ingin dimainkan
    /// - volume: Volume (0.0 - 1.0), default 1.0
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] AudioClip tidak tersedia!");
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }

    // =====================================================
    // FUNGSI: STOP ALL SFX
    // =====================================================
    /// <summary>
    /// Stop semua SFX yang sedang berjalan.
    /// Berguna untuk cleanup saat cutscene selesai.
    /// </summary>
    public void StopAllSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
            Debug.Log("[AudioManager] Semua SFX dihentikan");
        }
    }

    // =====================================================
    // FUNGSI: GET CURRENT SCENE TYPE
    // =====================================================
    /// <summary>
    /// Mendapatkan tipe scene saat ini (UI atau Level).
    /// Berguna untuk script lain yang perlu tahu scene type.
    /// </summary>
    public SceneType GetCurrentSceneType()
    {
        return currentSceneType;
    }

    // =====================================================
    // FUNGSI: SET VOLUME
    // =====================================================
    /// <summary>
    /// Mengatur volume BGM.
    /// 
    /// Parameter:
    /// - volume: 0.0 (mute) sampai 1.0 (max)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Mengatur volume SFX.
    /// 
    /// Parameter:
    /// - volume: 0.0 (mute) sampai 1.0 (max)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
