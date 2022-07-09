using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spotbar : MonoBehaviour
{
    float spotbarValue;
    public float spotbarSize = 10;
    public float reduceSpotValue = 1;

    public float shortSpotValue = 5;
    public float mediumSpotValue = 2;
    public float farSpotValue = 1;

    Image image1;
    Image image2;

    GameObject imageObject1;
    GameObject imageObject2;

    Slider slide;

    void Start()
    {
        slide = GetComponent<Slider>();
        imageObject1 = GameObject.Find("Fill");
        imageObject2 = GameObject.Find("Background");
        reveal(false);
    }

    void Update()
    {
        slide.value = spotbarValue;

        if (spotbarValue > spotbarSize)
        {
            spotbarValue = spotbarSize;
        }
    }

    public void reveal(bool revealStatus)
    {
        imageObject1.SetActive(revealStatus);
        imageObject2.SetActive(revealStatus);
    }

    public bool spotted()
    {
        if (spotbarValue == spotbarSize)
        { 
            return true;
        }
        else
        {
            return false;
        }
    }

    public void increaseSpot(float value)
    {
        if (spotbarValue < spotbarSize)
        {
            switch (value)
            {
                case 1:
                    spotbarValue += farSpotValue * Time.deltaTime;
                    break;

                case 2:
                    spotbarValue += mediumSpotValue * Time.deltaTime;
                    break;

                case 3:
                    spotbarValue += shortSpotValue * Time.deltaTime;
                    break;
            }
        }
        else
        {
            spotbarValue = spotbarSize;
        }    
    }

    public void decreaseSpot()
    {
        if (spotbarValue > 0)
        {
            spotbarValue -= reduceSpotValue * Time.deltaTime;
        }
        else
        {
            spotbarValue = 0;
        }
    }
}
