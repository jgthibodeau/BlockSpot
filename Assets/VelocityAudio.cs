using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VelocityAudio : MonoBehaviour {
    private Rigidbody2D rb;

    public AudioSource audioSource;
    
    public float minVelocity, maxVelocity;
    public float minPitch, maxPitch;
    public float minVolume, maxVolume;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
	// Update is called once per frame
	void Update () {
        float velocity = rb.velocity.magnitude;

        audioSource.volume = Util.ConvertScale(minVelocity, maxVelocity, minVolume, maxVolume, velocity);
        audioSource.pitch = Util.ConvertScale(minVelocity, maxVelocity, minPitch, maxPitch, velocity);
    }
}
