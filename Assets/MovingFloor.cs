using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFloor : MonoBehaviour
{
    public float speed;
    public Transform[] tiles;
    public float tileSize = 3000;

    public Transform lastTile;
    public float tileDistance;

    void Start()
    {
        tileDistance = tileSize * 10;
        lastTile = tiles[tiles.Length - 1];
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform t in tiles)
        {
            Vector3 position = t.position;
            position.z -= speed * Time.deltaTime;
            t.position = position;
        }

        if (lastTile.position.z < -tileDistance)
        {
            Vector3 position = lastTile.transform.position;
            position.z += tiles.Length * tileDistance;
            lastTile.position = position;

            for (int i = tiles.Length - 1; i > 0; i--)
            {
                tiles[i] = tiles[i - 1];
            }
            tiles[0] = lastTile;
            lastTile = tiles[tiles.Length - 1];
        }
    }
}
