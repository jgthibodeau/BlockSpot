using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlock : MonoBehaviour
{
    public enum WallType { L, BACK_L, ZIG, ZAG, LINE, BLOCK, ARROW, NONE }
    public WallType wallType;

    public WallSuccess[] wallSuccesses;
    public WallFailure wallFailure;

    public GameObject defaultBlocks;
    //MeshRenderer[] children;

    public List<Transform> rotatableChildren;
    public Transform rotatableGrandChildren;
    public Transform hiddenGrandChildren;

    public bool Is90DegreeRotationValid()
    {
        return wallType == WallType.BLOCK;
    }

    public bool Is180DegreeRotationValid()
    {
        return wallType == WallType.ZIG
            || wallType == WallType.ZAG
            || wallType == WallType.LINE
            || wallType == WallType.BLOCK;
    }

    void Start()
    {
        wallSuccesses = GetComponentsInChildren<WallSuccess>();
        wallFailure = GetComponentInChildren<WallFailure>();
        //children = transform.GetComponentsInChildren<MeshRenderer>();
    }

    public void Explode()
    {
        Vector3 position = transform.position;
        position.z -= 0.001f;
        transform.position = position;
        StartCoroutine(DoExplosion());
    }

    private float explodeSpeed = 5;
    private float explodeRotateSpeed = 300;
    private float explodeScaleSpeed = 1.5f;
    private float explodeLineScaleSpeed = 0.5f;
    private IEnumerator DoExplosion()
    {
        //foreach (Transform t in rotatableChildren)
        //{
        //    TryHideObject(t);
        //}
        hiddenGrandChildren.gameObject.SetActive(false);
        //yield return null;

        //int i = 0;
        //foreach (Transform t in rotatableGrandChildren.transform)
        //{
        //    if (t.parent == rotatableGrandChildren)
        //    {
        //        TryHideObject(t);
        //    }
        //    //if (i % 100 == 0)
        //    //{
        //    //    yield return null;
        //    //}
        //    //i++;
        //}
        //yield return null;

        rotatableGrandChildren.gameObject.SetActive(true);
        Vector3 rotation = transform.eulerAngles;
        while (true)
        {
            rotation.z += explodeRotateSpeed * Time.deltaTime;
            //transform.eulerAngles = rotation;

            foreach (Transform t in rotatableChildren)
            {
                RotateObject(t, rotation, true);
            }

            //i = 0;
            foreach (Transform t in rotatableGrandChildren.transform)
            {
                if (t.parent == rotatableGrandChildren)
                {
                    RotateObject(t, rotation, false);
                }
                //if (i % 100 == 0)
                //{
                //    yield return null;
                //}
                //i++;
            }

            yield return null;
        }
    }

    private void TryHideObject(Transform t)
    {
        if (Mathf.Approximately(t.localPosition.x, 0) && Mathf.Approximately(t.localPosition.y, 0))
        {
            t.gameObject.SetActive(false);
        }
    }

    private void RotateObject(Transform t, Vector3 rotation, bool isLineRenderer)
    {
        //Vector3 newPosition = t.position;
        //newPosition += (t.position - transform.position) * explodeSpeed * Time.deltaTime;
        //newPosition.z = 0;
        //t.position = newPosition;

        t.eulerAngles = rotation;

        float newScale = t.localScale.x;
        newScale -= explodeScaleSpeed * Time.deltaTime;
        if (newScale < 0)
        {
            newScale = 0;
        }
        if (isLineRenderer)
        {
            LineRenderer lineRenderer = t.GetComponentInChildren<LineRenderer>();
            if (lineRenderer != null)
            {
                float lineScale = lineRenderer.widthMultiplier - explodeLineScaleSpeed * Time.deltaTime;
                if (lineScale < 0)
                {
                    lineScale = 0;
                }
                lineRenderer.widthMultiplier = lineScale;
            } else
            {
                isLineRenderer = false;
            }
        }

        if (!isLineRenderer)
        {
            t.localScale = new Vector3(newScale, newScale, newScale);
        }
    }

    public void TriggerColliders(MovingWall wall, Player player)
    {
        if (IsSuccessfulHit(player))
        {
            Success(MyGameManager.instance.GetPlayer().gameObject, wall);
        } else if (wallFailure.ContainsPlayer())
        {
            Fail(MyGameManager.instance.GetPlayer().gameObject, wall);
        }
    }

    private bool IsSuccessfulHit(Player player)
    {
        bool allSuccess = true;
        if (wallSuccesses.Length > 0)
        {
            foreach (WallSuccess wallSuccess in wallSuccesses)
            {
                if (!wallSuccess.ContainsPlayer())
                {
                    allSuccess = false;
                    break;
                }
            }
        }
        else
        {
            allSuccess = false;
        }

        if (allSuccess)
        {
            return IsValidAngle(player);
        }

        return false;
    }

    bool IsValidAngle(Player player)
    {
        if (wallType != player.currentType)
        {
            return false;
        }

        float angleDifference = player.blockController.desiredAngle - transform.eulerAngles.z;
        if (angleDifference < 0)
        {
            angleDifference += 360;
        }
        
        return (Mathf.Approximately(angleDifference, 0))
            || (Mathf.Approximately(angleDifference, 90) && Is90DegreeRotationValid())
            || (Mathf.Approximately(angleDifference, 180) && Is180DegreeRotationValid())
            || (Mathf.Approximately(angleDifference, 270) && Is90DegreeRotationValid());
    }

    private void Success(GameObject go, MovingWall wall)
    {
        Explode();

        if (!wall.alreadyHit)
        {
            wall.Hit();
            Player p = go.GetComponentInParent<Player>();
            p.HitGoal();
        }
    }

    public void Fail(GameObject go, MovingWall wall)
    {
        Explode();

        if (!wall.alreadyHit)
        {
            wall.Hit();
            Player p = go.GetComponentInParent<Player>();
            p.HitWall();
        }
    }
}
