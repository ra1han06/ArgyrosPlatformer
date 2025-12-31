using TMPro;
using UnityEngine;

/// <summary>
/// Simple world-space label that can optionally billboard toward the main camera.
/// Designed for "FINISH" markers, etc.
/// 
/// Usage:
/// - Create a child GameObject above the target.
/// - Add TextMeshPro (3D) component.
/// - Add this component; assign the TMP reference.
/// </summary>
[DisallowMultipleComponent]
public class WorldBillboardLabel : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private string message = "FINISH";

    [Header("Camera")]
    [Tooltip("Optional explicit camera to face (useful during intro sequences if MainCamera tag changes).")]
    [SerializeField] private Camera cameraOverride;

    [Header("Layout")]
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 2f, 0f);

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool lockPitch = true;
    [SerializeField] private bool lockRoll = true;

    // We treat localOffset as an ADDITIVE offset relative to whatever the author placed in the scene.
    // This avoids fighting manual positioning or parent transforms.
    private Vector3 _baseLocalPosition;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        _baseLocalPosition = transform.localPosition;

        ApplyText();
    }

    private void LateUpdate()
    {
        // Keep offset stable even if the parent animates/moves.
        transform.localPosition = _baseLocalPosition + localOffset;

        if (!faceCamera)
            return;

        var cam = ResolveCamera();
        if (cam == null) return;

        Vector3 dir = transform.position - cam.transform.position;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        Quaternion look = Quaternion.LookRotation(dir);

        Vector3 e = look.eulerAngles;
        if (lockPitch) e.x = 0f;
        if (lockRoll) e.z = 0f;

        transform.rotation = Quaternion.Euler(e);
    }

    private void OnValidate()
    {
        ApplyText();

        // In edit mode, keep the label preview consistent.
        // We don't want to *reset* the base position every frame—only when the component is edited.
        _baseLocalPosition = transform.localPosition - localOffset;
    }

    private void ApplyText()
    {
        if (text == null)
            return;

        text.text = message;
    }

    private Camera ResolveCamera()
    {
        if (cameraOverride != null && cameraOverride.isActiveAndEnabled)
            return cameraOverride;

        var main = Camera.main;
        if (main != null && main.isActiveAndEnabled)
            return main;

        // Fallback: during some sequences the "MainCamera" tag can be missing or swapped.
        // Pick any enabled camera so the label still faces what the player is seeing.
        // (Camera.allCameras is already filtered to active cameras).
        var cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            var c = cams[i];
            if (c != null && c.isActiveAndEnabled)
                return c;
        }

        return null;
    }
}
