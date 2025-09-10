using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    [SerializeField] private GameObject[] roomChecks;

    [SerializeField] private GameObject activeRoom;
    private RoomTrigger activeTrigger;

    private GameObject player;

    public bool isActive;

    [SerializeField] private float timeBetweenMoves;
    [SerializeField] private float timeToMove;

    private int num = -1;

    private bool isKillingPlayer;

    Coroutine co;

    [SerializeField] private GameManager gm;

    [SerializeField] private GameObject subtitlesManager;
    [SerializeField] private GameObject reader;
    AudioManager am;
    public AudioClip StaticSFX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        roomChecks = GameObject.FindGameObjectsWithTag("RoomCollider");
        am = FindAnyObjectByType<AudioManager>();

        //player = GameObject.FindGameObjectWithTag("Player");


        StartCoroutine(moveRoom());
    }

    // Update is called once per frame
    void Update()
    {
        if (GameTimeManager.GetIsGamePaused())
        {
            return;
        }
        
        if (isActive)
        {
            if (activeTrigger == null) return;

            if (activeTrigger.isOccupied && !isKillingPlayer)
            {
                isKillingPlayer = true;
                subtitlesManager.GetComponent<SubtitlesManager>().playRandomSubtitle(reader.GetComponent<Reader>().allSubtitlesList.CaughtInRoom);
                co = StartCoroutine(killPlayer());
            }
            //if the player makes it out
            if (isKillingPlayer && !activeTrigger.isOccupied)
            {
                StopCoroutine(co);
                Debug.Log("Stopped");
                subtitlesManager.GetComponent<SubtitlesManager>().playRandomSubtitle(reader.GetComponent<Reader>().allSubtitlesList.EscapedRoom);
                isKillingPlayer = false;
            }
        }
    }



    IEnumerator moveRoom()
    {
        yield return new WaitForSeconds(timeBetweenMoves);
        int oldNum = num;
        num = UnityEngine.Random.Range(0, roomChecks.Length);

        //Ensure the same room is not picked twice
        if (num == oldNum)
        {
            if (num != roomChecks.Length - 1)
            {
                num++;
            }
            else
            {
                num--;
            }
        }

        //Start doing necessary things to signify ai movement

        if (roomChecks[num].GetComponent<RoomTrigger>().isOccupied)
        {
            subtitlesManager.GetComponent<SubtitlesManager>().playSubtitleSequence(reader.GetComponent<Reader>().allSubtitlesList.AIWarning);
        }

        am.PlaySFX(StaticSFX);

        yield return new WaitForSeconds(timeToMove);

        if (activeRoom != null)
        {
            foreach (Light l in activeTrigger.lights)
            {
                l.color = new UnityEngine.Color(0.7372549f, 0.9254902f, 0.7843137f);
            }
        }

        activeRoom = roomChecks[num];

        activeTrigger = activeRoom.GetComponent<RoomTrigger>();

        foreach (Light l in activeTrigger.lights)
        {
            l.color = new UnityEngine.Color(0.7803922f, 0.4862745f, 0.4862745f);
        }

        StartCoroutine(moveRoom());
    }


    IEnumerator killPlayer()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Death");
        //Kill player
        gm.killPlayer();
    }
}
