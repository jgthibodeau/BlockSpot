using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    public Slider sfxSlider, musicSlider, rotateSensitivitySlider, gyroSlider;
    public Toggle showButtonToggle, invertToggle, invertGyroXToggle, invertGyroYToggle, muteToggle;
    public TMPro.TMP_Text startingLevelText;

    public int minStartingLevel = 0, maxStartingLevel = 9;

    public static string LOADED = "LOADED";
    public static string SFX_VOL = "SFX_VOL";
    public static string MUSIC_VOL = "MUSIC_VOL";
    public static string GYRO_SENSITIVITY = "GYRO_SENSITIVITY";
    public static string ROTATE_SENSITIVITY = "ROTATE_SENSITIVITY";
    public static string SHOW_BUTTONS = "SHOW_BUTTONS";
    public static string INVERT_ROTATION = "INVERT_ROTATION";
    public static string INVERT_GYRO_X = "INVERT_GYRO_X";
    public static string INVERT_GYRO_Y = "INVERT_GYRO_Y";
    public static string MUTE = "MUTE";
    public static string STARTING_LEVEL = "STARTING_LEVEL";
    public static string GYRO_CALIBRATION = "GYRO_CALIBRATION";

    public static string SETTINGS_CHANGED = "SETTINGS_CHANGED";
    
    public AudioMixer mainMixer;

    public float defaultSfxVolume = 0;
    public float defaultMusicVolume = 0;
    public float defaultGyroSensitivity = 2;
    public float defaultRotateSensitivity = 25;
    public int defaultShowButtons = 1;
    public int defaultInvertRotation = 0;
    public int defaultInvertGyroX = 0;
    public int defaultInvertGyroY = 0;
    public int defaultMute = 0;
    public int defaultStartingLevel = 0;

    private bool settingsLoaded = false;

    void Start()
    {
        //StartCoroutine(LoadSettings());
        LoadSavedSettings();
    }
    
    IEnumerator LoadSettings()
    {
        yield return new WaitForSeconds(0.5f);
        //if (PlayerPrefs.GetInt(LOADED, 0) == 1)
        //{
            LoadSavedSettings();
        //}
        //else
        //{
        //    Debug.Log("Loading default settings");
        //    LoadDefaultSettings();
        //}
        //yield return null;
        //settingsLoaded = true;
    }

    void LoadSavedSettings()
    {
        Debug.Log("Loading settings");

        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOL, defaultSfxVolume);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOL, defaultMusicVolume);
        gyroSlider.value = PlayerPrefs.GetFloat(GYRO_SENSITIVITY, defaultGyroSensitivity);
        rotateSensitivitySlider.value = PlayerPrefs.GetFloat(ROTATE_SENSITIVITY, defaultRotateSensitivity);
        showButtonToggle.isOn = PlayerPrefs.GetInt(SHOW_BUTTONS, defaultShowButtons) == 1;
        invertToggle.isOn = PlayerPrefs.GetInt(INVERT_ROTATION, defaultInvertRotation) == 1;
        invertGyroXToggle.isOn = PlayerPrefs.GetInt(INVERT_GYRO_X, defaultInvertGyroX) == 1;
        invertGyroYToggle.isOn = PlayerPrefs.GetInt(INVERT_GYRO_Y, defaultInvertGyroY) == 1;
        muteToggle.isOn = PlayerPrefs.GetInt(MUTE, defaultMute) == 1;
        SetMute(muteToggle.isOn);
        startingLevelText.text = "" + (PlayerPrefs.GetInt(STARTING_LEVEL, defaultMute) + 1);

        settingsLoaded = true;
    }

    void LoadDefaultSettings()
    {
        SetSfxLevel(defaultSfxVolume);
        SetMusicLevel(defaultMusicVolume);
        SetGyroSensitivity(defaultGyroSensitivity);
        SetRotateSensitivity(defaultRotateSensitivity);
        SetShowButtons(defaultShowButtons == 1);
        SetInvertRotation(defaultInvertRotation == 1);
        SetInvertGyroX(defaultInvertGyroX == 1);
        SetInvertGyroY(defaultInvertGyroY == 1);
        SetMute(defaultMute == 1);
        SetStartingLevel(defaultStartingLevel);

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

    public void SetRotateSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(ROTATE_SENSITIVITY, sensitivity);
        SaveSettings();
    }

    public void SetShowButtons(bool showButtons)
    {
        int val = showButtons ? 1 : 0;
        PlayerPrefs.SetInt(SHOW_BUTTONS, val);
        SaveSettings();
    }

    public void SetInvertRotation(bool invert)
    {
        int val = invert ? 1 : 0;
        PlayerPrefs.SetInt(INVERT_ROTATION, val);
        SaveSettings();
    }

    public void SetInvertGyroX(bool invert)
    {
        int val = invert ? 1 : 0;
        PlayerPrefs.SetInt(INVERT_GYRO_X, val);
        SaveSettings();
    }

    public void SetInvertGyroY(bool invert)
    {
        int val = invert ? 1 : 0;
        PlayerPrefs.SetInt(INVERT_GYRO_Y, val);
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
