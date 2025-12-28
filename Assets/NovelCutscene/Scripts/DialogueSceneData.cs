using System;
using UnityEngine;

/// <summary>
/// ScriptableObject untuk menyimpan data dialog visual novel cutscene
/// Setiap asset mewakili 1 scene cutscene dengan background dan array dialog
/// </summary>
[CreateAssetMenu(fileName = "NewDialogueScene", menuName = "Novel Cutscene/Dialogue Scene Data", order = 1)]
public class DialogueSceneData : ScriptableObject
{
    [Header("Visual Settings")]
    [Tooltip("Background sprite untuk scene ini (36.png - 39.png)")]
    public Sprite backgroundSprite;

    [Header("Dialogue Content")]
    [Tooltip("Array teks dialog untuk scene ini. 1 scene bisa punya 1-2 dialog")]
    [TextArea(3, 10)]
    public string[] dialogues;

    [Header("Optional Settings")]
    [Tooltip("Nama karakter yang berbicara (optional, kosongkan jika narrator)")]
    public string characterName = "Narrator";

    [Tooltip("Kecepatan typing effect (detik per karakter). Default: 0.05f")]
    [Range(0.01f, 0.1f)]
    public float typingSpeed = 0.05f;

    [Tooltip("BGM khusus untuk scene ini (optional, kosongkan jika pakai BGM level)")]
    public AudioClip cutsceeneBGM;

    [Tooltip("SFX typing sound (optional, untuk efek typewriter)")]
    public AudioClip typingSFX;

    /// <summary>
    /// Validasi data saat di Inspector
    /// </summary>
    private void OnValidate()
    {
        // Pastikan ada minimal 1 dialog
        if (dialogues == null || dialogues.Length == 0)
        {
            dialogues = new string[] { "Enter dialogue here..." };
        }

        // Clamp typing speed
        if (typingSpeed < 0.01f) typingSpeed = 0.01f;
        if (typingSpeed > 0.1f) typingSpeed = 0.1f;

        // Set default character name jika kosong
        if (string.IsNullOrEmpty(characterName))
        {
            characterName = "Narrator";
        }
    }
}
