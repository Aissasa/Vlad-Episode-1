using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class TweakableEditor : Editor
{
    private static readonly string[] invisibleInDefault = new string[0];

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        OnBeforeDefaultInspector();
        DrawPropertiesExcluding(serializedObject, GetInvisibleInDefaultInspector());
        OnAfterDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void OnBeforeDefaultInspector()
    { }

    protected virtual void OnAfterDefaultInspector()
    { }

    protected virtual string[] GetInvisibleInDefaultInspector()
    {
        return invisibleInDefault;
    }
}
