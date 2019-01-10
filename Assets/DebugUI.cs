using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    public GameObject debugBase;

    public TMPro.TMP_Text fpsText;
    public TMPro.TMP_Text speedText;
    public TMPro.TMP_Text scoreText;
    public TMPro.TMP_Text accuracyText;
    public TMPro.TMP_Text multiplierText;
    public TMPro.TMP_Text randomTilesText;
    public TMPro.TMP_Text randomTileChanceText;
    public TMPro.TMP_Text nextTilesText;
    public TMPro.TMP_Text levelProgressText;
    public TMPro.TMP_Text gyroCalibrationText;

    public Player player;
    public WallController wallController;
    
    const float fpsMeasurePeriod = 0.5f;
    private int fpsAccumulator = 0;
    private float fpsNextPeriod = 0;
    private int currentFps;

    public bool active = false;

    private void Start()
    {
        if (active)
        {
            Activate();
        } else
        {
            Deactivate();
        }

        fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }

        // measure average frames per second
        fpsAccumulator++;
        if (Time.realtimeSinceStartup > fpsNextPeriod)
        {
            currentFps = (int)(fpsAccumulator / fpsMeasurePeriod);
            fpsAccumulator = 0;
            fpsNextPeriod += fpsMeasurePeriod;
            fpsText.text = string.Format("{0}", currentFps);
        }

        speedText.text = string.Format("{0}", wallController.difficulty.Get(wallController.difficulty.speed));
        scoreText.text = string.Format("{0}", wallController.difficulty.Get(wallController.difficulty.pointsPerWall));
        accuracyText.text = string.Format("{0}%", player.accuracy*100);
        multiplierText.text = string.Format("{0}", wallController.GetCurrentMultiplier());
        randomTilesText.text = string.Format("{0}", wallController.difficulty.Get(wallController.difficulty.maxRandomTiles));
        randomTileChanceText.text = string.Format("{0}", wallController.difficulty.Get(wallController.difficulty.chanceForRandomTiles));
        levelProgressText.text = string.Format("{0}/{1}", wallController.currentWallsHit, wallController.difficulty.Get(wallController.difficulty.wallsTillNextLevel));

        string gyroPrecision = "#0.00000";
        gyroCalibrationText.text = string.Format("({0}, {1}, {2})", player.gyroCalibration.x.ToString(gyroPrecision), player.gyroCalibration.y.ToString(gyroPrecision), player.gyroCalibration.z.ToString(gyroPrecision));

        string[] nextTiles = new string[player.nextTypeList.Length];
        for (int i = 0; i < player.nextTypeList.Length; i++)
        {
            WallBlock.WallType type = player.nextTypeList[i];
            nextTiles[i] = System.Enum.GetName(typeof(WallBlock.WallType), type);
        }
        nextTilesText.text = string.Join(", ", nextTiles);
    }

    public void Activate()
    {
        active = true;
        debugBase.SetActive(true);
    }

    public void Deactivate()
    {
        active = false;
        debugBase.SetActive(false);
    }
}
