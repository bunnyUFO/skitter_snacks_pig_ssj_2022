using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCam;
    
    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 direction = (this.transform.position - new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z)).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8);
    }
}
