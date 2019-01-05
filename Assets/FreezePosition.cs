using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePosition : MonoBehaviour
{
    public bool freezeWorldPosition;
    public bool freezeX, freezeY, freezeZ;

    private Vector3 initialWorldPosition;
    private Vector3 initialLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialWorldPosition = transform.position;
        initialLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeWorldPosition)
        {
            Vector3 worldPosition = transform.position;
            if (freezeX)
            {
                worldPosition.x = initialWorldPosition.x;
            }
            if (freezeY)
            {
                worldPosition.y = initialWorldPosition.y;
            }
            if (freezeZ)
            {
                worldPosition.z = initialWorldPosition.z;
            }
            transform.position = worldPosition;
        } else
        {
            Vector3 localPosition = transform.localPosition;
            if (freezeX)
            {
                localPosition.x = initialLocalPosition.x;
            }
            if (freezeY)
            {
                localPosition.y = initialLocalPosition.y;
            }
            if (freezeZ)
            {
                localPosition.z = initialLocalPosition.z;
            }
            transform.localPosition = localPosition;
        }
    }
}
