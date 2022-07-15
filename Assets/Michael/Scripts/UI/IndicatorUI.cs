using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorUI : MonoBehaviour
{
    GameObject Idle;
    GameObject Detected;
    GameObject Chasing;

    void Start()
    {
        Idle = GameObject.Find("IdlePNG");
        Detected = GameObject.Find("DetectedPNG");
        Chasing = GameObject.Find("ChasingPNG");

        Idle.SetActive(true);
        Detected.SetActive(false);
        Chasing.SetActive(false);
    }

    public void updateState(int state)
    {
        Idle.SetActive(false);
        Detected.SetActive(false);
        Chasing.SetActive(false);

        switch (state)
        {
            case 1:
                Idle.SetActive(true);
                break;

            case 2:
                Detected.SetActive(true);
                break;

            case 3:
                Chasing.SetActive(true);
                break;
        }
    }
}
