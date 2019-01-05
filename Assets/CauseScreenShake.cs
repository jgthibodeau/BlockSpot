using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauseScreenShake : MonoBehaviour
{
    public float duration = 0.5f;
    public float amount = 0.5f;
    void Start () {
        ScreenShake.instance.Shake(duration, amount);
    }
}
