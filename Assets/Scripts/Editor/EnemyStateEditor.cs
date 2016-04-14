using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnemyAI.EnemyStateHandler))]
[CanEditMultipleObjects]
public class EnemyStateEditor : TweakableEditor
{
    SerializedProperty attackRange;
    SerializedProperty chasingRange;
    SerializedProperty pursuitRange;
   // SerializedProperty displayPath;

    // bools
    //bool showDebug = true;

    void OnEnable()
    {
        attackRange = serializedObject.FindProperty("attackRange");
        chasingRange = serializedObject.FindProperty("chasingRange");
        pursuitRange = serializedObject.FindProperty("pursuitRange");
        //displayPath = serializedObject.FindProperty("displayPath");
    }

    protected override void OnAfterDefaultInspector()
    {
        //showDebug = EditorGUILayout.Foldout(showDebug, "For debugging purposes :");
        //if (showDebug)
        //{
        //    EditorGUILayout.Toggle("Display Path", displayPath.boolValue);
        //    EditorGUILayout.Space();
        //}
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Transition ranges:", EditorStyles.boldLabel);
        EditorGUILayout.Slider(attackRange, 0.05f, 3, "Attack Range");
        EditorGUILayout.Slider(chasingRange, 0.5f, 5, "Chasing Range");
        EditorGUILayout.Slider(pursuitRange, 1f, 8, "Pursuit Range");
        EditorGUILayout.Space();
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
