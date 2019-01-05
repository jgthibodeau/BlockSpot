using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineBetweenObjects : MonoBehaviour {
    LineRenderer lineRenderer;
    Vector3[] points = new Vector3[2];

    public GameObject go1, go2;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    
	// Update is called once per frame
	void Update () {
		if (go1 != null && go2 != null)
        {
            points[0] = go1.transform.position;
            points[1] = go2.transform.position;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(points);
            lineRenderer.positionCount = 2;
        } else
        {
            lineRenderer.positionCount = 0;
        }
	}
}
