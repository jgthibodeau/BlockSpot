using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private TokenController tokenController;
    private ShowPanels showPanels;
    private StartOptions startOptions;
    public static MenuController instance = null;
    public Menu gameOverMenu;

    public GameObject newGameNoAdButton;
    public GameObject newGameWithAdButton;

    public Menu startMenu;
    public TMPro.TMP_Text scoreText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            showPanels = GetComponentInChildren<ShowPanels>();
            startOptions = GetComponentInChildren<StartOptions>();
            tokenController = TokenController.instance;
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
            displayText = string.Format(newHighScorePrefix, currentScoreString);
            //displayText += prevHighScorePrefix + " " + highScoreString + "\n\n";
            displayText += string.Format(accuracyPrefix, accuracyString);
            PlayerPrefs.SetInt(HIGH_SCORE, p.score);

            //newGameNoAdButton.SetActive(true);
            //newGameWithAdButton.SetActive(false);

            tokenController.AddToken();
        } else
        {
            displayText = string.Format(currentHighScorePrefix, highScoreString);
            displayText += string.Format(playerScorePrefix, currentScoreString);
            displayText += string.Format(accuracyPrefix, accuracyString);

            //newGameNoAdButton.SetActive(false);
            //newGameWithAdButton.SetActive(true);
        }
        scoreText.text = displayText;

        Debug.Log("GAME OVER");
        showPanels.Show(gameOverMenu);
    }
    
    //public void Restart()
    //{
    //    //afterAd = showPanels.Show(startMenu);

    //    //if (playedOnce)
    //    //{
    //        adController.ShowRewardedAd(startMenu);
    //    //} else
    //    //{
    //    //    playedOnce = true;
    //    //    afterAd();
    //    //}
    //}
}
