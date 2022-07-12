using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody; 
    // Start is called before the first frame update
    public void OnTriggerStay(Collider other)
    {
        rigidbody.useGravity = false;
    }
    
    public void OnTriggerExit(Collider other)
    {
        rigidbody.useGravity = true;
    }
}
