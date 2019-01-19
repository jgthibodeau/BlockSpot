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
    public string outOfTokensText = "Watch an ad for {0} game tokens\n\nTime until next free token {1}:{2}";

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
        currentTokens = tokenController.CurrentTokens();
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
            outOfTokensTextBox.text = string.Format(outOfTokensText, tokenController.tokensPerAd, timeUntilNextToken.Minutes.ToString("00"), timeUntilNextToken.Seconds.ToString("00"));
        }
        
        if (!tokenController.infiniteTokens && showPanels.InMenu() && !showPanels.current.hideTokenBox)
        {
            updateTokens = true;

            if (tokenController.CurrentTokens() != currentTokens)
            {
                currentTokens = tokenController.CurrentTokens();
                tokenTextBox.text = string.Format(tokenText, currentTokens);

                bounceText.Trigger();
            }

            ActivateTokenBox();
        } else
        {
            DeactivateTokenBox();
        }

        if (updateTokens)
        {
            tokenController.UpdateTokens();
        }
    }

    void ActivateTokenBox()
    {
        if (!tokenTextBoxHolder.activeSelf)
        {
            tokenTextBoxHolder.SetActive(true);
        }
    }

    void DeactivateTokenBox()
    {
        if (tokenTextBoxHolder.activeSelf)
        {
            tokenTextBoxHolder.SetActive(false);
        }
    }
}
