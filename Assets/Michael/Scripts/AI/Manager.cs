using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    INFO info;
    Phermone phermone;
    Spotbar spotbar;
    AudioManager audioManager;
    UIManager uiManager;

    [Header("Found Enemies")]
    public List<AI> enemies = new List<AI>();

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
    public float phermoneResetTimer = 5.0f;
    float reset;

    [Header("Spot Settings")]
    public float barSize;
    public float barDecrease;
    public float shortRangeBarIncrease;
    public float midRangeBarIncrease;
    public float longRangeBarIncrease;

    [Header("Teleport Settings")]
    public float loadTimer = 1.0f;
    public bool allowWarp = false;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy.GetComponent<AI>());
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
            if (spotbar)
            {
                spotbar.spotbarSize = barSize;
                spotbar.reduceSpotValue = barDecrease;
                spotbar.shortSpotValue = shortRangeBarIncrease;
                spotbar.mediumSpotValue = midRangeBarIncrease;
                spotbar.farSpotValue = longRangeBarIncrease;
            }
            
            // Perhaps add an addition to decide what long, mid and short range is? Currently those values are > 1/2 long, > 1/4 && < 1/2 mid, < 1/4 short
        }

        reset = phermoneResetTimer;
    }

    private void Update()
    {
        loadTimer -= Time.deltaTime;

        if(loadTimer < 0)
        {
            allowWarp = true;
        }

        phermoneResetTimer -= Time.deltaTime;
    }

    public void changeMusic()
    {
        bool idle = true;

        if (enemies.Any(enemy => enemy.isSpotting()) )
        {
            idle = false;
            if (audioManager.returnState() != 2 && audioManager.returnState() != 3)
            {
                audioManager.playMusic("Detected");
                uiManager.updateIndicator(2);
            }
        }

        if (enemies.Any(enemy => enemy.isChasing()) )
        {
            idle = false;
            if (audioManager.returnState() != 3)
            {
                audioManager.playMusic("Chasing");
                uiManager.updateIndicator(3);
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

    public bool checkPhermones()
    {
        bool releasePhermones = true;

        if (enemies.Any(enemy => enemy.isPhermoned()) || phermoneResetTimer > 0)
        {
            releasePhermones = false;
        }

        phermoneResetTimer = reset;

        return releasePhermones;
    }
}
