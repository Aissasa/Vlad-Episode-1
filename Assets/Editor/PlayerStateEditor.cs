using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerLogic.PlayerStateHandler))]
[CanEditMultipleObjects]
public class PlayerStateEditor : TweakableEditor {

    protected override string[] GetInvisibleInDefaultInspector()
    {
        return new[] { "m_Script" };
    }

}
