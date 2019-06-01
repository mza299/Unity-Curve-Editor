using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RouteFollower : MonoBehaviour {

    public RouteMaster routeMaster;

    List<Route> Routes = new List<Route>();

    List<Vector2> EverySinglePoint = new List<Vector2>();

    [Header("The higher the value, the slower it is")]
    public float speedModifier = 1;

    [Tooltip("Delay before starting movement")]
    public float Delay = 0;

    bool coroutineAllowed = true;

    bool delayHappened = false;

    bool reachedTheEnd = false;

    [SerializeField]
    [Tooltip("Do you want the AI to return back in a ping-pong fashion")]
    bool pingPong = false;

    public enum AITYPE { NONE, TEST};

    public AITYPE aITYPE;

    Dictionary<AITYPE, GameObject> AI = new Dictionary<AITYPE, GameObject>();

    public void ManageAIDictionaryForEditor()
    {
        AI.Clear();
        AI.Add(AITYPE.NONE, Resources.Load("Prefabs/AI") as GameObject);
        AI.Add(AITYPE.TEST, Resources.Load("Prefabs/AI") as GameObject);
    }

    public void ReplaceAIInEditor()
    {
        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            GameObject instance = PrefabUtility.InstantiatePrefab(AI[aITYPE]) as GameObject;
            instance.transform.position = transform.position;
            instance.transform.parent = transform;
        }
        else
            Debug.LogWarning(gameObject.name + " does not have any childs attached to it");
    }

    private void Start()
    {
        if (routeMaster != null)
        {
            Routes = routeMaster.lRoutes();
        }

        foreach (var route in Routes)
        {
            EverySinglePoint.AddRange(route.IndividualPoints);
        }
        //routeIndex = 0;
        //tParam = 0;
    }

    private void Update()
    {
        if (coroutineAllowed)
        {
            if (pingPong == false)
                StartCoroutine(Follow());
            else
                StartCoroutine(PingPongFollow());
        }
    }

    IEnumerator PingPongFollow()
    {
        coroutineAllowed = false;
        var miniInterval = speedModifier / EverySinglePoint.Count;
        if (reachedTheEnd == false)
        {
            for (int i = 0; i < EverySinglePoint.Count; i++)
            {
                var timer = 0f;
                if (delayHappened == true)
                {
                    while (timer < miniInterval)
                    {
                        timer += Time.deltaTime;
                        if (i < EverySinglePoint.Count - 1)
                        {
                            //if (i == EverySinglePoint.Count -1)
                            //{
                            //    reachedTheEnd = true;
                            //    //break;
                            //}
                            transform.position = Vector2.MoveTowards(EverySinglePoint[i], EverySinglePoint[i + 1], timer / miniInterval);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                else
                {
                    delayHappened = true;
                    yield return new WaitForSeconds(Delay);
                }
                coroutineAllowed = true;
                reachedTheEnd = !reachedTheEnd;
            }
        }
        else
        {
            for (int i = EverySinglePoint.Count-1; i > 0; i--)
            {
                var timer = 0f;
                if (delayHappened == true)
                {
                    while (timer < miniInterval)
                    {
                        timer += Time.deltaTime;
                        if (i < 1)
                        {
                            //if (i == 0)
                            //{
                            //    reachedTheEnd = false;
                            //    //break;
                            //}
                            transform.position = Vector2.MoveTowards(EverySinglePoint[i], EverySinglePoint[i - 1], timer / miniInterval);
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
                else
                {
                    delayHappened = true;
                    yield return new WaitForSeconds(Delay);
                }
                coroutineAllowed = true;
                reachedTheEnd = !reachedTheEnd;
            }
        }
    }

    IEnumerator Follow()
    {
        coroutineAllowed = false;
        var miniInterval = speedModifier / EverySinglePoint.Count;
        for (int i = 0; i < EverySinglePoint.Count; i++)
        {
            var timer = 0f;
            if (delayHappened == true)
            {
                while (timer < miniInterval)
                {
                    timer += Time.deltaTime;
                    if (i < EverySinglePoint.Count - 1)
                    {
                        transform.position = Vector2.MoveTowards(EverySinglePoint[i], EverySinglePoint[i + 1], timer / miniInterval);
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            else
            {
                delayHappened = true;
                yield return new WaitForSeconds(Delay);
            }
        }

        coroutineAllowed = true;
    }
}
