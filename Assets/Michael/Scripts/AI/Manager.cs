using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    INFO info;
    Phermone phermone;
    Spotbar spotbar;
    AI ai;
    AudioManager audioManager;
    UIManager uiManager;

    [Header("Found Enemies")]
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Speed Settings")]
    public float speed;
    public float angularSpeed;
    public float acceleration;

    [Header("AI Settings")]
    public float PatrolpointRange;
    public float returnTimer;

    [Header("FOV Settings")]
    public float FOV;
    public float viewRange;
    public LayerMask layermask;

    [Header("Phermone Settings")]
    public float expandSizePerSecond;
    public float maxSize;
    public float phermoneTimer;

    [Header("Spot Settings")]
    public float barSize;
    public float barDecrease;
    public float shortRangeBarIncrease;
    public float midRangeBarIncrease;
    public float longRangeBarIncrease;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            info = enemies[i].GetComponent<INFO>();
            info.changeSpeed(speed, angularSpeed, acceleration);
            info.changeAI(PatrolpointRange, returnTimer, phermoneTimer);
            info.changeFOV(FOV, viewRange, layermask);

            phermone = enemies[i].GetComponentInChildren<Phermone>();
            phermone.expansionSpeed = expandSizePerSecond;
            phermone.maxScale = maxSize;
            phermone.timer = maxSize / expandSizePerSecond;
            phermone.reset = phermone.timer;

            spotbar = enemies[i].GetComponentInChildren<Spotbar>();
            spotbar.spotbarSize = barSize;
            spotbar.reduceSpotValue = barDecrease;
            spotbar.shortSpotValue = shortRangeBarIncrease;
            spotbar.mediumSpotValue = midRangeBarIncrease;
            spotbar.farSpotValue = longRangeBarIncrease;
            // Perhaps add an addition to decide what long, mid and short range is? Currently those values are > 1/2 long, > 1/4 && < 1/2 mid, < 1/4 short
            

        }
    }

    public void changeMusic()
    {
        bool idle = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            ai = enemies[i].GetComponent<AI>();

            if (ai.isSpotting())
            {
                idle = false;
                if (audioManager.returnState() != 2 && audioManager.returnState() != 3)
                {
                    audioManager.playMusic("Detected");
                    uiManager.updateIndicator(2);
                }
            }

            if (ai.isChasing())
            {
                idle = false;
                if (audioManager.returnState() != 3)
                {
                    audioManager.playMusic("Chasing");
                    uiManager.updateIndicator(3);
                }
            }
        }

        if (idle)
        {
            if (audioManager.returnState() != 1)
            {
                audioManager.playMusic("Idle");
                uiManager.updateIndicator(1);
            }
        }
    }
}
