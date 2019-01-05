using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    public Slider sfxSlider, musicSlider, gyroSlider;
    public Toggle showButtonToggle, invertToggle, muteToggle;
    public TMPro.TMP_Text startingLevelText;

    public int minStartingLevel = 0, maxStartingLevel = 9;

    public static string LOADED = "LOADED";
    public static string SFX_VOL = "SFX_VOL";
    public static string MUSIC_VOL = "MUSIC_VOL";
    public static string GYRO_SENSITIVITY = "GYRO_SENSITIVITY";
    public static string SHOW_BUTTONS = "SHOW_BUTTONS";
    public static string INVERT_ROTATION = "INVERT_ROTATION";
    public static string MUTE = "MUTE";
    public static string STARTING_LEVEL = "STARTING_LEVEL";
    public static string GYRO_CALIBRATION = "GYRO_CALIBRATION";

    public static string SETTINGS_CHANGED = "SETTINGS_CHANGED";
    
    public AudioMixer mainMixer;

    void Start()
    {
        if (PlayerPrefs.HasKey(LOADED))
        {
            LoadSettings();
        } else
        {
            LoadDefaultSettings();
        }
    }

    void LoadSettings()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOL);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOL);
        gyroSlider.value = PlayerPrefs.GetFloat(GYRO_SENSITIVITY);
        showButtonToggle.isOn = PlayerPrefs.GetInt(SHOW_BUTTONS) == 1;
        invertToggle.isOn = PlayerPrefs.GetInt(INVERT_ROTATION) == 1;
        muteToggle.isOn = PlayerPrefs.GetInt(MUTE) == 1;
        startingLevelText.text = "" + (PlayerPrefs.GetInt(STARTING_LEVEL) + 1);
        SetMute(muteToggle.isOn);
    }

    void LoadDefaultSettings()
    {
        SetSfxLevel(sfxSlider.value);
        SetMusicLevel(musicSlider.value);
        SetGyroSensitivity(gyroSlider.value);
        SetShowButtons(showButtonToggle.isOn);
        SetInvertRotation(invertToggle.isOn);
        SetMute(muteToggle.isOn);
        SetStartingLevel(0);

        PlayerPrefs.SetInt(LOADED, 1);
        SaveSettings();
    }

    public static void SaveSettings() {
        PlayerPrefs.Save();
        EventManager.TriggerEvent(SETTINGS_CHANGED);
    }

    public void SetSfxLevel(float sfxVol)
    {
        if (!muteToggle.isOn)
        {
            mainMixer.SetFloat("sfxVol", sfxVol);
        }
        PlayerPrefs.SetFloat(SFX_VOL, sfxVol);
        SaveSettings();
    }

    public void SetMusicLevel(float musicVol)
    {
        if (!muteToggle.isOn)
        {
            mainMixer.SetFloat("musicVol", musicVol);
        }
        PlayerPrefs.SetFloat(MUSIC_VOL, musicVol);
        SaveSettings();
    }
    
    public void SetGyroSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(GYRO_SENSITIVITY, sensitivity);
        SaveSettings();
    }

    public void SetShowButtons(bool showButtons)
    {
        int val = showButtons ? 1 : 0;
        PlayerPrefs.SetInt(SHOW_BUTTONS, val);
        SaveSettings();
    }

    public void SetInvertRotation(bool invertRotation)
    {
        int val = invertRotation ? 1 : 0;
        PlayerPrefs.SetInt(INVERT_ROTATION, val);
        SaveSettings();
    }

    public void SetMute(bool mute)
    {
        int val = mute ? 1 : 0;
        PlayerPrefs.SetInt(MUTE, val);
        SaveSettings();

        if (mute)
        {
            mainMixer.SetFloat("sfxVol", -100);
            mainMixer.SetFloat("musicVol", -100);
        }
        else
        {
            mainMixer.SetFloat("sfxVol", PlayerPrefs.GetFloat(SFX_VOL));
            mainMixer.SetFloat("musicVol", PlayerPrefs.GetFloat(MUSIC_VOL));
        }
    }

    public void SetStartingLevel(int level)
    {
        level = Mathf.Clamp(level, minStartingLevel, maxStartingLevel);
        startingLevelText.text = "" + (level + 1);
        PlayerPrefs.SetInt(STARTING_LEVEL, level);
        SaveSettings();
    }

    public void IncreaseStartingLevel()
    {
        int level = PlayerPrefs.GetInt(STARTING_LEVEL, 0) + 1;
        SetStartingLevel(level);
    }

    public void DecreaseStartingLevel()
    {
        int level = PlayerPrefs.GetInt(STARTING_LEVEL, 0) - 1;
        SetStartingLevel(level);
    }
}
