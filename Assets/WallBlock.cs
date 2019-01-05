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
    private Renderer[] defaultBlockRenderers;
    public Material explodedMaterial;

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
        defaultBlockRenderers = defaultBlocks.GetComponentsInChildren<Renderer>();
    }

    public void Explode(float z)
    {
        foreach (Renderer renderer in defaultBlockRenderers)
        {
            renderer.material = explodedMaterial;
        }

        Vector3 position = transform.position;
        position.z -= 0.001f;
        transform.position = position;
        StartCoroutine(DoExplosion(z));
    }

    private float explodeSpeed = 5;
    private float explodeRotateSpeed = 200;
    private float explodeScaleSpeed = 3f;
    private IEnumerator DoExplosion(float z)
    {
        MeshRenderer[] children = transform.GetComponentsInChildren<MeshRenderer>();
        
        foreach (MeshRenderer r in children)
        {
            Transform t = r.transform;
            if (Mathf.Approximately(t.localPosition.x, 0) && Mathf.Approximately(t.localPosition.y, 0))
            {
                r.enabled = false;
            }
        }
        
        while (true)
        {
            Vector3 rotation = transform.eulerAngles;
            rotation.z += explodeRotateSpeed * Time.deltaTime;
            transform.eulerAngles = rotation;

            foreach (MeshRenderer r in children)
            {
                Transform t = r.transform;

                Vector3 newPosition = t.position;
                newPosition += (t.position - transform.position) * explodeSpeed * Time.deltaTime;
                newPosition.z = z;
                t.position = newPosition;

                t.eulerAngles = rotation;

                float newScale = t.localScale.x;
                newScale -= explodeScaleSpeed * Time.deltaTime;
                if (newScale < 0)
                {
                    newScale = 0;
                }
                t.localScale = new Vector3(newScale, newScale, newScale);
            }

            yield return null;
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
        Explode(go.transform.position.z);

        if (!wall.alreadyHit)
        {
            wall.Hit();
            Player p = go.GetComponentInParent<Player>();
            p.HitGoal();
        }
    }

    public void Fail(GameObject go, MovingWall wall)
    {
        Explode(go.transform.position.z);

        if (!wall.alreadyHit)
        {
            wall.Hit();
            Player p = go.GetComponentInParent<Player>();
            p.HitWall();
        }
    }
}
