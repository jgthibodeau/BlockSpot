﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public GameObject baseGraphics;
    public WallController wallController;

    public float speed;
    public float minZ;
    public int score;
    public Transform initialPosition;
    public bool active = false;
    public bool destroyOnEnd = true;

    public Player player;
    public float wallCollisionSpeed;

    public bool alreadyHit;
    private bool aboutToHit = false;

    public WallBlock[] wallBlocks;

    private MyGameManager myGameManager;

    void Start()
    {
        myGameManager = MyGameManager.instance;
    }

    void FixedUpdate()
    {
        if (myGameManager.isPaused)
        {
            return;
        }

        if (active)
        {
            if (aboutToHit)
            {
                speed = wallCollisionSpeed;
            }

            Vector3 newPosition = transform.position + Vector3.back * speed * Time.fixedDeltaTime;

            if (!aboutToHit && newPosition.z < player.transform.position.z + 1f)
            {
                aboutToHit = true;
                newPosition.z = player.transform.position.z + 1f;
                transform.position = newPosition;

                TriggerColliders();
                baseGraphics.SetActive(false);
            } else
            {
                transform.position = newPosition;
            }

            if (transform.position.z < minZ)
            {
                Deactivate();
            }
        }
    }

    void TriggerColliders()
    {
        foreach(WallBlock block in wallBlocks)
        {
            block.TriggerColliders(this, player);
        }
        bool success = alreadyHit;
        if (!success)
        {
            Hit();
            player.HitWall();
        }

        foreach (WallBlock block in wallBlocks)
        {
            block.Explode(success);
        }
    }

    public void Activate()
    {
        active = true;
        alreadyHit = false;
        transform.position = initialPosition.position;
    }

    public void Deactivate()
    {
        if (destroyOnEnd)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            this.active = false;
            transform.position = new Vector3(0, 0, -1000);
        }
    }

    //public void Explode()
    //{
    //    WallBlock[] wallBlocks = gameObject.GetComponentsInChildren<WallBlock>();

    //    foreach(WallBlock wallBlock in wallBlocks)
    //    {
    //        wallBlock.Explode();
    //    }
    //}

    public void Hit()
    {
        alreadyHit = true;
    }
}
