using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour
{
    public enum TutorialStep { NONE, CALIBRATION, BASIC_MOVEMENT, HEALTH, COMBO, LEVEL_UP, BOOST, DONE }
    public TutorialStep currentStep = TutorialStep.NONE;
    public TutorialStep nextStep = TutorialStep.CALIBRATION;

    public int initialLevel = 1;

    MenuController menuController;
    ShowPanels showPanels;
    StartOptions startOptions;

    public WallController wallController;
    public Player player;

    public GameObject playerHudHolder;

    public GameObject tutorialTextHolder;
    public TMPro.TMP_Text tutorialText;

    public GameObject nextButton;

    void Start()
    {
        PlayerPrefs.SetInt(MenuController.SHOWN_TUTORIAL, 1);

        menuController = MenuController.instance;
        showPanels = ShowPanels.instance;
        startOptions = StartOptions.instance;
        NextStep();
    }

    public void SkipTutorial()
    {
        startOptions.LoadLevel(initialLevel);
    }

    bool calibrationMenuOpen = false;
    public void ToggleCalibrationMenu()
    {
        if (!calibrationMenuOpen)
        {
            Debug.Log("showing calibration menu");
            menuController.ShowCalibrationMenu();
            calibrationMenuOpen = true;
        } else
        {
            Debug.Log("hiding calibration menu");
            menuController.HideCalibrationMenu();
            calibrationMenuOpen = false;
        }
    }

    [TextArea]
    public string calibrationText = "Tilt in any direction to move\nSwipe up and down on either side of the screen to rotate";
    [TextArea]
    public string spotText = "Tilt and swipe to position the current piece in the right spot before it gets too close";
    public int requiredSpotSuccesses = 3;
    public int currentSpotSuccesses = 0;
    [TextArea]
    public string healthText = "You will lose 1 hit point every time you hit a wall\nAfter 3 the game is over";
    [TextArea]
    public string levelUpText = "After going through several walls, your health will refil and the difficulty will increase";
    [TextArea]
    public string boostText = "Double tap to speed up\nDoing this will double the points received for successfully navigating a wall";
    [TextArea]
    public string comboText = "The more walls you get in a row, the more points you will receive";
    [TextArea]
    public string doneText = "That's it!\nClick \"Skip Tutorial\" to start playing";

    public void NextStep()
    {
        currentStep = nextStep;

        switch(currentStep)
        {
            case TutorialStep.CALIBRATION:
                nextStep = TutorialStep.BASIC_MOVEMENT;

                //calibration menu until player presses next
                menuController.UpdateTutorialButtons();
                ToggleCalibrationMenu();
                tutorialText.text = calibrationText;
                break;

            case TutorialStep.BASIC_MOVEMENT:
                nextStep = TutorialStep.HEALTH;

                //Popup saying to move to get shape in a hole, send walls until player does it
                ToggleCalibrationMenu();
                nextButton.SetActive(false);
                playerHudHolder.SetActive(true);
                tutorialText.text = spotText;

                StartCoroutine(wallController.InitialWall());
                break;

            case TutorialStep.HEALTH:
                nextStep = TutorialStep.LEVEL_UP;

                //Popup explaining health points, send wall with no hole
                tutorialText.text = healthText;
                player.invincible = false;
                break;

            case TutorialStep.LEVEL_UP:
                nextStep = TutorialStep.BOOST;

                //Popup explaining level up regaining health points
                tutorialText.text = levelUpText;
                break;

            case TutorialStep.BOOST:
                nextStep = TutorialStep.COMBO;

                //Popup explaining boost, send walls until player does it
                tutorialText.text = boostText;
                break;

            case TutorialStep.COMBO:
                nextStep = TutorialStep.DONE;

                //Popup explaining scoring and combos
                tutorialText.text = comboText;
                break;

            case TutorialStep.DONE:

                //done
                tutorialText.text = doneText;
                break;
        }
    }


    private UnityAction hitWallFailListener;
    private UnityAction hitWallSuccessListener;
    void OnEnable()
    {
        hitWallFailListener = new UnityAction(HitWallFail);
        hitWallSuccessListener = new UnityAction(HitWallSuccess);
        EventManager.StartListening(WallController.HIT_WALL_FAIL, hitWallFailListener);
        EventManager.StartListening(WallController.HIT_WALL_SUCCESS, hitWallSuccessListener);
    }
    void OnDisable()
    {
        EventManager.StopListening(WallController.HIT_WALL_FAIL, hitWallFailListener);
        EventManager.StopListening(WallController.HIT_WALL_SUCCESS, hitWallSuccessListener);
    }
    void HitWallFail()
    {
        Debug.Log("handling " + WallController.HIT_WALL_FAIL);
        if (currentStep == TutorialStep.HEALTH)
        {
            if (!player.invincible)
            {
                player.invincible = true;
            }
            NextStep();
        }
    }
    void HitWallSuccess()
    {
        Debug.Log("handling " + WallController.HIT_WALL_SUCCESS);
        if (currentStep == TutorialStep.BASIC_MOVEMENT)
        {
            currentSpotSuccesses++;
            if (currentSpotSuccesses >= requiredSpotSuccesses)
            {
                NextStep();
            }
        }
    }
}
