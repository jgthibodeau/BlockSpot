using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlock : MonoBehaviour
{
    public enum WallType { L, BACK_L, ZIG, ZAG, LINE, BLOCK, ARROW, NONE }
    public WallType wallType;

    public WallSuccess[] wallSuccesses;
    public WallFailure wallFailure;

    public List<Transform> rotatableChildren;
    public Transform hiddenGrandChildren;

    public Transform rotatableSuccessExplosion;
    public Transform rotatableFailExplosion;

    private float hitAccuracy = 0;

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
    }

    public void Explode(bool success)
    {
        Vector3 position = transform.position;
        position.z -= 0.001f;
        transform.position = position;
        StartCoroutine(DoExplosion(success));
    }

    private float explodeSpeed = 5;
    private float explodeRotateSpeed = 250;
    private float explodeScaleSpeed = 3f;
    private float explodeLineScaleSpeed = 0.5f;
    private IEnumerator DoExplosion(bool success)
    {
        hiddenGrandChildren.gameObject.SetActive(false);

        Transform rotatableGrandChildren = success ? rotatableSuccessExplosion : rotatableFailExplosion;

        rotatableGrandChildren.gameObject.SetActive(true);
        Vector3 rotation = transform.eulerAngles;
        while (true)
        {
            rotation.z += explodeRotateSpeed * Time.deltaTime;

            foreach (Transform t in rotatableChildren)
            {
                RotateObject(t, rotation, true);
            }
            
            foreach (Transform t in rotatableGrandChildren.transform)
            {
                if (t.parent == rotatableGrandChildren)
                {
                    RotateObject(t, rotation, false);
                }
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
        if (!wall.alreadyHit && IsSuccessfulHit(player))
        {
            wall.Hit();
            player.HitGoal(hitAccuracy);
        }
    }

    private bool IsSuccessfulHit(Player player)
    {
        //bool allSuccess = true;
        //if (wallSuccesses.Length > 0)
        //{
        //    foreach (WallSuccess wallSuccess in wallSuccesses)
        //    {
        //        if (!wallSuccess.ContainsPlayer())
        //        {
        //            allSuccess = false;
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    allSuccess = false;
        //}
        hitAccuracy = 0;
        if (wallSuccesses.Length > 0)
        {
            foreach (WallSuccess wallSuccess in wallSuccesses)
            {
                if (wallSuccess.ContainsPlayer())
                {
                    hitAccuracy++;
                }
            }
        }
        hitAccuracy /= wallSuccesses.Length;

        if (hitAccuracy > 0 && IsValidAngle(player)) {
            return true;
        } else
        {
            hitAccuracy = 0;
            return false;
        }
    }

    bool IsValidAngle(Player player)
    {
        Debug.Log("checking angle " + wallType + " " + player.currentType + " " + player.blockController.GetAdjustedDesiredAngle() + " " + transform.eulerAngles.z);

        if (wallType != player.currentType)
        {
            return false;
        }

        int angleDifference = (int)player.blockController.GetAdjustedDesiredAngle() - (int)transform.eulerAngles.z;
        if (angleDifference < 0)
        {
            angleDifference += 360;
        }
        
        return (Mathf.Approximately(angleDifference, 0))
            || (Mathf.Approximately(angleDifference, 90) && Is90DegreeRotationValid())
            || (Mathf.Approximately(angleDifference, 180) && Is180DegreeRotationValid())
            || (Mathf.Approximately(angleDifference, 270) && Is90DegreeRotationValid());
    }
}
