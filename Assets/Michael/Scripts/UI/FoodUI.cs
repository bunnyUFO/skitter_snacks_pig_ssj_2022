using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodUI : MonoBehaviour
{
    GameObject imageEmpty;
    GameObject imageFull;
    public string empty;
    public string full;

    private void Start()
    {
        imageEmpty = GameObject.Find(empty);
        imageFull = GameObject.Find(full);

        imageFull.SetActive(false);
        imageEmpty.SetActive(true);
    }
    public void foodActive()
    {
        imageEmpty.SetActive(false);
        imageFull.SetActive(true);
    }
}
