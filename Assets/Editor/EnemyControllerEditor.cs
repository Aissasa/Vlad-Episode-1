using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
[CanEditMultipleObjects]
public class EnemyControllerEditor : TweakableEditor {

    SerializedProperty attackRange;
    SerializedProperty chasingRange;

    void OnEnable()
    {
        attackRange = serializedObject.FindProperty("attackRange");
        chasingRange = serializedObject.FindProperty("chasingRange");
    }

    protected override void OnBeforeDefaultInspector()
    {
        EditorGUILayout.Slider(attackRange, 0.05f, 3, "Attack Range");
        EditorGUILayout.Slider(chasingRange, 0.5f, 5, "Chasing Range");
        if (chasingRange.floatValue < attackRange.floatValue)
        {
            chasingRange.floatValue = attackRange.floatValue;
        }
    }

    protected override string[] GetInvisibleInDefaultInspector()
    {
        return new[] { "m_Script" };
    }
}
