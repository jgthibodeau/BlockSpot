using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomProjector : MonoBehaviour
{
    public int maxDistance = 1000;
    public Vector3 offset = new Vector3(0, 0, -0.001f);
    public LayerMask layers;
    public GameObject shadowObject;
    
    void Update()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, layers))
        {
            Vector3 position = hitInfo.point;
            position += transform.right * offset.x;
            position += transform.up * offset.y;
            position += transform.forward * offset.z;
            shadowObject.transform.position = position;
        }
    }
}
