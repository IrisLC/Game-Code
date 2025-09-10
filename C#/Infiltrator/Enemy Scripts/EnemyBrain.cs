using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine;
using Unity.VisualScripting.FullSerializer;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] public bool isDebugging;
    [SerializeField] protected int health;
    [SerializeField] private int ammo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private Vector3 gunSpread;
    [SerializeField] private float thisEnemyAlertLvl;
    public float minAlertLvl = 0;

    //Alert Levels that are used to deal with basewide alert increases
    private float lastUpdatedAlertLevel = 0;
    private float updateCoolDown = 30f;
    private float updateTime = 0f;

    protected EnemyReference eRef;
    private StateMachine sm;


    public Vector3 lastSeenPosition;
    public bool lostPlayer = false;
    public bool hasBeenShot = false;
    public bool isInvestigating = false;

    public bool isSupporting;

    public InvestigateState investigate;


    /* Hearing */
    public bool isHearing = false;
    [SerializeField] private GameObject sfxInvestBase;
    private GameObject soundInvestigatable;
    [SerializeField] private GameObject corpse;

    // Audio
    [SerializeField] private AudioSource mySteps;

    void Awake()
    {
        eRef = GetComponent<EnemyReference>();
        sm = new StateMachine();

    }

    void Start()
    {
        soundInvestigatable = Instantiate(sfxInvestBase, new Vector3(0, 9999, 0), transform.rotation);
        soundInvestigatable.layer = LayerMask.NameToLayer("Investigatable");

        //Add the enemy to the list of the base alert system
        eRef.baseAlert.addEnemyToList(this);

        //Set up State Machine Transitions
        PatrolState patrol = new PatrolState(eRef);
        AttackState attack = new AttackState(eRef, gunSpread);
        ReloadState reload = new ReloadState(eRef);

        //Not actively set up to be used
        CombAreaState comb = new CombAreaState(eRef);
        investigate = new InvestigateState(eRef, comb);


        sm.AddTransition(attack, patrol, lowAlert());
        sm.AddTransition(attack, reload, outOfAmmo());
        sm.AddTransition(reload, attack, reload.isDone);
        sm.AddTransition(patrol, investigate, hasSeenInvestigatable());
        sm.AddTransition(patrol, investigate, hasHeardImportant());
        sm.AddTransition(investigate, comb, investigate.hasArrived);
        sm.AddTransition(comb, patrol, comb.isDone);

        sm.AddAnyTransition(attack, attackPlayer());
        sm.AddAnyTransition(attack, wasShot());

        sm.SetState(patrol);

        Func<bool> attackPlayer() => () => (isSupporting || (thisEnemyAlertLvl >= 75 && eRef.vision.isSeeingPlayer) && reload.isDone());
        Func<bool> lowAlert() => () => thisEnemyAlertLvl < 75;
        Func<bool> outOfAmmo() => () => ammo <= 0;
        Func<bool> wasShot() => () => hasBeenShot;
        Func<bool> hasSeenInvestigatable() => () => eRef.vision.isSeeingInvestigatable;
        Func<bool> hasHeardImportant() => () => isHearing;

        
        eRef.p.soundMade += checkHeardSound;
    }

    // Update is called once per frame
    void Update()
    {

        // If the enemy is out of health destroy it
        if (!isAlive())
        {
            eRef.baseAlert.removeEnemyFromList(this);
            eRef.p.soundMade -= checkHeardSound;
            Instantiate(corpse, transform.position, Quaternion.Euler(0, 0, 90));
            Destroy(gameObject);
        }

        // Handle step SFX
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            if (!mySteps.isPlaying)
            {
                mySteps.Play();
            }
        }
        else if (mySteps.isPlaying)
        {
            mySteps.Stop();
        }

            visionCall();


        sm.tick();
    }

    protected void visionCall()
    {
        bool oldIsSeeing = eRef.vision.isSeeingPlayer;
        eRef.vision.look();
        // Check if the player just escaped enemy LOS or is actively being seen
        if (oldIsSeeing && !eRef.vision.isSeeingPlayer)
        {
            lostPlayer = true;
            lastSeenPosition = eRef.p.transform.position;
        }
        else if (eRef.vision.isSeeingPlayer)
        {
            lostPlayer = false;
            isSupporting = false;
            lastSeenPosition = eRef.p.transform.position;
        }
        else
        {

            if (!isSupporting)
            {
                // if the enemy is not looking at the player then slowly decrease alertLevel
                //the lower the alert leve, the slower it will decrease
                increaseAlert(-((float)Math.Sqrt(thisEnemyAlertLvl) / 8.0f * Time.deltaTime));
            }
        }
    }

    /* Alert Methods */

    public virtual void increaseAlert(float modifier)
    {
        float oldAlert = thisEnemyAlertLvl;
        // Caps alert between 100 and the minimum
        if (thisEnemyAlertLvl + modifier >= 100.0f)
        {
            thisEnemyAlertLvl = 100;
            modifier = 0;

        }
        else if (thisEnemyAlertLvl + modifier <= minAlertLvl)
        {
            //ensures that increases to basewide alert lvl are not affected by changes
            //to alert lvl of an enemy due to increases of minimum level
            lastUpdatedAlertLevel += minAlertLvl - thisEnemyAlertLvl;
            thisEnemyAlertLvl = minAlertLvl;
            modifier = 0;
        }



        thisEnemyAlertLvl += modifier;

        // Calls various methods that rely on changes in alert level if the level has actually changed

        if (oldAlert != thisEnemyAlertLvl)
        {
            // Changes vision cone based on how much the alert level has increased
            eRef.vision.widenView(thisEnemyAlertLvl - oldAlert);
        }

        // Updates the base on the enemy's alert level changes
        if (!isSupporting && Time.time > updateTime)
        {
            float lvlDif = thisEnemyAlertLvl - lastUpdatedAlertLevel;

            //If the Enemy is actively attacking the player
            if (eRef.vision.isSeeingPlayer && thisEnemyAlertLvl >= 75)
            {
                //Debug.Log(2);
                eRef.baseAlert.updateBaseAlertLvl(5);
            }
            else
            {
                eRef.baseAlert.updateBaseAlertLvl(lvlDif / 5);
            }

            //Sets a new time to update, as well as the last updated level
            updateTime = Time.time + updateCoolDown;
            lastUpdatedAlertLevel = thisEnemyAlertLvl;
        }


    }

    public float getAlertLvl()
    {
        return thisEnemyAlertLvl;
    }

    public void tryRaiseMinAlert(float newLvl)
    {
        if (minAlertLvl < newLvl)
        {
            minAlertLvl = newLvl;
        }
    }

    public void lowerMinAlert(float lowerAmount)
    {
        minAlertLvl -= lowerAmount;
    }

    /* Health methods */

    protected bool isAlive()
    {
        return health > 0;
    }

    public void damageEnemy(int dmg)
    {
        
        health -= dmg;
        Debug.Log(health);
        hasBeenShot = true;

        increaseAlert(100);
        tryRaiseMinAlert(25);

        if (!eRef.brain.isSupporting)
        {
            eRef.baseAlert.enemyIsAttackingPlayer(eRef.brain);
        }

    }

    /* Methods for reload scripts */

    public int getAmmoCount()
    {
        return ammo;
    }

    public void fireAmmo()
    {
        ammo--;
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void reloadAmmo()
    {
        ammo = maxAmmo;
    }

    public void checkHeardSound(float soundRadius, float minAlertLvlToNotice, float alertRaise, float searchArea, Vector3 pos)
    {
        //if(isDebugging)Debug.Log(Vector3.Distance(pos, transform.position));
        //Debug.Log(pos);
        if (thisEnemyAlertLvl >= minAlertLvlToNotice && Vector3.Distance(pos, transform.position) <= soundRadius)
        {

            //Debug.Log(1);


            NavMeshPath path = new();
            //if no path could be made then leave this method
            if (!NavMesh.CalculatePath(pos, transform.position, NavMesh.AllAreas, path))
            {
                return;
            }

            float totalDist = 0f;
            //Debug.Log(path.corners.Length);
            for (int index = 1; index < path.corners.Length; index++)
            {
                float dist = Vector3.Distance(path.corners[index - 1], path.corners[index]);
                Debug.DrawLine(path.corners[index - 1], path.corners[index], Color.red, 10f);
                if (totalDist + dist < soundRadius)
                {
                    totalDist += dist;
                }
                else
                {
                    //Then the sound has traveled too far
                    return;
                }
            }

            heardSound(soundRadius, minAlertLvlToNotice, alertRaise, searchArea, pos);
        }


    }

    public void heardSound(float soundRadius, float minAlertLvlToNotice, float alertRaise, float searchArea, Vector3 pos)
    {

        if (lostPlayer)
        {
            lastSeenPosition = pos;
        }

        //If the enemy is investigating, only make them turn to this sound if it is more important than what they are looking at
        if (isInvestigating || isHearing)
        {
            if (minAlertLvlToNotice < investigate.interest.minToInterest)
            {
                return;
            }
        }

        isHearing = true;
        isInvestigating = true;
        soundInvestigatable.transform.position = pos;
        investigate.interest = soundInvestigatable.GetComponent<Investigatable>();
        investigate.interest.alertRaiseAmount = alertRaise;
        investigate.interest.searchDistance = searchArea;
        investigate.interest.isOfInterest = true;
        // Temp testing
        //Debug.Log("Enemy heard a noise.");

    }
}
