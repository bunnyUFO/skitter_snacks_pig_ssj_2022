using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// To add more doritos to UI, first create a gameobject called Food6
// Add FoodUI script to that game object
// Create 2 Images one called Empty6 and one called Full6 within the gameobject
// In the Game Object's Empty and Full public fields, write Empty6 and Full6
// Then add Doritos.Add(GameObject.Find("Food6")); below the others
// Finished

public class UIManager : MonoBehaviour
{
    [Header("Food")]
    public List<Food> foodList = new List<Food>();
    public int foodCollected = 0;
    public List<GameObject> Doritos = new List<GameObject>();

    GameObject fullFood;
    GameObject emptyFood;


    void Start()
    {
        foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
        {
            foodList.Add(food.GetComponent<Food>());
        }

        Doritos.Add(GameObject.Find("Slot1"));
        Doritos.Add(GameObject.Find("Slot2"));
        Doritos.Add(GameObject.Find("Slot3"));
        Doritos.Add(GameObject.Find("Slot4"));
        Doritos.Add(GameObject.Find("Slot5"));
        Doritos.Add(GameObject.Find("Slot6"));
        Doritos.Add(GameObject.Find("Slot7"));
    }

    public void updateFood()
    {
        foodCollected++;
        Doritos[foodCollected-1].GetComponent<FoodUI>().foodActive();
        Debug.Log("Collecting Food " + foodCollected);
    }

    public void updateIndicator(int state)
    {
        GameObject.Find("Indicator").GetComponent<IndicatorUI>().updateState(state);
    }

    public bool hasWon()
    {
        if (foodCollected == 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
