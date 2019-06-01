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

        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        centeredStyle.fontSize = 15;
        centeredStyle.fontStyle = FontStyle.Bold;
        centeredStyle.richText = true;

        GUILayout.Space(10);
        GUILayout.Label("<color=#a94064>Route Management</color>", centeredStyle);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("New Route"))
        {
            Debug.Log("Button Pressed");
            master.CreateNewRoute();
        }

        if (GUILayout.Button("Delete Newest Route"))
        {
            master.DeleteNewestRoute();
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Create Flat Route"))
        {
            CreateFlatPath(master);
        }

        GUILayout.Space(25);

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

        GUILayout.Space(10);
        GUILayout.Label("<color=#a94064>Eraser</color>", centeredStyle);

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

        GUILayout.Space(10);

        GUILayout.Label("<color=#a94064>Closing the Gap</color>", centeredStyle);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Loop @ Beginning"))
        {
            CloseGap(master, true);
        }

        if (GUILayout.Button("Loop @ End"))
        {
            CloseGap(master, false);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("<color=#a94064>AI Management</color>", centeredStyle);
        GUILayout.Label("<size=9><color=#a94064>The button will overwrite the number of AIs you have in the scene</color></size>", centeredStyle);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Instantiate AI"))
        {
            Spawn(master);
        }
        
        GUILayout.EndHorizontal();

    }

    void Spawn(RouteMaster master)
    {
        if (Resources.Load("Prefabs/_testAI") != null)
        {
            for (int i = 0; i < master.SpawnedAIs.Count; i++)
            {
                if (master.SpawnedAIs[i] != null)
                {
                    DestroyImmediate(master.SpawnedAIs[i]);
                }
            }

            master.SpawnedAIs.Clear();
           
            for (int i = 0; i < master.NoofAIs; i++)
            {
                var ai = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/_testAI")) as GameObject;
                ai.transform.position = master.transform.position + (Vector3.up * i);
                master.SpawnedAIs.Add(ai);

                //Get the relevant component from ai
                var routeFollower = ai.GetComponent<RouteFollower>();
                if (routeFollower != null)
                {
                    routeFollower.routeMaster = master;
                    routeFollower.speedModifier = master.speedModifier;
                    routeFollower.Delay = i* master.gapBetweenAgents;
                    switch (master._aITYPE)
                    {
                        case RouteMaster._AITYPE.NONE:
                            if (Resources.Load("Prefabs/AI") != null)
                            {
                                var type = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/AI")) as GameObject;
                                type.transform.position = master.transform.position + (Vector3.up * i);
                                type.transform.parent = ai.transform;
                            }
                            else
                                Debug.LogError("Error of Resources.Load possibly the path");
                            break;
                        case RouteMaster._AITYPE.TEST:
                            if (Resources.Load("Prefabs/AI") != null)
                            {
                                var type = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/AI")) as GameObject;
                                type.transform.position = master.transform.position + (Vector3.up * i);
                                type.transform.parent = ai.transform;
                            }
                            else
                                Debug.LogError("Error of Resources.Load possibly the path");
                            break;
                        default:
                            break;
                    }
                }
                else
                    Debug.LogError("No routefollower script attached to instance???");
            }
            
        }
    }

    void CreateFlatPath(RouteMaster m)
    {
        var flat = m.NewRoute();
        if (flat != null)
        {
            m.Routes.Peek().controlPoints[1].position = m.Routes.Peek().controlPoints[0].position + (Vector3.right * 10);
            m.Routes.Peek().controlPoints[2].position = m.Routes.Peek().controlPoints[3].position + (Vector3.left * 10);
        }
    }

    void CloseGap(RouteMaster m, bool atTheBegining)
    {
        if (m != null)
        {
            if (m.Routes.Count > 1)
            {
                if (atTheBegining == false)
                {
                    Vector2 lastpoint = m.Routes.Peek().IndividualPoints[m.Routes.Peek().IndividualPoints.Count-1];
                    m.lRoutes()[0].controlPoints[0].position = lastpoint;
                }
                else
                { 
                    Vector2 firstPoint = m.lRoutes()[0].controlPoints[0].position;
                    m.Routes.Peek().controlPoints[3].position = firstPoint;
                    
                }
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
