using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    [SerializeField] private Animator playerControl;

    bool isMoving;

    PlayerController pm;
    AudioManager am;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = GetComponent<PlayerController>();
        am = FindAnyObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameTimeManager.GetIsGamePaused())
        {
            return;
        }

        // playerControl.speed = 1;

        if (pm.publicMoveVector == new Vector2(0, 0))
        {
            isMoving = false;
            am.StopFootSteps();
        }
        else
        {
            isMoving = true;
            am.StartFootSteps();


            // if (pm.publicMoveVector.y < 0)
            // {
            //     playerControl.speed = -1;
            // }
        }

        playerControl.SetFloat("direction", pm.publicMoveVector.y);
        playerControl.SetBool("isRunning", isMoving);
    }
}
