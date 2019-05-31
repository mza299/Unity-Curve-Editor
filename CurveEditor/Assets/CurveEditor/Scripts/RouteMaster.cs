using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class RouteMaster : MonoBehaviour {

    public Stack<Route> Routes = new Stack<Route>();

    [Header("Curve Parameters")]
    [SerializeField]
    GameObject RouteNode;

    [Range(0.01f, 0.15f)]
    [Tooltip("The smaller the value the more precise it'll be")]
    public float precisionValue = 0.05f;

    [SerializeField]
    [Range(1f, 100f)]
    [Tooltip("The probability that an object will spawn at this position")]
    float possibility = 100f;

    [SerializeField]
    [Tooltip("The object that will spawn at every individual position on the curve")]
    GameObject ToSpawn;

    [Tooltip("An empty GO that represents the Eraser")]
    public GameObject Eraser;

    [Range(0.1f, 100f)]
    [Tooltip("The area of which to erase")]
    public float EraseArea = 1f;

    [HideInInspector]
    public bool isEraseMode = false;

    public const string BUSH = "_bush";

    [HideInInspector]
    public List<GameObject> Bushes = new List<GameObject>();

    [Header("AI Movement Parameters")]
    [SerializeField]
    [Tooltip("Are you going to use this curve for AI movement")]
    bool curveUsedForMovement = false;

    [SerializeField]
    List<GameObject> RouteFollowers = new List<GameObject>();

    Dictionary<GameObject, Vector2> Followers = new Dictionary<GameObject, Vector2>();

    List<Transform> RoutesAsAList = new List<Transform>();

    [SerializeField]
    [Tooltip("Speed at which AI moves")]
    float speedModifier = 1;

    int routeIndex = 1;

    float tParam;

    bool coroutineAllowed = true;

    bool initialInstantiation = false;

    public Vector2 EndPoint()
    {
        if (Routes != null && Routes.Count > 0)
            return Routes.Peek().controlPoints[3].position;
        else
        {
           // Debug.LogError("Something wrong with the routes");
            return transform.position;
        }
    }

    public void CreateNewRoute()
    {
        if (RouteNode != null)
        {
            GameObject temp = Instantiate(RouteNode, EndPoint(), Quaternion.identity, transform);
            Routes.Push(temp.GetComponent<Route>());
        }
    }

    public void DeleteNewestRoute()
    {
        if (Routes.Count > 0)
        {
            DestroyImmediate(Routes.Pop().gameObject);
        }
    }

    public void SpawnObject()
    {
        if (ToSpawn != null)
        {
            foreach (var route in Routes)
            {
                foreach (var pos in route.IndividualPoints)
                {
                    float rnd = Random.Range(0, 100);
                    if (rnd < possibility)
                    {
                        // Instantiate at pos
                        var _bush = Instantiate(ToSpawn, pos, randRot());
                        _bush.name = "_bush";
                        Bushes.Add(_bush);
                    }
                }
            }
        }
        else
            Debug.LogError("ERROR: Haven't specified ToSpawn!");
    }

    Quaternion randRot()
    {
        return Quaternion.Euler(0, 0, Random.Range(0, 360));
    }

    public void Init()
    {
        Routes.Clear();
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<Route>() != null)
                    Routes.Push(transform.GetChild(i).GetComponent<Route>());
            }
        }
    }

    #region Code executed in "RUN" mode

    void Start()
    {
        routeIndex = 1;
        tParam = 0;
        if (curveUsedForMovement)
        {
            PopulateRoutesAsAList();
            coroutineAllowed = true;
            //StartCoroutine(SpawnFollowers());
            //StartCoroutine(FollowRoute(routeIndex));
            
        }
        Debug.Log(Routes.Count);
    }

    void Update()
    {
        if (coroutineAllowed && curveUsedForMovement)
            StartCoroutine(FollowRoute(routeIndex));
    }

    void PopulateRoutesAsAList()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var r = transform.GetChild(i).transform;
            if (r.GetComponent<Route>() != null)
                RoutesAsAList.Add(r);
        }
    }

    public List<Route> lRoutes ()
    {
        List<Route> temp = new List<Route>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var r = transform.GetChild(i).transform;
            if (r.GetComponent<Route>() != null)
                temp.Add(r.GetComponent<Route>());
        }
        return temp;
    }

    IEnumerator SpawnFollowers()
    {
        if (curveUsedForMovement && initialInstantiation == false)
        {
            for (int i = 0; i < RouteFollowers.Count; i++)
            {
                yield return new WaitForSeconds(0.5f * i);
                Followers.Add(Instantiate(RouteFollowers[i]), Vector3.zero);
            }
        }
    }

    IEnumerator FollowRoute(int routeNumber)
    {
        coroutineAllowed = false;

        Vector2 p0 = RoutesAsAList[routeNumber].GetChild(0).position;
        Vector2 p1 = RoutesAsAList[routeNumber].GetChild(1).position;
        Vector2 p2 = RoutesAsAList[routeNumber].GetChild(2).position;
        Vector2 p3 = RoutesAsAList[routeNumber].GetChild(3).position;

        foreach (var f in RouteFollowers)
        {
            if (f != null)
            {
                while (tParam < 1)
                {
                    tParam += Time.deltaTime * speedModifier;

                    var pos = Mathf.Pow(1 - tParam, 3) * p0 +
                        3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                        3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                        Mathf.Pow(tParam, 3) * p3;

                    f.transform.position = pos;
                    yield return new WaitForEndOfFrame();
                } 
            }
        }
        tParam = 0f;
        routeIndex += 1;

        if (routeIndex > RouteFollowers.Count - 1)
            routeIndex = 0;

        coroutineAllowed = true;
    }
    #endregion
}
