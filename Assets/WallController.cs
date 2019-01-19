using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(WallGenerator))]
public class WallController : MonoBehaviour
{
    public int currentWallsHit = 0;

    public Difficulty difficulty;

    public float wallCollisionSpeed = 200;
    public float speedupScale;
    public int speedupScoreScale = 2;
    public bool boosting;
    public float initialWallSpawnTime;

    public GameObject scoreObject;

    public GameObject levelUpObject;
    public TMPro.TMP_Text levelUpText;
    public AnimationCurve levelUpSizeCurve;
    public float levelUpTextDelay = 3, levelUpDelay = 1;
    public float fontScale = 80;

    public Transform startingPoint;

    public int maxCombo = 10;
    public int currentCombo = 0;
    public float multiplierPerCombo = 0.2f;//1;
    public int maxMultiplier = 10;

    private MovingWall spawnedWall;

    private WallGenerator wallGenerator;
    private MyGameManager myGameManager;
    private Player player;

    public bool createWallOnStart = true;

    public bool emitEvents = false;

    public static string HIT_WALL_FAIL = "HIT_WALL_FAIL";
    public static string HIT_WALL_SUCCESS = "HIT_WALL_SUCCESS";

    void Start()
    {
        myGameManager = MyGameManager.instance;
        wallGenerator = GetComponent<WallGenerator>();
        difficulty = GetComponent<Difficulty>();
        player = myGameManager.GetPlayer();
        difficulty.SetDifficulty(PlayerPrefs.GetInt(Options.STARTING_LEVEL, 0));
        currentCombo = 0;

        if (createWallOnStart)
        {
            StartCoroutine(InitialWall());
        }
    }

    public void HitWall(bool success, float hitAccuracy, Player player)
    {
        int previousDifficulty = difficulty.CurrentLevel();
        if (success)
        {
            HandleSuccessfulHit(player, hitAccuracy);
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
            levelUpObject.transform.localScale = new Vector3(percent, percent, 1);

            time += Time.deltaTime;
            yield return null;
        } while (time <= levelUpTextDelay);


        yield return new WaitForSeconds(levelUpDelay);
        CreateWall();
    }

    public int GetCurrentMultiplier()
    {
        //if (currentCombo == 0)
        //{
        //    return 1;
        //}
        int multiplier = 1 + Mathf.FloorToInt(currentCombo * multiplierPerCombo);
        multiplier = Mathf.Clamp(multiplier, 1, maxMultiplier);
        return multiplier;
        //if (multiplier >= 1 && multiplier < maxMultiplier)
        //{
        //    return multiplier;
        //} else if (multiplier >= 1)
        //{
        //    return maxMultiplier;
        //} else
        //{
        //    return 1;
        //}
    }

    private void HandleSuccessfulHit(Player player, float hitAccuracy)
    {
        if (emitEvents)
        {
            Debug.Log("emitting " + HIT_WALL_SUCCESS);
            EventManager.TriggerEvent(HIT_WALL_SUCCESS);
        }

        int newScore = difficulty.GetAsInt(difficulty.pointsPerWall) ;
        //multiply by combo
        if (currentCombo > 0)
        {
            newScore *= GetCurrentMultiplier();
        }
        //multiply if boosting
        if (boosting)
        {
            newScore *= speedupScoreScale;
        }
        //multiply by accuracy
        newScore = (int)(newScore * hitAccuracy);
        player.score += newScore;

        //TODO show scoreObject

        if (maxCombo < 0 || currentCombo < maxCombo)
        {
            currentCombo++;
            if (currentCombo < 0)
            {
                currentCombo = int.MaxValue;
            }
        }

        currentWallsHit++;
        if (currentWallsHit >= difficulty.Get(difficulty.wallsTillNextLevel))
        {
            currentWallsHit = 0;
            difficulty.IncreaseDifficulty();
            //player.GainMistake();
            player.RefillMistakes();
        }
    }

    private void HandleFailedHit()
    {
        if (emitEvents)
        {
            Debug.Log("emitting " + HIT_WALL_FAIL);
            EventManager.TriggerEvent(HIT_WALL_FAIL);
        }
        currentCombo = 0;
    }

    public void IncreaseCurrentSpeed()
    {
        if (spawnedWall != null)
        {
            boosting = true;
            spawnedWall.speed = difficulty.Get(difficulty.speed) * speedupScale;
            //spawnedWall.speed *= speedupScale;
        }
    }

    public void DecreaseCurrentSpeed()
    {
        boosting = false;
        if (spawnedWall != null)
        {
            spawnedWall.speed = difficulty.GetAsInt(difficulty.speed);
        }
    }

    public IEnumerator InitialWall()
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

        spawnedWall = wallGenerator.CreateWall(player, 1, randomWallCount);
        spawnedWall.initialPosition = startingPoint;
        spawnedWall.minZ = transform.position.z;
        spawnedWall.player = player;
        spawnedWall.score = difficulty.GetAsInt(difficulty.pointsPerWall);
        spawnedWall.wallController = this;
        spawnedWall.speed = difficulty.Get(difficulty.speed);
        spawnedWall.wallCollisionSpeed = wallCollisionSpeed;
        spawnedWall.Activate();
    }
}
