using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour
{
    public static AdController instance = null;
    private ShowPanels showPanels;

    public delegate void AfterAdDelegate();
    private AfterAdDelegate afterAdDelegate;

    public static string TOKEN_COUNT = "TOKEN_COUNT";
    public int initialTokenCount = 3;
    public int tokensPerMinute;

    private Menu afterAdMenu;

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

    public void ShowRewardedAd(Menu afterAdMenu)
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
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
        showPanels.Show(afterAdMenu);
    }
}
