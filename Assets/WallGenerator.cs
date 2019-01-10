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

    public MovingWall CreateWall(Player player, int requiredCount, int randomCount)
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
        GameObject wall = GameObject.Instantiate(baseMovingWall, transform);
        MovingWall movingWall = wall.GetComponent<MovingWall>();

        //generate list of wall pieces
        WallBlock[] wallBlocks = GenerateWallBlocks(wall.transform, player, requiredCount, randomCount);
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
            }
        }
    }

    private WallBlock[] GenerateWallBlocks(Transform wall, Player player, int requiredCount, int randomCount)
    {
        WallBlock[] wallBlocks = new WallBlock[wallPieceCount];
        int pieceIndex = 0;
        for (; pieceIndex < requiredCount; pieceIndex++)
        {
            wallBlocks[pieceIndex] = CreateWallBlock(wall, player.currentType, GetRandomRotation(player));
        }
        for (; pieceIndex < randomCount; pieceIndex++)
        {
            wallBlocks[pieceIndex] = CreateRandomWallBlock(wall, player.currentType, GetRandomRotation());
        }
        for (; pieceIndex < wallPieceCount; pieceIndex++)
        {
            wallBlocks[pieceIndex] = GameObject.Instantiate(blankWallComponent, wall).GetComponent<WallBlock>();
        }

        return Util.Randomize<WallBlock>(wallBlocks);
    }

    private float GetRandomRotation()
    {
        return 90 * Random.Range(0, 4);
    }

    private float GetRandomRotation(Player player)
    {
        float rotation = player.blockController.GetAdjustedDesiredAngle();
        rotation += 90 * Random.Range(-1, 2);
        if (rotation > 360)
        {
            rotation -= 360;
        }
        if (rotation < 0)
        {
            rotation += 360;
        }
        return rotation;
    }

    private WallBlock CreateWallBlock(Transform wall, WallBlock.WallType wallType, float rotation)
    {
        GameObject go;
        wallComponentDictionary.TryGetValue(wallType, out go);
        GameObject wallBlock = GameObject.Instantiate(go, wall);
        wallBlock.transform.eulerAngles = new Vector3(0, 0, rotation);
        return wallBlock.GetComponent<WallBlock>(); 
    }

    private WallBlock CreateRandomWallBlock(Transform wall, WallBlock.WallType ignoreWallType, float rotation)
    {
        WallBlock.WallType wallType;
        do
        {
            wallType = Util.RandomEnumValue<WallBlock.WallType>(WallBlock.WallType.NONE);
        } while (wallType == ignoreWallType);
        
        return CreateWallBlock(wall, wallType, rotation);
    }
}
