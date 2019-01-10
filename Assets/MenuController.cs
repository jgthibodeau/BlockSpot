using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private AdController adController;
    private ShowPanels showPanels;
    private StartOptions startOptions;
    public static MenuController instance = null;
    public Menu gameOverMenu;
    public Menu startMenu;
    public TMPro.TMP_Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            showPanels = GetComponentInChildren<ShowPanels>();
            startOptions = GetComponentInChildren<StartOptions>();
            adController = AdController.instance;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static string HIGH_SCORE = "HIGH_SCORE";
    [TextArea]
    public string newHighScorePrefix = "NEW HIGH SCORE";
    [TextArea]
    public string prevHighScorePrefix = "PREVIOUS HIGH SCORE";
    [TextArea]
    public string currentHighScorePrefix = "HIGH SCORE";
    [TextArea]
    public string playerScorePrefix = "YOUR SCORE";
    [TextArea]
    public string accuracyPrefix = "ACCURACY";
    public void GameOver(Player p)
    {
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE, 0);
        string highScoreString = Util.FormatNumber(highScore);
        string currentScoreString = Util.FormatNumber(p.score);

        string accuracyString = Util.FormatPercentage(p.accuracy) + "%";

        string displayText = "";
        if (p.score > highScore)
        {
            displayText = newHighScorePrefix + " " + currentScoreString + "\n\n";
            //displayText += prevHighScorePrefix + " " + highScoreString + "\n\n";
            displayText += accuracyPrefix + " " + accuracyString;
            PlayerPrefs.SetInt(HIGH_SCORE, p.score);
        } else
        {
            displayText = currentHighScorePrefix + " " + highScoreString + "\n\n";
            displayText += playerScorePrefix + " " + currentScoreString + "\n\n";
            displayText += accuracyPrefix + " " + accuracyString;
        }
        scoreText.text = displayText;

        Debug.Log("GAME OVER");
        showPanels.Show(gameOverMenu);
    }

    //bool playedOnce = false;
    public AdController.AfterAdDelegate afterAd; // This is the variable holding the method you're going to call.
    public void Restart()
    {
        //afterAd = showPanels.Show(startMenu);

        //if (playedOnce)
        //{
            adController.ShowRewardedAd(startMenu);
        //} else
        //{
        //    playedOnce = true;
        //    afterAd();
        //}
    }
}
