using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject baseMovingWall;

    public GameObject blankWallComponent;
    public List<GameObject> wallComponents;
    public int wallSize;
    public float blockSize;

    private Dictionary<WallBlock.WallType, GameObject> wallComponentDictionary = new Dictionary<WallBlock.WallType, GameObject>();
    private int wallPieceCount;
    private float minPosition;

    void Start()
    {
        wallPieceCount = wallSize * wallSize;

        minPosition = (1 - wallSize) * blockSize * 0.5f;

        foreach (GameObject go in wallComponents)
        {
            wallComponentDictionary.Add(go.GetComponent<WallBlock>().wallType, go);
        }
    }

    private float CalcPosition(int column)
    {
        return minPosition + column * blockSize;
    }

    public MovingWall CreateWall(WallBlock.WallType requiredType, int requiredCount, int randomCount)
    {
        //keep values bounded
        if (requiredCount + randomCount >= wallPieceCount)
        {
            if (requiredCount >= wallPieceCount)
            {
                requiredCount = wallPieceCount - 1;
                randomCount = requiredCount;
            } else
            {
                randomCount = wallPieceCount - 1;
            }
        } else
        {
            randomCount += requiredCount;
        }

        //create parent wall object
        //GameObject wall = new GameObject("wall");
        //wall.transform.parent = transform;
        //MovingWall movingWall = wall.AddComponent<MovingWall>();
        GameObject wall = GameObject.Instantiate(baseMovingWall, transform);
        MovingWall movingWall = wall.GetComponent<MovingWall>();

        //generate list of wall pieces
        WallBlock[] wallBlocks = GenerateWallBlocks(wall.transform, requiredType, requiredCount, randomCount);
        movingWall.wallBlocks = wallBlocks;

        //place and rotate each wall piece
        PlaceWallBlocks(wallBlocks);

        return movingWall;
    }

    private void PlaceWallBlocks(WallBlock[] wallBlocks)
    {
        for (int i = 0; i < wallSize; i++)
        {
            for (int j = 0; j < wallSize; j++)
            {
                GameObject wallBlock = wallBlocks[i * wallSize + j].gameObject;

                wallBlock.transform.localPosition = new Vector3(CalcPosition(i), CalcPosition(j), 0);

                int z = Random.Range(0, 4);
                z *= 90;
                wallBlock.transform.eulerAngles = new Vector3(0, 0, z);
            }
        }
    }

    private WallBlock[] GenerateWallBlocks(Transform wall, WallBlock.WallType requiredType, int requiredCount, int randomCount)
    {
        WallBlock[] wallBlocks = new WallBlock[wallPieceCount];
        int pieceIndex = 0;
        for (; pieceIndex < requiredCount; pieceIndex++)
        {
            wallBlocks[pieceIndex] = CreateWallBlock(wall, requiredType);
        }
        for (; pieceIndex < randomCount; pieceIndex++)
        {
            wallBlocks[pieceIndex] = CreateRandomWallBlock(wall, requiredType);
        }
        for (; pieceIndex < wallPieceCount; pieceIndex++)
        {
            wallBlocks[pieceIndex] = GameObject.Instantiate(blankWallComponent, wall).GetComponent<WallBlock>();
        }

        return Util.Randomize<WallBlock>(wallBlocks);
    }

    private WallBlock CreateWallBlock(Transform wall, WallBlock.WallType wallType)
    {
        GameObject go;
        wallComponentDictionary.TryGetValue(wallType, out go);
        return GameObject.Instantiate(go, wall).GetComponent<WallBlock>();
    }

    private WallBlock CreateRandomWallBlock(Transform wall, WallBlock.WallType ignoreWallType)
    {
        WallBlock.WallType wallType;
        do
        {
            wallType = Util.RandomEnumValue<WallBlock.WallType>(WallBlock.WallType.NONE);
        } while (wallType == ignoreWallType);
        
        return CreateWallBlock(wall, wallType);
    }
}
