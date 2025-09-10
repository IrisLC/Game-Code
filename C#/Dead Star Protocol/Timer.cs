using UnityEngine;
using TMPro;
using System;
using UnityEngine.Rendering;
public class Timer : MonoBehaviour
{

    [SerializeField] private float startingTime;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameManager gm;

    private float timeRemaining;
    public AudioClip alarmSFX;
    bool alarmSoundPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeRemaining = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameTimeManager.GetIsGamePaused())
        {
            return;
        }

        if (timeRemaining >= 0) timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 30 && !alarmSoundPlaying) StartAlarm();
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = String.Format("{00:00}:{1:00}", minutes, seconds);

        if (timeRemaining <= 0)
        {
            //trigger events for game over

            gm.timeUp();
        }
    }
    void StartAlarm()
    {
        alarmSoundPlaying = true;
        AudioManager audioManager = FindAnyObjectByType<AudioManager>();
        audioManager.PlaySFX(alarmSFX);
    }
}
