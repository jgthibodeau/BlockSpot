using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudio : MonoBehaviour {
    public AudioSource audioSource;

    public float minVolume, maxVolume, volChangeSpeed;

    public bool enabled;

	// Use this for initialization
	void Start () {
        audioSource.volume = 0;
	}
	
	// Update is called once per frame
	void Update () {
        float desiredVol = enabled ? maxVolume : minVolume;

        audioSource.volume = Mathf.Lerp(audioSource.volume, desiredVol, volChangeSpeed * Time.deltaTime);
    }
}
