using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
public class GameManagerEditor : TweakableEditor
{
    SerializedProperty bezierInterpolationRange;
    SerializedProperty pathFindingRate;
    SerializedProperty aiReachingPrecision;

    void OnEnable()
    {
        bezierInterpolationRange = serializedObject.FindProperty("bezierInterpolationRange");
        pathFindingRate = serializedObject.FindProperty("pathFindingRate");
        aiReachingPrecision = serializedObject.FindProperty("aiReachingPrecision");
    }

    protected override void OnBeforeDefaultInspector()
    {
        EditorGUILayout.LabelField("Bezier Interpolation Range: ", EditorStyles.boldLabel);
        EditorGUILayout.Slider(bezierInterpolationRange, 0.1f, 1, "Value:");
        EditorGUILayout.HelpBox("This variable effects the bezier curves of the enemies. 0.3 is the recommended value.", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Enemy pathfinding Rate: ", EditorStyles.boldLabel);
        EditorGUILayout.Slider(pathFindingRate, 0.05f, 1, "Value:");
        EditorGUILayout.HelpBox("This is the delay between two consecutive pathfinding requests by an enemy. 0.3 is the recommended value.", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("AI reaching precision: ", EditorStyles.boldLabel);
        EditorGUILayout.Slider(aiReachingPrecision, 0.05f, 1, "Value:");
        EditorGUILayout.HelpBox("This variable is used for the precision of an AI reaching a certain point after pathfinding. 0.1 is the recommended value.", MessageType.Info);
        EditorGUILayout.Space();
    }

    protected override string[] GetInvisibleInDefaultInspector()
    {
        return new[] { "m_Script" };
    }

}
