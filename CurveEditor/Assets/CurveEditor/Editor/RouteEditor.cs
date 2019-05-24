using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RouteMaster)), CanEditMultipleObjects]
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

    protected virtual void OnSceneGUI()
    {
        RouteMaster master = (RouteMaster)target;

        if (master.Routes.Count > 1)
        {
            List<Route> routes = new List<Route>();

            routes.AddRange(master.Routes);

            for (int i = 0; i < routes.Count; i++)
            {
                var handleSize = HandleUtility.GetHandleSize(Vector3.one * 0.2f);
                var snap = Vector3.one * 0.1f;

                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPos = Handles.FreeMoveHandle(routes[i].controlPoints[3].position, Quaternion.identity, handleSize, snap,
                    Handles.SphereHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(master, "Change Look At Target Position");
                    routes[i].controlPoints[3].position = newTargetPos;
                    if (routes[i-1] != null)
                        routes[i - 1].controlPoints[0].position = newTargetPos;
                }
            }
        }
    }

}
