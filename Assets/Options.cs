using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    public Slider[] rotateSensitivitySliders, gyroSliders;
    public Toggle[] invertToggles, invertGyroXToggles, invertGyroYToggles;
    public Slider sfxSlider, musicSlider;
    public Toggle /*showButtonToggle,*/ muteToggle;
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

        float sfxVol = PlayerPrefs.GetFloat(SFX_VOL, defaultSfxVolume);
        PlayerPrefs.SetFloat(SFX_VOL, sfxVol);
        sfxSlider.value = sfxVol;

        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOL, defaultMusicVolume);
        PlayerPrefs.SetFloat(MUSIC_VOL, musicVol);
        musicSlider.value = musicVol;

        float gyroSensitivity = PlayerPrefs.GetFloat(GYRO_SENSITIVITY, defaultGyroSensitivity);
        PlayerPrefs.SetFloat(GYRO_SENSITIVITY, gyroSensitivity);
        UpdateSliders(gyroSliders, gyroSensitivity);

        float rotateSensitivity = PlayerPrefs.GetFloat(ROTATE_SENSITIVITY, defaultRotateSensitivity);
        PlayerPrefs.SetFloat(ROTATE_SENSITIVITY, rotateSensitivity);
        UpdateSliders(rotateSensitivitySliders, rotateSensitivity);

        //showButtonToggle.isOn = PlayerPrefs.GetInt(SHOW_BUTTONS, defaultShowButtons) == 1;
        int invertRotation = PlayerPrefs.GetInt(INVERT_ROTATION, defaultInvertRotation);
        PlayerPrefs.SetInt(INVERT_ROTATION, invertRotation);
        UpdateToggles(invertToggles, invertRotation == 1);

        int invertGyroX = PlayerPrefs.GetInt(INVERT_GYRO_X, defaultInvertGyroX);
        PlayerPrefs.SetInt(INVERT_GYRO_X, invertGyroX);
        UpdateToggles(invertGyroXToggles, invertGyroX == 1);

        int invertGyroY = PlayerPrefs.GetInt(INVERT_GYRO_Y, defaultInvertGyroY);
        PlayerPrefs.SetInt(INVERT_GYRO_Y, invertGyroY);
        UpdateToggles(invertGyroYToggles, invertGyroY == 1);

        int mute = PlayerPrefs.GetInt(MUTE, defaultMute);
        PlayerPrefs.SetInt(MUTE, mute);
        muteToggle.isOn = mute == 1;
        SetMute(mute == 1);

        int startingLevel = PlayerPrefs.GetInt(STARTING_LEVEL, defaultStartingLevel);
        PlayerPrefs.SetInt(STARTING_LEVEL, startingLevel);
        startingLevelText.text = "" + (startingLevel + 1);

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
    
    void UpdateSliders(Slider[] sliders, float value)
    {
        foreach (Slider s in sliders)
        {
            s.value = value;
        }
    }
    
    void UpdateToggles(Toggle[] toggles, bool isOn)
    {
        foreach (Toggle t in toggles)
        {
            t.isOn = isOn;
        }
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
        UpdateSliders(gyroSliders, sensitivity);
    }

    public void SetRotateSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(ROTATE_SENSITIVITY, sensitivity);
        SaveSettings();
        UpdateSliders(rotateSensitivitySliders, sensitivity);
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
        UpdateToggles(invertToggles, invert);
    }

    public void SetInvertGyroX(bool invert)
    {
        int val = invert ? 1 : 0;
        PlayerPrefs.SetInt(INVERT_GYRO_X, val);
        SaveSettings();
        UpdateToggles(invertGyroXToggles, invert);
    }

    public void SetInvertGyroY(bool invert)
    {
        int val = invert ? 1 : 0;
        PlayerPrefs.SetInt(INVERT_GYRO_Y, val);
        SaveSettings();
        UpdateToggles(invertGyroYToggles, invert);
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
