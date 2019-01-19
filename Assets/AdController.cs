using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;

public class AdController : MonoBehaviour
{
    public static AdController instance = null;
    private ShowPanels showPanels;
    private TokenController tokenController;

    private Menu afterAdMenu;

    public int gamesPerAd = 5;
    public int gamesSinceLastAd;
    public static string GAMES_PLAYED_SINCE_LAST_AD = "GAMES_PLAYED_SINCE_LAST_AD";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            tokenController = TokenController.instance;
            showPanels = GetComponentInChildren<ShowPanels>();
            gamesSinceLastAd = PlayerPrefs.GetInt(GAMES_PLAYED_SINCE_LAST_AD, 0);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddGameSinceLastAd()
    {
        gamesSinceLastAd++;
        PlayerPrefs.SetInt(GAMES_PLAYED_SINCE_LAST_AD, gamesSinceLastAd);
    }

    public void ResetGamesSinceLastAd()
    {
        gamesSinceLastAd = 0;
        PlayerPrefs.SetInt(GAMES_PLAYED_SINCE_LAST_AD, gamesSinceLastAd);
    }

    public bool EnoughGamesToShowAd()
    {
        return gamesSinceLastAd >= gamesPerAd;
    }

    public void ShowRewardedAdBack()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    public void ShowRewardedAdNewMenu(Menu afterAdMenu)
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            this.afterAdMenu = afterAdMenu;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                OnSuccess();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                OnSkip();
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                OnFail();
                break;
        }
    }

    private void OnSuccess()
    {
        ResetGamesSinceLastAd();
        tokenController.AddAdTokens();
        NextMenu();
    }

    private void OnSkip()
    {
        showPanels.Back();
    }

    private void OnFail()
    {
        ResetGamesSinceLastAd();
        tokenController.AddAdTokens();
        NextMenu();
    }

    private void NextMenu()
    {
        if (afterAdMenu == null)
        {
            showPanels.Back();
        } else
        {
            showPanels.Show(afterAdMenu);
        }
    }
}
