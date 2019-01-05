using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrum : MonoBehaviour
{
    public static float spectrumValue { get; private set; }
    public int denormalizationValue = 100;

    public LineRenderer[] lineRenderers;

    public float[] spectrum = new float[256];

    public float spectrumScale = 4;
    public float spectrumChangeSpeed = 0.5f;
    Vector3[] positions = new Vector3[255];
    
    void Start()
    {
    }

    public float RandomValue()
    {
        int index = Random.Range(0, 256);
        return spectrum[index];
    }
    
    void Update()
    {
        //AudioListener.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);

        //if (audioSpectrum != null && audioSpectrum.Length > 0)
        //{
        //    spectrumValue = audioSpectrum[0] * denormalizationValue;
        //}

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            //Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            //Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);

            Vector3 desiredPosition = new Vector3(0, Mathf.Log(spectrum[i - 1]) / spectrumScale + 10, 2 * i);
            positions[i] = Vector3.Lerp(positions[i], desiredPosition, spectrumChangeSpeed * Time.deltaTime);
        }

        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 255;
            lineRenderer.SetPositions(positions);
        }
    }
}
