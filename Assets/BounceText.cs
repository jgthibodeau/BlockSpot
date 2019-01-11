using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class BounceText : MonoBehaviour
{
    private Coroutine bounceCoroutine;
    private TMPro.TMP_Text text;

    public float minFontSize, maxFontSize, fontChangeSpeed, freezeTime;

    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
        text.fontSize = minFontSize;
    }

    public void Trigger()
    {
        if (bounceCoroutine == null)
        {
            bounceCoroutine = StartCoroutine(BounceTokenBox());
        }
    }

    private IEnumerator BounceTokenBox()
    {
        float fontSize = minFontSize;
        while(fontSize < maxFontSize)
        {
            text.fontSize = fontSize;
            fontSize += fontChangeSpeed * Time.deltaTime;
            yield return null;
        }

        fontSize = maxFontSize;
        text.fontSize = fontSize;
        yield return new WaitForSecondsRealtime(freezeTime);

        while (fontSize > minFontSize)
        {
            text.fontSize = fontSize;
            fontSize -= fontChangeSpeed * Time.deltaTime;
            yield return null;
        }

        fontSize = minFontSize;
        text.fontSize = fontSize;

        bounceCoroutine = null;
    }
}
