using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RouteFollower))]
public class RouteFollowerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var master = (RouteFollower)target;

        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 15;
        centeredStyle.fontStyle = FontStyle.Bold;
        centeredStyle.richText = true;

        GUILayout.Space(15);

        GUILayout.Label("<color=#a94064>AI Management</color>", centeredStyle);
        GUILayout.Label("<size=12><color=#a94064>Pick the relevant AITYPE above. Use RouteMaster to change all</color></size>", centeredStyle);

        GUILayout.Space(10);

        if (GUILayout.Button("Change AI Type"))
        {
            master.ManageAIDictionaryForEditor();
            master.ReplaceAIInEditor();
        }


    }
}
