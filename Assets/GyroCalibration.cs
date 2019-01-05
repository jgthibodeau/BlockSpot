using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GyroCalibration : MonoBehaviour
{
    public int gyroCalibrationDelay = 3;
    public int gyroCalibrationCount = 100;
    public Vector3 gyroDrift;

    public TMPro.TMP_Text text;

    public Button[] buttons;

    [TextArea]
    public string defaultText = "To calibrate, place your device on a flat surface and press \"Calibrate\"";

    [TextArea]
    public string inProgressText = "Please wait, this process will take a few seconds";

    Menu gyroCalibrationInProgressMenu;

    void Start()
    {
        DefaultView();
    }

    public void TriggerGyroCalibration()
    {
        StartCoroutine(CalibrateGyro());
    }

    public void ResetCalibration()
    {
        gyroDrift = Vector3.zero;
        SaveCalibration();
    }

    private IEnumerator CalibrateGyro()
    {
        InprogressView();

        //wait seconds
        for (int i=gyroCalibrationDelay; i>0; i--)
        {
            yield return new WaitForSeconds(1);
            text.text = "" + i;
        }

        InprogressView();

        //listen for 3 seconds and get an average drift
        gyroDrift = Vector3.zero;
        for (int i = 0; i < gyroCalibrationCount; i++)
        {
            text.text = inProgressText + "  " + i + "/" + gyroCalibrationCount;
            gyroDrift += Input.gyro.rotationRateUnbiased;
            yield return null;
        }
        gyroDrift = gyroDrift / gyroCalibrationCount;

        //store calibration
        SaveCalibration();

        DefaultView();
    }

    private void SaveCalibration()
    {
        PlayerPrefs.SetFloat(Options.GYRO_CALIBRATION + 'x', gyroDrift.x);
        PlayerPrefs.SetFloat(Options.GYRO_CALIBRATION + 'y', gyroDrift.y);
        PlayerPrefs.SetFloat(Options.GYRO_CALIBRATION + 'z', gyroDrift.z);
        Options.SaveSettings();
    }

    private void InprogressView()
    {
        text.text = inProgressText;

        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    private void DefaultView()
    {
        text.text = defaultText;
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}
