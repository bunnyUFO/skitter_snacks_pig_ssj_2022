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

    public Image redTriangle;
    public Image orangeTriangle;
    public Image whiteTriangle;

    void Start()
    {
        redTriangle = GameObject.Find("RedTriangle").GetComponent<Image>();
        orangeTriangle = GameObject.Find("OrangeTriangle").GetComponent<Image>();
        whiteTriangle = GameObject.Find("WhiteTriangle").GetComponent<Image>();
        
        //GetComponent<Image>().sprite = 
    }

    void Update()
    {
        if (spotbarValue > spotbarSize)
        {
            spotbarValue = spotbarSize;
        }
    }

    public bool spotted()
    {
        if (spotbarValue == spotbarSize)
        {
            GetComponent<Image>().sprite = redTriangle.sprite;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void increaseSpot(float value)
    {
        GetComponent<Image>().sprite = orangeTriangle.sprite;

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
        GetComponent<Image>().sprite = whiteTriangle.sprite;

        if (spotbarValue > 0)
        {
            spotbarValue -= reduceSpotValue * Time.deltaTime;
        }
        else
        {
            spotbarValue = 0;
        }
    }

    public void maxSpot()
    {
        spotbarValue = spotbarSize;
    }
}
