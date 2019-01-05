using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static MenuController menuController;
    private MyGameManager myGameManager;
    private Player player;

    public static LevelManager instance = null;
    public int currentScore;
    public Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            currentScore = 0;
            menuController = MenuController.instance;
            myGameManager = MyGameManager.instance;
            player = myGameManager.GetPlayer();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateScoreText();
    }

    public void AddScore(int score)
    {
        currentScore += score;
    }

    void UpdateScoreText()
    {
        //scoreText.text = string.Format("{0:n0}", currentScore);
    }

    private bool playerDead = false;
    public void OnPlayerDeath()
    {
        if (!playerDead)
        {
            playerDead = true;
            menuController.GameOver(player.score);
        }
    }

    public bool IsPlayerDead()
    {
        return playerDead;
    }
}
