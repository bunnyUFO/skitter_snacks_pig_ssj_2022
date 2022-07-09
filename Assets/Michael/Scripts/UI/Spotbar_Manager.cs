using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spotbar_Manager : MonoBehaviour
{
    Slider slide;

    float spotLevel = 0;

    void Start()
    {
        slide = GetComponent<Slider>();
    }

    void Update()
    {
        slide.value = spotLevel;
    }

    public void IncreaseSpot(int value)
    {
        spotLevel += value * Time.deltaTime;
    }

    public void DecrementSpot(int value)
    {
        spotLevel -= value * Time.deltaTime;
    }
}
