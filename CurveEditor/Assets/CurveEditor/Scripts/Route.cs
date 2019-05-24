using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour {

    [Tooltip("Needs to contain four values or will not work!!")]
    public Transform[] controlPoints;

    Vector2 gizmosPositions;

    //[SerializeField]
    [Range(0.01f, 0.1f)]
    public float precisionValue = 0.05f;

    [HideInInspector]
    public List<Vector2> IndividualPoints = new List<Vector2>();

    void OnDrawGizmos()
    {
        IndividualPoints.Clear();
        for (float t = 0; t <= 1; t+=precisionValue)
        {
            gizmosPositions = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
                3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
                3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
                Mathf.Pow(t, 3) * controlPoints[3].position;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(gizmosPositions, 0.25f);
            IndividualPoints.Add(gizmosPositions);
        }

        Gizmos.DrawLine(new Vector2(controlPoints[0].position.x, controlPoints[0].position.y),
            new Vector2(controlPoints[1].position.x, controlPoints[1].position.y));

        Gizmos.DrawLine(new Vector2(controlPoints[2].position.x, controlPoints[2].position.y),
            new Vector2(controlPoints[3].position.x, controlPoints[3].position.y));
    }

}
