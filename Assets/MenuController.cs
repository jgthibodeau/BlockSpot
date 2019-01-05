using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private ShowPanels showPanels;
    public static MenuController instance = null;
    public Menu gameOverMenu;
    public TMPro.TMP_Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            showPanels = GetComponentInChildren<ShowPanels>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static string HIGH_SCORE = "HIGH_SCORE";
    public string newHighScorePrefix = "NEW HIGH SCORE";
    public string prevHighScorePrefix = "PREVIOUS HIGH SCORE";
    public string currentHighScorePrefix = "HIGH SCORE";
    public string playerScorePrefix = "YOUR SCORE";
    public void GameOver(int score)
    {
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE, 0);
        string highScoreString = Util.FormatNumber(highScore);
        string scoreString = Util.FormatNumber(score);

        string displayText = "";
        if (score > highScore)
        {
            displayText = newHighScorePrefix + "\n" + scoreString + "\n\n" + prevHighScorePrefix + "\n" + highScoreString;
            PlayerPrefs.SetInt(HIGH_SCORE, score);
        } else
        {
            displayText = currentHighScorePrefix + "\n" + highScoreString + "\n\n" + playerScorePrefix + "\n" + score;
        }
        scoreText.text = displayText;

        Debug.Log("GAME OVER");
        showPanels.Show(gameOverMenu);
    }
}
