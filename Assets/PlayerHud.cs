using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PlayerHud : MonoBehaviour
{
    public string scorePrefix = "";
    public TMPro.TMP_Text scoreText;

    public string multiplierPrefix = "x";
    public TMPro.TMP_Text multiplierText;

    public string levelPrefix = "Level ";
    public TMPro.TMP_Text levelText;
    public Image levelImg;

    public Image mistakeMask;
    public Image[] mistakeImages;
    public Gradient mistakeGradient;

    public GameObject[] toggleableButtons;

    private MyGameManager myGameManager;
    private Player player;
    private WallController wallController;

    private int currentScore;
    private int currentCombo;
    private int currentLevel;
    private int currentMistakes;
    private int currentWallsHit;

    void Start()
    {
        myGameManager = MyGameManager.instance;
        player = myGameManager.GetPlayer();
        wallController = player.wallController;

        UpdateScore();
        UpdateCombo();
        UpdateLevel();
        UpdateLevelProgress();
        UpdateMistakes();
        UpdateHiddenButtons();
    }

    void Update()
    {
        if (player.score != currentScore)
        {
            UpdateScore();
        }
        if (wallController.currentCombo != currentCombo)
        {
            UpdateCombo();
        }
        if (currentLevel != wallController.difficulty.CurrentLevel())
        {
            UpdateLevel();
        }
        if (currentWallsHit != wallController.currentWallsHit)
        {
            UpdateLevelProgress();
        }
        if (currentMistakes != player.remainingMistakes)
        {
            UpdateMistakes();
        }
    }

    void UpdateScore()
    {
        currentScore = player.score;
        scoreText.SetText(scorePrefix + Util.FormatNumber(currentScore));
    }

    void UpdateCombo()
    {
        currentCombo = wallController.currentCombo;
        multiplierText.SetText(multiplierPrefix + Util.FormatNumber(currentCombo));
    }

    void UpdateLevel()
    {
        currentLevel = wallController.difficulty.CurrentLevel();
        levelText.SetText(levelPrefix + (currentLevel + 1));
    }

    void UpdateLevelProgress()
    {
        currentWallsHit = wallController.currentWallsHit;
        levelImg.fillAmount = Util.ConvertScale(0, wallController.difficulty.Get(wallController.difficulty.wallsTillNextLevel), 0f, 1f, currentWallsHit);
    }

    void UpdateMistakes()
    {
        currentMistakes = player.remainingMistakes;
        float mistakeFillAmount = Util.ConvertScale(0, player.maxMistakes, 0f, 1f, player.remainingMistakes);
        mistakeMask.fillAmount = mistakeFillAmount;
        Color mistakeColor = mistakeGradient.Evaluate(mistakeFillAmount);
        foreach (Image img in mistakeImages)
        {
            img.color = mistakeColor;
        }
    }


    private UnityAction settingsListener;
    void OnEnable()
    {
        settingsListener = new UnityAction(UpdateHiddenButtons);
        EventManager.StartListening(Options.SETTINGS_CHANGED, settingsListener);
    }
    void OnDisable()
    {
        EventManager.StopListening(Options.SETTINGS_CHANGED, settingsListener);
    }
    void UpdateHiddenButtons()
    {
        int btn = PlayerPrefs.GetInt(Options.SHOW_BUTTONS);
        bool showButtons = btn == 1;
        Debug.Log("Checking for hidden buttons " + btn + " " + showButtons);
        foreach (GameObject button in toggleableButtons)
        {
            button.SetActive(showButtons);
        }
    }
}
