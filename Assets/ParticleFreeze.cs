using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFreeze : MonoBehaviour {
    public bool pause;
    // Use this for initialization
    void Start () {
        if (pause)
        {
            GetComponent<ParticleSystem>().Pause();
        }
    }
}
