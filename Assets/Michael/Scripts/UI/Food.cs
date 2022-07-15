using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    UIManager uiManager;
    public bool Collected = false;

    private void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    private void OnTriggerStay(Collider touchedObject)
    {
        if (touchedObject.gameObject.CompareTag("Player") && !Collected)
        {
            Collected = true;
            GetComponent<MeshRenderer>().enabled = false;
            uiManager.updateFood();
        }
    }
}
