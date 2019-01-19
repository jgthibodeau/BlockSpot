using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private AdController adController;
    private TokenController tokenController;
    private ShowPanels showPanels;
    private StartOptions startOptions;
    public static MenuController instance = null;
    public Menu gameOverMenu;

    public GameObject newGameNoAdButton;
    public GameObject newGameWithAdButton;

    public Menu startMenu;
    public TMPro.TMP_Text scoreText;

    public static string SHOWN_TUTORIAL = "SHOWN_TUTORIAL";
    public Menu calibrationMenu;
    public GameObject normalNewGame, tutorialNewGame;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            showPanels = GetComponentInChildren<ShowPanels>();
            startOptions = GetComponentInChildren<StartOptions>();
            tokenController = TokenController.instance;
            adController = AdController.instance;

            UpdateTutorialButtons();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ShowCalibrationMenu()
    {
        showPanels.Show(calibrationMenu);
    }

    public void HideCalibrationMenu()
    {
        showPanels.Close();
    }

    public void ResetTutorial()
    {
        PlayerPrefs.SetInt(SHOWN_TUTORIAL, 0);
        UpdateTutorialButtons();
    }

    public void UpdateTutorialButtons()
    {
        if (PlayerPrefs.GetInt(SHOWN_TUTORIAL, 0) == 1)
        {
            normalNewGame.SetActive(true);
            tutorialNewGame.SetActive(false);
        }
        else
        {
            normalNewGame.SetActive(false);
            tutorialNewGame.SetActive(true);
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
            //displayText = string.Format(newHighScorePrefix, currentScoreString);
            displayText += string.Format(newHighScorePrefix, accuracyString);
            //displayText += prevHighScorePrefix + " " + highScoreString + "\n\n";
            displayText += string.Format(accuracyPrefix, accuracyString);
            PlayerPrefs.SetInt(HIGH_SCORE, p.score);


            tokenController.AddToken();
        } else
        {
            displayText = string.Format(currentHighScorePrefix, highScoreString);
            displayText += string.Format(playerScorePrefix, currentScoreString);
            displayText += string.Format(accuracyPrefix, accuracyString);

            newGameNoAdButton.SetActive(false);
            newGameWithAdButton.SetActive(true);
        }

        adController.AddGameSinceLastAd();

        scoreText.text = displayText;

        Debug.Log("GAME OVER");
        showPanels.Show(gameOverMenu);
    }

    public bool showAd = false;
    void Update()
    {
        if (showPanels.current == gameOverMenu)
        {
            if (!showAd && adController.EnoughGamesToShowAd())
            {
                Debug.Log("showing ad button");
                showAd = true;
            }
            else if (showAd && !adController.EnoughGamesToShowAd())
            {
                Debug.Log("hiding ad button");
                showAd = false;
            }
            newGameNoAdButton.SetActive(!showAd);
            newGameWithAdButton.SetActive(showAd);
        }
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
