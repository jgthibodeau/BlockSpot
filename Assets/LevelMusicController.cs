using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicController : MonoBehaviour
{
    public AudioClip levelMusic;
    private PlayMusic playMusic;

    void Start()
    {
        playMusic = PlayMusic.instance;
        playMusic.PlayMusicClip(levelMusic);
    }
}
