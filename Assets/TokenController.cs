using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenController : MonoBehaviour
{
    //public static TokenController instance = null;

    //public static string LAST_FREE_TOKEN_TIME = "LAST_FREE_TOKEN_TIME";
    //public static string TOKEN_COUNT = "TOKEN_COUNT";
    //public int initialTokenCount = 3;
    //public int minutesPerToken;
    //public int maxTokens = 3;
    //public int maxFreeTokens = 3;
    //public int tokensPerAd = 3;
    //public int currentTokens;

    //private System.DateTime currentTime;
    //private System.DateTime lastFreeTokenTime;
    //private System.TimeSpan timeBetweenLastTokenAndNow;
    //private System.TimeSpan timeUntilNextFreeToken;

    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else if (instance != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //void Start()
    //{
    //    UpdateTokens();
    //}

    //public System.TimeSpan TimeUntilNextFreeToken()
    //{
    //    return timeUntilNextFreeToken;
    //}

    //public void UpdateTokens()
    //{
    //    currentTokens = PlayerPrefs.GetInt(TOKEN_COUNT, initialTokenCount);

    //    UpdateTimeUntilNextToken();

    //    if (currentTokens < maxFreeTokens)
    //    {
    //        GetFreeTokens();
    //    }
    //    else
    //    {
    //        EmptyFreeTokenTime();
    //    }
    //}

    //void EmptyFreeTokenTime()
    //{
    //    timeUntilNextFreeToken = new System.TimeSpan(0, 0, 0);
    //    lastFreeTokenTime = currentTime;
    //    SaveTokenTime();
    //}

    //void UpdateTimeUntilNextToken()
    //{
    //    currentTime = System.DateTime.Now;
    //    GetTimeSinceLastFreeToken(currentTime);

    //    timeBetweenLastTokenAndNow = currentTime.Subtract(lastFreeTokenTime);

    //    timeUntilNextFreeToken = new System.TimeSpan(0, minutesPerToken - timeBetweenLastTokenAndNow.Minutes - 1, 60 - timeBetweenLastTokenAndNow.Seconds);
    //}

    //void SaveTokenTime()
    //{
    //    PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_YEAR", lastFreeTokenTime.Year);
    //    PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_MONTH", lastFreeTokenTime.Month);
    //    PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_DAY", lastFreeTokenTime.Day);

    //    PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_HOUR", lastFreeTokenTime.Hour);
    //    PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_MINUTE", lastFreeTokenTime.Minute);
    //    PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_SECOND", lastFreeTokenTime.Second);

    //    PlayerPrefs.Save();
    //}

    //void GetTimeSinceLastFreeToken(System.DateTime currentTime) {
    //    lastFreeTokenTime = new System.DateTime(
    //        PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_YEAR", currentTime.Year),
    //        PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_MONTH", currentTime.Month),
    //        PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_DAY", currentTime.Day),

    //        PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_HOUR", currentTime.Hour),
    //        PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_MINUTE", currentTime.Minute),
    //        PlayerPrefs.GetInt(LAST_FREE_TOKEN_TIME + "_SECOND", currentTime.Second)
    //        );
    //}

    //private void GetFreeTokens()
    //{
    //    if (timeBetweenLastTokenAndNow.TotalMinutes > minutesPerToken)
    //    {
    //        int numberOfTokens = (int)timeBetweenLastTokenAndNow.TotalMinutes / minutesPerToken;
    //        AddTokens(numberOfTokens);

    //        if (currentTokens < maxFreeTokens)
    //        {
    //            //store lastFreeTokenTime + added tokens * 1 minute as lastFreeTokenTime
    //            lastFreeTokenTime = lastFreeTokenTime.AddMinutes(numberOfTokens * minutesPerToken);
    //            timeBetweenLastTokenAndNow = timeBetweenLastTokenAndNow.Subtract(new System.TimeSpan(0, numberOfTokens * minutesPerToken, 0));
    //        } else
    //        {
    //            lastFreeTokenTime = currentTime;
    //        }

    //        SaveTokenTime();
    //    }
    //}

    //public bool HasTokens()
    //{
    //    return currentTokens > 0;
    //}

    //public void UseToken()
    //{
    //    if (currentTokens > 0)
    //    {
    //        currentTokens--;
    //    }
    //    SaveTokens();
    //}

    //public void AddToken()
    //{
    //    AddTokens(1);
    //}

    //public void AddTokens(int tokens)
    //{
    //    if (currentTokens < maxTokens)
    //    {
    //        currentTokens++;
    //    }
    //    SaveTokens();
    //}

    //public void AddAdTokens()
    //{
    //    currentTokens = tokensPerAd;
    //    SaveTokens();
    //}

    //void SaveTokens()
    //{
    //    PlayerPrefs.SetInt(TOKEN_COUNT, currentTokens);
    //    PlayerPrefs.Save();
    //}
    public bool infiniteTokens = false;

    public static TokenController instance = null;

    public static string LAST_FREE_TOKEN_TIME = "LAST_FREE_TOKEN_TIME";
    public static string TOKEN_COUNT = "TOKEN_COUNT";
    public int initialTokenCount = 3;
    public float minutesPerFreeToken = 5;
    public int maxTokens = 10;
    public int freeTokens = 3;
    public int tokensPerAd = 3;
    private int currentTokens;

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
        
        if (currentTokens == 0)
        {
            currentTime = System.DateTime.Now;
            lastFreeTokenTime = GetTimeSinceLastFreeToken(currentTime);

            timeBetweenLastTokenAndNow = currentTime.Subtract(lastFreeTokenTime);

            float timeElapsedInMinutes = (float)timeBetweenLastTokenAndNow.TotalMinutes;
            int remainingMinutes = Mathf.FloorToInt(Mathf.Clamp(minutesPerFreeToken - timeElapsedInMinutes, 0, minutesPerFreeToken));
            int remainingSeconds = Mathf.FloorToInt(Mathf.Clamp(((minutesPerFreeToken - timeElapsedInMinutes) % 1) * 60, 0, 60));
            timeUntilNextFreeToken = new System.TimeSpan(0, remainingMinutes, remainingSeconds);

            GetFreeTokens();
        }
        else
        {
            timeUntilNextFreeToken = new System.TimeSpan(0, 0, 0);
        }
    }

    void UpdateTimeUntilNextToken()
    {
    }

    void SaveTokenTime(System.DateTime time)
    {
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_YEAR", time.Year);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_MONTH", time.Month);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_DAY", time.Day);

        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_HOUR", time.Hour);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_MINUTE", time.Minute);
        PlayerPrefs.SetInt(LAST_FREE_TOKEN_TIME + "_SECOND", time.Second);

        PlayerPrefs.Save();
    }

    System.DateTime GetTimeSinceLastFreeToken(System.DateTime currentTime)
    {
        return new System.DateTime(
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
        if (timeBetweenLastTokenAndNow.TotalMinutes > minutesPerFreeToken)
        {
            AddTokens(freeTokens);
        }
    }

    public bool HasTokens()
    {
        return infiniteTokens || currentTokens > 0;
    }

    public int CurrentTokens()
    {
        return currentTokens;
    }

    public void UseToken()
    {
        if (infiniteTokens)
        {
            return;
        }

        if (currentTokens > 0)
        {
            currentTokens--;
        }

        if (currentTokens == 0)
        {
            SaveTokenTime(System.DateTime.Now);
        }

        SaveTokens();
    }

    public void AddToken()
    {
        AddTokens(1);
    }

    public void AddAdTokens()
    {
        AddTokens(tokensPerAd);
    }

    public void AddTokens(int tokens)
    {
        if (currentTokens < maxTokens)
        {
            currentTokens = Mathf.Clamp(currentTokens + tokens, 0, maxTokens);
        }
        SaveTokens();
    }

    void SaveTokens()
    {
        PlayerPrefs.SetInt(TOKEN_COUNT, currentTokens);
        PlayerPrefs.Save();
    }
}
