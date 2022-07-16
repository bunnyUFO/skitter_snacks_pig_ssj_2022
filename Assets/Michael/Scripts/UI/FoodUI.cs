using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    Image imageEmpty;
    Image imageFull;

    private void Start()
    {
        imageEmpty = GameObject.Find("EmptyDorito").GetComponent<Image>();
        imageFull = GameObject.Find("FullDorito").GetComponent<Image>();

        GetComponent<Image>().sprite = imageEmpty.sprite;
    }
    public void foodActive()
    {
        GetComponent<Image>().sprite = imageFull.sprite;
    }
}
