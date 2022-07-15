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

    void Start()
    {

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

    public void maxSpot()
    {
        spotbarValue = spotbarSize;
    }
}
