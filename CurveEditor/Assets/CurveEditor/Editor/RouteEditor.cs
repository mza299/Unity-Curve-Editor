using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RouteMaster)), CanEditMultipleObjects]
public class RouteEditor : Editor{

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

        if (GUILayout.Button("Reload precision points"))
        {
            foreach (var r in master.Routes)
            {
                r.precisionValue = master.precisionValue;
            }
        }

        if (GUILayout.Button("Populate Curve"))
        {
            master.SpawnObject();
        }

        if (master.isEraseMode == false)
        {
            if (GUILayout.Button("ENABLE Erase Mode"))
            {
                master.isEraseMode = true;
            }
        }
        else
        {
            if (GUILayout.Button("DISABLE Erase Mode"))
            {
                master.isEraseMode = false;
            }
        }

    }

    protected virtual void OnSceneGUI()
    {
        RouteMaster master = (RouteMaster)target;

        //if (master.isEraseMode == true)
        //{
        //    var handleSize = 1f;
        //    var snap = Vector3.one * 0.1f;

        //    EditorGUI.BeginChangeCheck();
        //    Vector3 EraserPos = Handles.FreeMoveHandle((master.transform.position + Vector3.left*5f), Quaternion.identity, handleSize, snap,
        //            Handles.SphereHandleCap);

        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        Undo.RecordObject(master, "Eraser stuff");
        //        EraserPos = HandleUtility.WorldToGUIPoint(Input.mousePosition);
        //    }
        //}

        if (master.Routes.Count > 1)
        {
            List<Route> routes = new List<Route>();

            routes.AddRange(master.Routes);

            for (int i = 0; i < routes.Count; i++)
            {
                var handleSize = 3f;
                var snap = Vector3.one * 0.1f;

                Handles.color = Color.cyan;

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
