using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : MonoBehaviour
{
    public static TokenController instance = null;

    public static string LAST_FREE_TOKEN_TIME = "LAST_FREE_TOKEN_TIME";
    public static string TOKEN_COUNT = "TOKEN_COUNT";
    public int initialTokenCount = 3;
    public int minutesPerToken;
    public int maxTokens = 3;
    public int tokensPerAd = 3;
    public int currentTokens;

    private System.DateTime currentTime;
    private System.DateTime lastFreeTokenTime;
    private System.TimeSpan timeBetweenLastTokenAndNow;
    private System.TimeSpan timeUntilNextFreeToken;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateTokens();
    }

    public System.TimeSpan TimeUntilNextFreeToken()
    {
        return timeUntilNextFreeToken;
    }

    public void UpdateTokens()
    {
        currentTokens = PlayerPrefs.GetInt(TOKEN_COUNT, initialTokenCount);

        UpdateTimeUntilNextToken();

        if (currentTokens < maxTokens)
        {
            GetFreeTokens();
        }
        else
        {
            EmptyFreeTokenTime();
        }
    }

    void EmptyFreeTokenTime()
    {
        timeUntilNextFreeToken = new System.TimeSpan(0, 0, 0);
        lastFreeTokenTime = currentTime;
        SaveTokenTime();
    }

    void UpdateTimeUntilNextToken()
    {
        currentTime = System.DateTime.Now;
        GetTimeSinceLastFreeToken(currentTime);

        timeBetweenLastTokenAndNow = currentTime.Subtract(lastFreeTokenTime);

        timeUntilNextFreeToken = new System.TimeSpan(0, minutesPerToken - timeBetweenLastTokenAndNow.Minutes - 1, 60 - timeBetweenLastTokenAndNow.Seconds);
    }

    void SaveTokenTime()
    {
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_YEAR", lastFreeTokenTime.Year);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_MONTH", lastFreeTokenTime.Month);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_DAY", lastFreeTokenTime.Day);

        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_HOUR", lastFreeTokenTime.Hour);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_MINUTE", lastFreeTokenTime.Minute);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_SECOND", lastFreeTokenTime.Second);

        PlayerPrefs.Save();
    }

    void GetTimeSinceLastFreeToken(System.DateTime currentTime) {
        lastFreeTokenTime = new System.DateTime(
            PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_YEAR", currentTime.Year),
            PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_MONTH", currentTime.Month),
            PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_DAY", currentTime.Day),

            PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_HOUR", currentTime.Hour),
            PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_MINUTE", currentTime.Minute),
            PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_SECOND", currentTime.Second)
            );
    }

    private void GetFreeTokens()
    {
        if (timeBetweenLastTokenAndNow.TotalMinutes > minutesPerToken)
        {
            int numberOfTokens = (int)timeBetweenLastTokenAndNow.TotalMinutes / minutesPerToken;
            currentTokens = Mathf.Clamp(currentTokens + numberOfTokens, 0, maxTokens);
            SaveTokens();

            //if not at max tokens
            if (currentTokens < maxTokens)
            {
                //store lastFreeTokenTime + added tokens * 1 minute as lastFreeTokenTime
                lastFreeTokenTime = lastFreeTokenTime.AddMinutes(numberOfTokens * minutesPerToken);
                timeBetweenLastTokenAndNow = timeBetweenLastTokenAndNow.Subtract(new System.TimeSpan(0, numberOfTokens * minutesPerToken, 0));
            } else
            {
                lastFreeTokenTime = currentTime;
            }

            SaveTokenTime();
        }
    }

    public bool HasTokens()
    {
        return currentTokens > 0;
    }

    public void UseToken()
    {
        if (currentTokens > 0)
        {
            currentTokens--;
        }
        SaveTokens();
    }

    public void AddToken()
    {
        AddToken(1);
    }

    public void AddToken(int tokens)
    {
        if (currentTokens < maxTokens)
        {
            currentTokens++;
        }
        SaveTokens();
    }

    public void AddAdTokens()
    {
        currentTokens = tokensPerAd;
        SaveTokens();
    }

    void SaveTokens()
    {
        PlayerPrefs.SetInt(TOKEN_COUNT, currentTokens);
        PlayerPrefs.Save();
    }
}
