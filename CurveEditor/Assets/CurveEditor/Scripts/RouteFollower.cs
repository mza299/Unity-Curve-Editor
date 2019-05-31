using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteFollower : MonoBehaviour {

    public RouteMaster routeMaster;

    List<Route> Routes = new List<Route>();

    List<Vector2> EverySinglePoint = new List<Vector2>();

    [SerializeField]
    [Header("The higher the value, the slower it is")]
    float speedModifier = 1;

    [SerializeField]
    [Tooltip("Delay before starting movement")]
    float Delay = 0;

    bool coroutineAllowed = true;

    bool delayHappened = false;

    bool reachedTheEnd = false;

    [SerializeField]
    [Tooltip("Do you want the AI to return back in a ping-pong fashion")]
    bool pingPong = false;

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
