 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CameraBrain : EnemyBrain
{
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewDistance;
    [SerializeField] private float alertIncreaseBaseAmount;
    [SerializeField] private Light sLight;
    [SerializeField] private GameObject lookPoint;

    [SerializeField] private float speed = 25f;
    [SerializeField] private float angleToTurn = 90f;
    private float startAngle;

    private AudioSource myAudio;

    void Awake()
    {
        eRef = GetComponent<EnemyReference>();

    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - 50), Color.green, 20);
        startAngle = gameObject.transform.rotation.eulerAngles.y;
        // Debug.Log(startAngle);

        eRef.baseAlert.addCameraToList(this);
        eRef.vision.canWidenView = false;

        viewAngle = sLight.spotAngle / 2;

        viewAngle = (float)Math.Cos(viewAngle * (Math.PI / 180));

        viewDistance = sLight.range * 0.75f;

        eRef.vision.changeParameters(viewAngle, viewDistance);

        eRef.vision.defaultAlertIncrease = alertIncreaseBaseAmount;

        eRef.vision.lookPoint = lookPoint;

        health = 1;

        myAudio = GetComponent<AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive())
        {
            eRef.baseAlert.removeCameraFromList(this);
            Destroy(gameObject);
        }



        visionCall();

        rotate();
    }


    public override void increaseAlert(float modifier)
    {
        
        isSupporting = true;
        eRef.baseAlert.updateBaseAlertLvl(modifier);
        //Debug.Log(eRef.vision.isSeeingPlayer);
        //Debug.Log(eRef.baseAlert.getBaseWideAlertLevel());
        if (eRef.vision.isSeeingPlayer && eRef.baseAlert.getBaseWideAlertLevel() >= 50f)
        {
            //Debug.Log("Help");
            eRef.baseAlert.enemyIsAttackingPlayer((EnemyBrain)this);
        }
    }

    private void rotate()
    {
        transform.localEulerAngles = new Vector3(0, Mathf.PingPong(Time.time * speed, angleToTurn) + startAngle, 0);
    }

    public void ToggleAlarm(bool active)
    {
        if (active)
        {
            myAudio.Play();
        }
        else
        {
            myAudio.Stop();
        }
    }
}
