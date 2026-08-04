using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAlert : MonoBehaviour
{

    [SerializeField] private List<EnemyBrain> enemyList;
    [SerializeField] private List<CameraBrain> cameraList;

    [SerializeField] private float baseWideAlertLvl = 0;
    [SerializeField] private float notifyDistance = 30f;
    private float toUpdateLevel = 0;
    private float oldToUpdate = 0;

    private UI ui;
    [SerializeField] private float previousAlertLevel = 0;

    void Awake()
    {
        enemyList = new List<EnemyBrain>();
        cameraList = new List<CameraBrain>();
        ui = GameObject.Find("UI").GetComponent<UI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("updateEnemyMinAlert", 0f, 30f);
    }

    private void updateEnemyMinAlert() {
        toUpdateLevel = (float) Math.Pow(3 * baseWideAlertLvl / 40, 2);

        foreach(EnemyBrain e in enemyList) {
            if(oldToUpdate > toUpdateLevel) {
                e.lowerMinAlert(oldToUpdate - toUpdateLevel);
                
            } else {
                e.tryRaiseMinAlert(toUpdateLevel);
            }
            
        }

        oldToUpdate = toUpdateLevel;
    }

    public void updateBaseAlertLvl(float addedValue) {
        
        // Caps alert at 100 and 0
        if (baseWideAlertLvl + addedValue >= 100.0f) {
            baseWideAlertLvl = 100;

            return;
        } else if(baseWideAlertLvl + addedValue <= 0) {
            
            baseWideAlertLvl = 0;
            return;
        }

        previousAlertLevel = baseWideAlertLvl;
        baseWideAlertLvl += addedValue;

        // Logic for updates from the Mole
        if (baseWideAlertLvl < 25 & previousAlertLevel >= 25)
        {
            ui.SetSpeaker("The Mole");
            StartCoroutine(ui.DisplayText("They're starting to relax security measures. I think they've lost you."));
        } else if (baseWideAlertLvl > 75 & previousAlertLevel <= 75)
        {
            ui.SetSpeaker("The Mole");
            StartCoroutine(ui.DisplayText("They've enabled maximum security! Run for it!"));
        } else if (baseWideAlertLvl > 50 & previousAlertLevel <= 50)
        {
            ui.SetSpeaker("The Mole");
            StartCoroutine(ui.DisplayText("The security level just went up again. Are you trying to brute force this? I wouldn't reccomend it."));
        } else if (baseWideAlertLvl > 25 & previousAlertLevel <= 25)
        {
            ui.SetSpeaker("The Mole");
            StartCoroutine(ui.DisplayText("They've raised the base security level! Everything alright down there?"));
        }

        // Logic for toggling camera alarms
        if (baseWideAlertLvl >= 90 & previousAlertLevel < 90)
        {
            foreach (CameraBrain cam in cameraList)
            {
                cam.ToggleAlarm(true);
            }
        } else if (baseWideAlertLvl < 90 & previousAlertLevel >= 90)
        {
            foreach (CameraBrain cam in cameraList)
            {
                cam.ToggleAlarm(false);
            }
        }
    }

    public void addEnemyToList(EnemyBrain enemy) {
        enemyList.Add(enemy);
    }

    public void removeEnemyFromList(EnemyBrain enemy) {
        enemyList.Remove(enemy);
    }

    public void addCameraToList(CameraBrain cam) {
        cameraList.Add(cam);
    }

    public void removeCameraFromList(CameraBrain cam) {
        cameraList.Remove(cam);
    }

    public void enemyIsAttackingPlayer(EnemyBrain e) {
        foreach(EnemyBrain e1 in enemyList) {
            if(e != e1) {
                if(Vector3.Distance(e.gameObject.transform.position, 
                    e1.gameObject.transform.position) <= notifyDistance) {
                    
                    e1.increaseAlert(100);
                    e1.isSupporting = true;
                    e1.lastSeenPosition = e.lastSeenPosition;
                }
            }
        }
    }

    public float getBaseWideAlertLevel() {
        return baseWideAlertLvl;
    }
}
