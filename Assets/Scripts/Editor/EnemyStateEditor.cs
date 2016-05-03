using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyAI.EnemyStateHandler))]
[CanEditMultipleObjects]
public class EnemyStateEditor : TweakableEditor
{
    SerializedProperty attackRange;
    SerializedProperty attackReach;
    SerializedProperty chasingRange;
    SerializedProperty pursuitRange;

    void OnEnable()
    {
        attackRange = serializedObject.FindProperty("attackRange");
        attackReach = serializedObject.FindProperty("attackReach");
        chasingRange = serializedObject.FindProperty("chasingRange");
        pursuitRange = serializedObject.FindProperty("pursuitRange");
    }

    protected override void OnAfterDefaultInspector()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Transition ranges:", EditorStyles.boldLabel);
        EditorGUILayout.Slider(attackReach, 0.05f, 4, "Attack Reach");
        EditorGUILayout.Slider(attackRange, 0.02f, 3, "Attack Range");
        EditorGUILayout.Slider(chasingRange, 0.5f, 5, "Chasing Range");
        EditorGUILayout.Slider(pursuitRange, 1f, 8, "Pursuit Range");
        EditorGUILayout.Space();
        if (attackReach.floatValue < attackRange.floatValue)
        {
            attackReach.floatValue = attackRange.floatValue;
        }
        if (chasingRange.floatValue < attackRange.floatValue)
        {
            chasingRange.floatValue = attackRange.floatValue;
        }
        if (pursuitRange.floatValue < chasingRange.floatValue)
        {
            pursuitRange.floatValue = chasingRange.floatValue;
        }
        if (pursuitRange.floatValue < attackReach.floatValue)
        {
            pursuitRange.floatValue = attackReach.floatValue;
        }

    }

    protected override string[] GetInvisibleInDefaultInspector()
    {
        return new[] { "m_Script" };
    }
}
