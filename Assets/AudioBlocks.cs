using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBlocks : MonoBehaviour
{
    public AudioSpectrum audioSpectrum;
    
    public GameObject block;
    public int numberCubes;
    public float blockWidth = 0.0416f;
    public float blockHeight = 0.1125f;

    public int minX = -25, maxX = 25, minY = -39, maxY = 39;

    public float scaleSpeed = 10;
    public float spectrumPower = 1;
    public float spectrumPreScale = 1;
    public float spectrumScale;

    public bool randomSpectrum;

    public bool randomMaterial = true;
    public Material[] materials;

    private List<GameObject> blocks = new List<GameObject>();
    
    void Start()
    {
        for (int i=0; i<numberCubes; i++)
        {
            GameObject newBlock = GameObject.Instantiate(block);
            newBlock.transform.parent = transform;

            if (randomMaterial)
            {
                newBlock.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
            }
            
            int randomX;
            if (Random.value > 0.5f)
            {
                randomX = Random.Range(minX, -1);
            } else
            {
                randomX = Random.Range(1, maxX);
            }
            int randomY = Random.Range(minY, maxY);

            float x = (randomX + 0.5f) * blockWidth;
            float y = (randomY + 0.5f) * blockHeight;
            newBlock.transform.localPosition = new Vector3(x, -0.1f, y);
            blocks.Add(newBlock);
        }
    }
    
    void Update()
    {
        for(int i=0; i<numberCubes; i++)
        {
            float spectrumValue;
            if (randomSpectrum)
            {
                spectrumValue = audioSpectrum.RandomValue();
            }
            else
            {
                spectrumValue = audioSpectrum.spectrum[i];
            }

            spectrumValue = spectrumScale * Mathf.Pow(spectrumPreScale * spectrumValue, spectrumPower);

            Vector3 localScale = new Vector3(blockWidth, spectrumValue, blockHeight);
            blocks[i].transform.localScale = Vector3.Lerp(blocks[i].transform.localScale, localScale, scaleSpeed * Time.deltaTime);
        }
    }
}
