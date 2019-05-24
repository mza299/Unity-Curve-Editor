using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteMaster : MonoBehaviour {

    public Stack<Route> Routes = new Stack<Route>();

    [SerializeField]
    GameObject RouteNode;

    [Range(0.01f, 0.15f)]
    [Tooltip("The smaller the value the more precise it'll be")]
    public float precisionValue = 0.05f;

    [SerializeField]
    [Range(1f, 100f)]
    [Tooltip("The probability that an object will spawn at this position")]
    float possibility = 100f;

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
        foreach (var route in Routes)
        {
            foreach (var pos in route.IndividualPoints)
            {
                float rnd = Random.Range(0, 100);
                if (rnd > possibility)
                {
                    // Instantiate at pos
                }
            }
        }
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
}
