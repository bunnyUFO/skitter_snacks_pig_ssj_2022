using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phermone : MonoBehaviour
{
    bool Expand = false;

    public float expansionSpeed = 2.0f;
    public float maxScale = 10.0f;
    float timer;
    float reset;

    void Start()
    {
        maxScale--;

        GetComponent<MeshRenderer>().enabled = false;

        timer = maxScale / expansionSpeed;
        reset = timer;
    }

    void Update()
    {
        if (Expand && timer >= 0)
        {
            timer -= Time.deltaTime;
            Debug.Log("Expansion time:" + timer);
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            Expand = false;
            transform.localScale = Vector3.one;
        }
    }

    public void ReleasePhermones()
    {
        Expand = true;
        GetComponent<MeshRenderer>().enabled = true;
        timer = reset;
    }

}
