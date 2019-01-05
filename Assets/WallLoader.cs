using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WallLoader : MonoBehaviour
{
    //TODO make script to auto-generate walls with missing blocks

    public string wallDirectory = "WallFormats/";
    
    public List<WallBlock.WallType> validWallTypes;

    private Dictionary<WallBlock.WallType, List<MovingWall>> wallDictionary = new Dictionary<WallBlock.WallType, List<MovingWall>>();
    
    public GameObject blockPrefab;
    public GameObject wallBlockPrefab;

    public const char blockChar = 'x';
    public const char wallChar = 'w';

    public int rowCount = 33;
    public int columnCount = 33;
    public int blockWidth = 1;
    public int blockHeight = 1;

    public Transform startingPoint;
    public WallController wallController;

    private float minX, maxY;
    
    void Start()
    {
        LoadWalls();
        List<MovingWall> initialWalls;
        wallDictionary.TryGetValue(WallBlock.WallType.L, out initialWalls);
        //wallController.SetAvailableWalls(initialWalls);
    }

    void LoadWalls()
    {
        BetterStreamingAssets.Initialize();

        minX = (-columnCount * blockWidth) / 2;
        maxY = (columnCount * blockHeight) / 2;

        foreach (WallBlock.WallType wallType in validWallTypes)
        {
            List<MovingWall> walls = new List<MovingWall>();

            string typeName = System.Enum.GetName(typeof(WallBlock.WallType), wallType);
            string dir = wallDirectory + wallType;
            foreach (string path in BetterStreamingAssets.GetFiles(dir, "*.txt"))
            {
                walls.Add(CreateWall(BetterStreamingAssets.ReadAllLines(path)));
            }

            wallDictionary.Add(wallType, walls);
        }
    }

    float CalcX(int column)
    {
        return minX + column * blockWidth;
    }

    float CalcY(int row)
    {
        return maxY - row * blockHeight;
    }

    MovingWall CreateWall(StreamReader reader)
    {
        GameObject wall = new GameObject("wall");
        wall.layer = gameObject.layer;
        wall.transform.parent = transform;
        wall.transform.position = transform.position;

        int rowIndex = 0;
        while (!reader.EndOfStream)
        {
            GameObject row = CreateRow(reader.ReadLine());
            row.transform.position = new Vector3(0, CalcY(rowIndex), 0);
            row.transform.parent = wall.transform;
            rowIndex++;
        }

        MovingWall movingWall = wall.AddComponent<MovingWall>();
        movingWall.minZ = transform.position.z;
        movingWall.initialPosition = startingPoint;
        movingWall.destroyOnEnd = false;
        return movingWall;
    }

    MovingWall CreateWall(string[] lines)
    {
        //TODO keep track on how many cubes are seen before we have a blank
        //use this information to create a series of multi-row box colliders instead of 1 collider for each cube

        //first pass: just do this for each row individually
        //need to test with colliders disabled to see if this is really the bottleneck

        GameObject wall = new GameObject("wall");
        wall.layer = gameObject.layer;
        wall.transform.parent = transform;
        wall.transform.localPosition = Vector3.zero;

        int rowIndex = 0;
        foreach(string line in lines)
        {
            GameObject row = CreateRow(line);
            row.transform.parent = wall.transform;
            row.transform.localPosition = new Vector3(0, CalcY(rowIndex), 0);
            rowIndex++;
        }

        MovingWall movingWall = wall.AddComponent<MovingWall>();
        movingWall.minZ = transform.position.z;
        movingWall.initialPosition = startingPoint;
        return movingWall;
    }

    GameObject CreateRow(string rowText)
    {
        GameObject row = new GameObject("row");
        row.layer = gameObject.layer;

        bool hasStart = false;
        int colliderStart = 0;
        int colliderEnd = 0;

        int columnIndex = 0;
        foreach(char c in rowText)
        {
            GameObject instancedObject = null;
            switch(c)
            {
                case blockChar:
                    instancedObject = CreateBlock(blockPrefab, row.transform, columnIndex);
                    break;
                case wallChar:
                    instancedObject = CreateBlock(wallBlockPrefab, row.transform, columnIndex);
                    break;
                default:
                    break;
            }
            if (instancedObject != null)
            {
                if (!hasStart)
                {
                    hasStart = true;
                    colliderStart = columnIndex;
                }
                colliderEnd = columnIndex;
            } else
            {
                if (hasStart)
                {
                    hasStart = false;
                    CreateCollider(row, colliderStart, colliderEnd);
                }
            }
            columnIndex++;
        }
        if (hasStart)
        {
            hasStart = false;
            CreateCollider(row, colliderStart, colliderEnd);
        }
        return row;
    }

    GameObject CreateBlock(GameObject prefab, Transform parent, int columnIndex)
    {
        GameObject go = GameObject.Instantiate(prefab, parent);
        go.layer = gameObject.layer;
        go.transform.localPosition = new Vector3(CalcX(columnIndex), 0, 0);
        return go;
    }

    void CreateCollider(GameObject go, int start, int end)
    {
        BoxCollider bc = go.AddComponent<BoxCollider>();
        float x = CalcX(start) / 2f + CalcX(end) / 2f;
        bc.center = new Vector3(x, 0);
        bc.size = new Vector3((end - start) * blockWidth, blockHeight, blockWidth);
    }
}
