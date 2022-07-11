using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    Text text;
    string foodString;

    [Header("Found Food")]
    public List<Food> foodList = new List<Food>();

    [Header("Food Collected")]
    public int food;

    void Start()
    {
        text = GetComponentInChildren<Text>();

        foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
        {
            foodList.Add(food.GetComponent<Food>());
        }

        foodString = "Food: " + food + "/" + foodList.Count;
        text.text = foodString;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].Collected && !foodList[i].Sent)
            {
                UpdateText();
                foodList[i].Sent = true;
            }
        }
    }

    void UpdateText()
    {
        food++;

        foodString = "Food: " + food + "/" + foodList.Count;
        text.text = foodString;
    }
}
