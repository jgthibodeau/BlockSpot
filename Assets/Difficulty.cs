using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
    private int currentLevel = 0;

    [System.Serializable]
    public class DifficultyValue
    {
        [SerializeField]
        private float initialValue;
        [SerializeField]
        private float increasePerLevel;
        [SerializeField]
        private float maxValue;

        public DifficultyValue(float initialValue, float increasePerLevel, float maxValue)
        {
            this.initialValue = initialValue;
            this.increasePerLevel = increasePerLevel;
            this.maxValue = maxValue;
        }

        public float GetCurrentValue(int level)
        {
            float value = initialValue + increasePerLevel * level;
            return Mathf.Min(value, maxValue);
        }
    }

    public DifficultyValue speed = new DifficultyValue(100, 25, 500);
    public DifficultyValue chanceForRandomTiles = new DifficultyValue(0, 0.05f, 1f);
    public DifficultyValue maxRandomTiles = new DifficultyValue(0, 1, 8);
    public DifficultyValue pointsPerWall = new DifficultyValue(5, 5, Mathf.Infinity);
    public DifficultyValue wallsTillNextLevel = new DifficultyValue(10, 2, Mathf.Infinity);

    public float Get(DifficultyValue difficultyValue)
    {
        return difficultyValue.GetCurrentValue(currentLevel);
    }

    public int GetAsInt(DifficultyValue difficultyValue)
    {
        return Mathf.FloorToInt(difficultyValue.GetCurrentValue(currentLevel));
    }

    public void IncreaseDifficulty()
    {
        currentLevel = Mathf.Max(currentLevel + 1, 0);
    }

    public void DecreaseDifficulty()
    {
        currentLevel = Mathf.Max(currentLevel - 1, 0);
    }

    public void SetDifficulty(int level)
    {
        currentLevel = Mathf.Max(level, 0);
    }

    public int CurrentLevel()
    {
        return currentLevel;
    }
}
