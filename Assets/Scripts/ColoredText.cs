using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColoredText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    public void RainbowText()
    {
        
        if (!audioSource.isPlaying)
        {
            StartCoroutine(CicledTextColor());
        } 
        else
        {
            StopAllCoroutines();
            text.color = Color.gray;
        }
    }

    public IEnumerator CicledTextColor()
    {
        List<Color> colorsSet = new List<Color>();

        colorsSet.Add(Color.blue);
        colorsSet.Add(Color.cyan);
        colorsSet.Add(Color.green);
        colorsSet.Add(Color.magenta);
        colorsSet.Add(Color.red);
        colorsSet.Add(Color.yellow);
        while (true)
        {
            for (int i = 0; i < colorsSet.Count; i++)
            {
                if (i % 2 == 1)
                {
                    text.fontSize = 24;
                }
                else
                {
                    text.fontSize = 30;
                }
                text.color = colorsSet[i];
                yield return new WaitForSeconds(0.14f);
            }
        }
    }
}
