using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerObject))]
public class PlayerObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerObject playerObject = (PlayerObject)target;
        GUILayout.Space(10);
        if (GUILayout.Button("Reset Player"))
        {
            playerObject.ResetPlayer();
        }
    }
}
