using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveObjectToNextIncr))]
class MoveObjectToNextIncrEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = target as MoveObjectToNextIncr;

        if (GUILayout.Button("INCR"))
            script.Move();
    }
}