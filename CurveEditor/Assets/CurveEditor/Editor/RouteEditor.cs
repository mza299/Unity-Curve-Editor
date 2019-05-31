using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(RouteMaster)), CanEditMultipleObjects]
public class RouteEditor : Editor{

    [SerializeField]
    float precision;

    [MenuItem("Besier Curves/Create Route")]
    public static void Initiate()
    {
        var toLoad = Resources.Load("RM") as GameObject;
        if (toLoad != null)
            //Instantiate(toLoad);
            PrefabUtility.InstantiatePrefab(toLoad);
        else
            Debug.LogError("Unable to load via PrefabUtility.ins...?");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RouteMaster master = (RouteMaster)target;

        master.Init();
        //master.InitPrefab();

        //master.CreateNewRoute();

        //This is only debugging purposes
        if (master.AllowDebugMsg)
            CheckRouteDataInEditor(master);

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

    bool IsWithinEraseArea(Vector3 toErase, RouteMaster _master)
    {
        //Is the obj pos < x
        if (toErase.x < _master.Eraser.transform.position.x + _master.EraseArea &&
            //pos > x
            toErase.x > _master.Eraser.transform.position.x - _master.EraseArea &&
            //pos < y
            toErase.y < _master.Eraser.transform.position.y + _master.EraseArea &&
            //pos > y
            toErase.y > _master.Eraser.transform.position.y - _master.EraseArea)
            return true;
        else
            return false;
    }

    [ContextMenu("Check route data")]
    void CheckRouteDataInEditor(RouteMaster _m)
    {
        Debug.Log("You have " + _m.Routes.Count + " routes");
        Debug.Log("You have a total of " + totalControlPoints(_m) + " Handles");
    }

    int totalControlPoints(RouteMaster _m)
    {
        int index = 0;
        foreach (var route in _m.Routes)
        {
            foreach (var cp in route.controlPoints)
            {
                if (cp != null)
                    index++;
            }
        }
        return index;
    }

    protected virtual void OnSceneGUI()
    {
        RouteMaster master = (RouteMaster)target;

        if (master.isEraseMode == true)
        {

            master.Eraser.transform.position = Handles.PositionHandle(master.Eraser.transform.position, Quaternion.identity);

            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.red;
                //Handles.DotHandleCap(0, master.Eraser.transform.position, Quaternion.identity, 0.5f, EventType.Repaint);
                Handles.SphereHandleCap(0, master.Eraser.transform.position, Quaternion.identity, 1f, EventType.Repaint);

                Handles.color = new Color(1f, 0.5f, 0.5f);
                Handles.RectangleHandleCap(0, master.Eraser.transform.position, Quaternion.identity, master.EraseArea, EventType.Repaint);

                foreach (var bush in master.Bushes)
                {
                    if (bush != null)
                    {
                        if (IsWithinEraseArea(bush.transform.position, master))
                        {
                            master.Bushes.Remove(bush);
                            DestroyImmediate(bush);
                        }
                    }
                }

            }

        }


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

                //The index of the tangent handlers are 1 and 2. These determine the nature of the curve or
                //how curvy it is
                Handles.color = Color.grey;
                Vector3 curveDeterment1 = Handles.FreeMoveHandle(routes[i].controlPoints[1].position, Quaternion.identity, handleSize, snap,
                    Handles.SphereHandleCap);

                Vector3 curveDeterment2 = Handles.FreeMoveHandle(routes[i].controlPoints[2].position, Quaternion.identity, handleSize, snap,
                    Handles.SphereHandleCap);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(master, "Change Look At Target Position");

                    routes[i].controlPoints[1].position = curveDeterment1;
                    routes[i].controlPoints[2].position = curveDeterment2;

                    routes[i].controlPoints[3].position = newTargetPos;
                    if (routes[i - 1] != null)
                        routes[i - 1].controlPoints[0].position = newTargetPos;

                    //Undo.RecordObject(master, "Change Look At Target Position");
                    
                }
            }
        }
    }

}
