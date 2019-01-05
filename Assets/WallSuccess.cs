using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSuccess : MonoBehaviour
{
    private WallBlock wallBlock;
    private Collider c;
    public LayerMask playerMask;

    void Start()
    {
        c = GetComponent<Collider>();
        wallBlock = GetComponentInParent<WallBlock>();
    }

    public bool ContainsPlayer()
    {
        return Physics.CheckBox(transform.position, c.bounds.extents, Quaternion.identity, playerMask);
    }
}
