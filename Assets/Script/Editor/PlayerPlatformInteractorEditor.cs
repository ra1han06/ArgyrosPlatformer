using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerPlatformInteractor))]
public class PlayerPlatformInteractorEditor : Editor
{
    private void OnSceneGUI()
    {
        PlayerPlatformInteractor interactor = (PlayerPlatformInteractor)target;
        
        // Ambil SerializedObject untuk akses ke private fields
        SerializedObject so = new SerializedObject(interactor);
        SerializedProperty offsetProp = so.FindProperty("raycastOffset");
        SerializedProperty raycastPointProp = so.FindProperty("raycastPoint");
        SerializedProperty boxSizeProp = so.FindProperty("boxCastSize");
        
        // Skip jika pakai Transform reference
        if (raycastPointProp.objectReferenceValue != null) return;
        
        Vector3 currentOffset = offsetProp.vector3Value;
        Vector3 boxSize = boxSizeProp.vector3Value;
        Vector3 playerPosition = interactor.transform.position;
        Vector3 rayOrigin = playerPosition + currentOffset;
        
        // GAMBAR BOX MERAH DI ORIGIN (visual lebih jelas)
        Handles.color = new Color(1f, 0f, 0f, 0.3f);
        Handles.DrawWireCube(rayOrigin, boxSize);
        
        // SPHERE KUNING DI TENGAH BOX - INI YANG BISA DI-DRAG!
        Handles.color = Color.yellow;
        float handleSize = HandleUtility.GetHandleSize(rayOrigin) * 0.15f;
        
        EditorGUI.BeginChangeCheck();
        var fmh_36_13_639024815345905034 = Quaternion.identity; Vector3 newRayOrigin = Handles.FreeMoveHandle(
            rayOrigin,
            handleSize,
            Vector3.zero,
            Handles.SphereHandleCap
        );
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(interactor, "Change Raycast Offset");
            Vector3 newOffset = newRayOrigin - playerPosition;
            offsetProp.vector3Value = newOffset;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(interactor);
        }
        
        // PANAH UNTUK GERAK X, Y, Z (lebih mudah kontrol per axis!)
        Handles.color = Color.cyan;
        EditorGUI.BeginChangeCheck();
        Vector3 newPosWithArrows = Handles.PositionHandle(rayOrigin, Quaternion.identity);
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(interactor, "Change Raycast Offset");
            Vector3 newOffset = newPosWithArrows - playerPosition;
            offsetProp.vector3Value = newOffset;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(interactor);
        }
        
        // Garis dari player ke raycast origin
        Handles.color = Color.cyan;
        Handles.DrawDottedLine(playerPosition, rayOrigin, 3f);
        
        // Label instruksi BESAR
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.yellow;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 12;
        Handles.Label(rayOrigin + Vector3.up * (boxSize.y * 0.5f + 0.3f), 
                     "DRAG PANAH/BOLA INI!", 
                     labelStyle);
        
        // Info offset
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 10;
        Handles.Label(rayOrigin + Vector3.down * (boxSize.y * 0.5f + 0.2f), 
                     $"Offset: ({currentOffset.x:F2}, {currentOffset.y:F2}, {currentOffset.z:F2})", 
                     labelStyle);
    }
}
