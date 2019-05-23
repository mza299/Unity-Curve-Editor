using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RouteMaster))]
public class RouteEditor : Editor {

    [SerializeField]
    float precision;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RouteMaster master = (RouteMaster)target;

        master.Init();
        //master.InitPrefab();

        //master.CreateNewRoute();

        if (GUILayout.Button("New Route"))
        {
            Debug.Log("Button Pressed");
            master.CreateNewRoute();
        }

        if (GUILayout.Button("Delete Newest Route"))
        {
            master.DeleteNewestRoute();
        }

        if (GUILayout.Button("Load precision points"))
        {
            foreach (var r in master.Routes)
            {
                r.precisionValue = master.precisionValue;
            }
        }
    }

}
