using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phermone : MonoBehaviour
{
    bool Expand = false;

    public float expansionSpeed = 2.0f;
    public float maxScale = 10.0f;
    public float timer;
    public float reset;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;

        timer = maxScale / expansionSpeed;
        reset = timer;
        maxScale = maxScale - 0.5f;
    }

    void Update()
    {
        if (Expand && timer >= 0)
        {
            timer -= Time.deltaTime;
            transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            Expand = false;
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void ReleasePhermones()
    {
        Expand = true;
        GetComponent<MeshRenderer>().enabled = true;
        timer = reset;
    }

}
