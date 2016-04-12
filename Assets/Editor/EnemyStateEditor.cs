using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyStateHandler))]
[CanEditMultipleObjects]
public class EnemyStateEditor : TweakableEditor
{

    SerializedProperty attackRange;
    SerializedProperty chasingRange;
    SerializedProperty pursuitRange;

    void OnEnable()
    {
        attackRange = serializedObject.FindProperty("attackRange");
        chasingRange = serializedObject.FindProperty("chasingRange");
        pursuitRange = serializedObject.FindProperty("pursuitRange");
    }

    protected override void OnBeforeDefaultInspector()
    {
        EditorGUILayout.Slider(attackRange, 0.05f, 3, "Attack Range");
        EditorGUILayout.Slider(chasingRange, 0.5f, 5, "Chasing Range");
        EditorGUILayout.Slider(pursuitRange, 1f, 8, "Pursuit Range");
        if (chasingRange.floatValue < attackRange.floatValue)
        {
            chasingRange.floatValue = attackRange.floatValue;
        }
        if (pursuitRange.floatValue < chasingRange.floatValue)
        {
            pursuitRange.floatValue = chasingRange.floatValue;
        }

    }

    protected override string[] GetInvisibleInDefaultInspector()
    {
        return new[] { "m_Script" };
    }
}
