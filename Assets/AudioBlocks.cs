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

    public float originalBlockWidth = 0.39f;
    public float originalBlockHeight = 0.117f;

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
                //newBlock.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
                int index = (int)Util.ConvertScale(0, numberCubes, 0, materials.Length, i);
                newBlock.GetComponent<Renderer>().material = materials[index];
            }

            Vector2 randomPos = GetRandomPos();
            float x = (randomPos.x + 0.5f) * blockWidth;
            float y = (randomPos.y + 0.5f) * blockHeight;
            newBlock.transform.localPosition = new Vector3(x, 0.1f, y);
            blocks.Add(newBlock);
        }
    }

    List<Vector2> posList = new List<Vector2>();
    Vector2 GetRandomPos()
    {
        Vector2 randomPos;

        do
        {
            if (Random.value > 0.5f)
            {
                randomPos.x = Random.Range(minX, -1);
            }
            else
            {
                randomPos.x = Random.Range(1, maxX);
            }
            
            randomPos.y = Random.Range(minY, maxY);

        } while (posList.Contains(randomPos));

        posList.Add(randomPos);

        return randomPos;
    }

    public float minScale = 0.1f;

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

            Vector3 newScale = blocks[i].transform.localScale;
            newScale.x = originalBlockWidth;
            newScale.z = originalBlockHeight;
            newScale.y = Mathf.Lerp(newScale.y, spectrumValue, scaleSpeed * Time.deltaTime);
            if (newScale.y < minScale)
            {
                newScale.y = minScale;
            }
            blocks[i].transform.localScale = newScale;
        }
    }
}
