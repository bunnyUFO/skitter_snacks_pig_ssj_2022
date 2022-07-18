using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{
    private void OnTriggerEnter(Collider touchedObject)
    {
        if (touchedObject.gameObject.CompareTag("Player"))
        {
            touchedObject.SendMessage("Die");
        }
    }
}
