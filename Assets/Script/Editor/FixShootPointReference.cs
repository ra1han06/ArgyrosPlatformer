using UnityEngine;
using UnityEditor;

public class FixShootPointReference : EditorWindow
{
    [MenuItem("Tools/Fix Enemy ShootPoint Reference")]
    static void Fix()
    {
        GameObject enemy = GameObject.Find("Enemy");
        if (enemy == null)
        {
            Debug.LogError("Enemy not found!");
            return;
        }

        EnemyShooter shooter = enemy.GetComponent<EnemyShooter>();
        if (shooter == null)
        {
            Debug.LogError("EnemyShooter not found!");
            return;
        }

        Transform shootPoint = enemy.transform.Find("ShootPoint");
        if (shootPoint == null)
        {
            Debug.LogError("ShootPoint child not found!");
            return;
        }

        SerializedObject so = new SerializedObject(shooter);
        so.FindProperty("shootPoint").objectReferenceValue = shootPoint;
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(shooter);
        Debug.Log($"✅ ShootPoint reference fixed! Now points to: {shootPoint.name} at position {shootPoint.localPosition}");
    }
}
