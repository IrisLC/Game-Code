using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool startMoving;
    private Vector2 prevPosition, nextPosition;
    private float nextTime;
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private Animator walk;
    private bool canPlay = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        walkSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startMoving)
        {
            transform.position = Vector2.Lerp(prevPosition, nextPosition, Time.time - nextTime);
            if (!walkSound.isPlaying && canPlay)
            {
                walkSound.Play();
                StartCoroutine(soundReset());
            }
        }
        else
        {
            walkSound.Stop();
        }

    }

    private IEnumerator soundReset()
    {
        canPlay = false;
        yield return new WaitForSeconds(.5f);
        canPlay = true;
    }

    public void newPosition(Vector2 prev, Vector2 next)
    {
        startMoving = true;
        prevPosition = prev;
        nextPosition = next;
        nextTime = Time.time;
        walk.SetBool("isMoving", true);
        if (next.y > prev.y)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (next.y < prev.y)
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (next.x > prev.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }
        else if (next.x < prev.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }

    public void endMove()
    {
        startMoving = false;
        walk.SetBool("isMoving", false);
    }

    public void bump(Vector2 prev, Vector2 next)
    {
        startMoving = true;
        prevPosition = prev;
        nextPosition = next;
        nextTime = Time.time;
        walk.SetBool("isMoving", true);
        if (next.y > prev.y)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (next.y < prev.y)
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (next.x > prev.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
        }
        else if (next.x < prev.x)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
        }

        StartCoroutine(returnBump());
    }

    private IEnumerator returnBump() {
        yield return new WaitForSeconds(0.25f);
        Vector2 temp = prevPosition;
        prevPosition = nextPosition;
        nextPosition = temp;
        nextTime = Time.time - 0.75f;
    }
}
