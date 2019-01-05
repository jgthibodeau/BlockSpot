﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(WallGenerator))]
public class WallController : MonoBehaviour
{
    [System.Serializable]
    public class DifficultyLevel
    {
        public float moveSpeed;
        public int wallsTillNextLevel;
        public int pointsPerSuccess;
    }
    public DifficultyLevel[] difficultyLevels;
    //public int currentDifficulty;
    public int currentWallsHit = 0;

    public Difficulty difficulty;

    public float wallCollisionSpeed = 200;
    public float speedupScale;
    public int speedupScoreScale = 2;
    public bool boosting;
    public float initialWallSpawnTime;

    public GameObject levelUpObject;
    public TMPro.TMP_Text levelUpText;
    public AnimationCurve levelUpSizeCurve;
    public float levelUpTextDelay = 3, levelUpDelay = 1;
    public float fontScale = 80;

    public Transform startingPoint;

    public int maxMultiplier, currentMultiplier;

    private MovingWall spawnedWall;

    private WallGenerator wallGenerator;
    private MyGameManager myGameManager;
    private Player player;

    void Start()
    {
        myGameManager = MyGameManager.instance;
        wallGenerator = GetComponent<WallGenerator>();
        difficulty = GetComponent<Difficulty>();
        player = myGameManager.GetPlayer();
        //currentDifficulty = 0;
        difficulty.SetDifficulty(PlayerPrefs.GetInt(Options.STARTING_LEVEL, 0));
        currentMultiplier = 1;

        StartCoroutine(InitialWall());
    }

    //public DifficultyLevel GetCurrentDifficulty()
    //{
    //    return difficultyLevels[currentDifficulty];
    //}

    //public float GetCurrentWallSpeed()
    //{
    //    return GetCurrentDifficulty().moveSpeed;
    //}

    public void HitWall(bool success, Player player)
    {
        int previousDifficulty = difficulty.CurrentLevel(); //currentDifficulty;
        if (success)
        {
            HandleSuccessfulHit(player);
        } else
        {
            HandleFailedHit();
        }
        if (difficulty.CurrentLevel() > previousDifficulty)
        {
            StartCoroutine(DelayedCreateWall());
        } else
        {
            CreateWall();
        }
    }

    private IEnumerator DelayedCreateWall()
    {
        float time = 0;
        do
        {
            float percent = Util.ConvertScale(0, levelUpTextDelay, 0, 1, time);
            percent = levelUpSizeCurve.Evaluate(percent);
            //levelUpText.fontSize = fontScale * percent;
            levelUpObject.transform.localScale = new Vector3(percent, percent, 1);

            time += Time.deltaTime;
            yield return null;
        } while (time <= levelUpTextDelay);


        yield return new WaitForSeconds(levelUpDelay);
        CreateWall();
    }

    private void HandleSuccessfulHit(Player player)
    {
        int newScore = difficulty.GetAsInt(difficulty.pointsPerWall) * currentMultiplier; //GetCurrentDifficulty().pointsPerSuccess * currentMultiplier;
        if (boosting)
        {
            newScore *= speedupScoreScale;
        }
        player.score += newScore;

        if (currentMultiplier < maxMultiplier)
        {
            currentMultiplier++;
        }

        currentWallsHit++;
        if (currentWallsHit >= difficulty.Get(difficulty.wallsTillNextLevel)) //GetCurrentDifficulty().wallsTillNextLevel)
        {
            currentWallsHit = 0;
            difficulty.IncreaseDifficulty();

            //if (currentDifficulty <= difficultyLevels.Length)
            //{
            //    currentDifficulty++;
            //    currentWallsHit = 0;
            //} else
            //{
            //    currentWallsHit--;
            //}
        }
    }

    private void HandleFailedHit()
    {
        currentMultiplier = 1;
    }

    public void IncreaseCurrentSpeed()
    {
        if (spawnedWall != null)
        {
            boosting = true;
            spawnedWall.speed = difficulty.Get(difficulty.speed) * speedupScale; //GetCurrentWallSpeed() * speedupScale;
        }
    }

    public void DecreaseCurrentSpeed()
    {
        if (spawnedWall != null)
        {
            boosting = false;
            spawnedWall.speed = difficulty.GetAsInt(difficulty.speed); //GetCurrentWallSpeed();
        }
    }

    private IEnumerator InitialWall()
    {
        yield return new WaitForSeconds(initialWallSpawnTime);
        CreateWall();
    }

    private void CreateWall()
    {
        int maxRandomTiles = difficulty.GetAsInt(difficulty.maxRandomTiles);
        float randomChance = difficulty.Get(difficulty.chanceForRandomTiles);
        int randomWallCount = 0;
        for (int i=0; i < maxRandomTiles; i++)
        {
            if (Random.value <= randomChance)
            {
                randomWallCount++;
            }
        }

        Debug.Log("maxRandomTiles " + maxRandomTiles + ", randomChance " + randomChance + ", randomWallCount " + randomWallCount);

        spawnedWall = wallGenerator.CreateWall(player.currentType, 1, randomWallCount);
        spawnedWall.initialPosition = startingPoint;
        spawnedWall.minZ = transform.position.z;
        spawnedWall.player = player;
        spawnedWall.score = difficulty.GetAsInt(difficulty.pointsPerWall); //GetCurrentDifficulty().pointsPerSuccess;
        spawnedWall.wallController = this;
        spawnedWall.speed = difficulty.Get(difficulty.speed); //GetCurrentWallSpeed();
        spawnedWall.wallCollisionSpeed = wallCollisionSpeed;
        spawnedWall.Activate();
    }
}
