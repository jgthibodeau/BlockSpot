using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenUI : MonoBehaviour
{
    private TokenController tokenController;
    private ShowPanels showPanels;
    private StartOptions startOptions;
    public Menu tokenMenu;

    public TMPro.TMP_Text outOfTokensTextBox;
    [TextArea]
    public string outOfTokensText = "Watch an ad for 3 game tokens\n\nTime until next free token {0}:{1}";

    public GameObject tokenTextBoxHolder;
    public TMPro.TMP_Text tokenTextBox;
    [TextArea]
    public string tokenText = "TOKENS: {0}";
    int currentTokens = -1;

    public BounceText bounceText;

    void Start()
    {
        tokenController = TokenController.instance;
        showPanels = ShowPanels.instance;
        startOptions = StartOptions.instance;

        tokenController.UpdateTokens();
        currentTokens = tokenController.currentTokens;
        tokenTextBox.text = string.Format(tokenText, currentTokens);
    }

    public void NewGame(int level)
    {
        tokenController.UseToken();
        startOptions.LoadLevel(level);
    }

    public void ShowMenuIfHaveTokens(Menu menu)
    {
        if (tokenController.HasTokens())
        {
            showPanels.Show(menu);
        } else
        {
            showPanels.Show(tokenMenu);
        }
    }
    
    void Update()
    {
        bool updateTokens = false;
        if (tokenMenu.IsVisible())
        {
            updateTokens = true;

            System.TimeSpan timeUntilNextToken = tokenController.TimeUntilNextFreeToken();
            outOfTokensTextBox.text = string.Format(outOfTokensText, timeUntilNextToken.Minutes.ToString("00"), timeUntilNextToken.Seconds.ToString("00"));
        }
        
        if (showPanels.InMenu())
        {
            updateTokens = true;

            if (tokenController.currentTokens != currentTokens)
            {
                currentTokens = tokenController.currentTokens;
                tokenTextBox.text = string.Format(tokenText, currentTokens);

                bounceText.Trigger();
            }

            if (!tokenTextBoxHolder.activeSelf)
            {
                tokenTextBoxHolder.SetActive(true);
            }
        } else
        {
            if (tokenTextBoxHolder.activeSelf)
            {
                tokenTextBoxHolder.SetActive(false);
            }
        }

        if (updateTokens)
        {
            tokenController.UpdateTokens();
        }
    }
}
