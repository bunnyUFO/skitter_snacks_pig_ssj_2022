using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public bool Collected = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider touchedObject)
    {
        if (touchedObject.gameObject.CompareTag("Player") && !Collected)
        {
            Collected = true;
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
