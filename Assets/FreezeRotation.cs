using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour {
    public bool x;
    public bool y;
    public bool z;

    public Vector3 frozenRotation;
    public bool useStartingRotation;

    // Use this for initialization
    void Start () {
        if (useStartingRotation)
        {
            frozenRotation = transform.eulerAngles;
        }
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 current = transform.eulerAngles;
        if (x)
        {
            current.x = frozenRotation.x;
        }
        if (y)
        {
            current.y = frozenRotation.y;
        }
        if (z)
        {
            current.z = frozenRotation.z;
        }

        transform.eulerAngles = current;
	}
}
