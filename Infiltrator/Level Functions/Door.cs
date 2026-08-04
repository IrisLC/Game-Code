using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Investigatable
{
    private bool isOpen = false;

    public bool isLocked;
    public String[] requiredKeycardName;

    [SerializeField] Animator anim;

    private AudioSource myAudio;

    public override void use() {
        // Open/Close the door
        if(isOpen) {
            isOpen = false;
            isOfInterest = false;
            // Play SFX
            myAudio = GetComponent<AudioSource>();
            myAudio.Play();
            // use an animation to close the door
        } else {
            isOpen = true;
            isOfInterest = true;
            // Play SFX
            myAudio = GetComponent<AudioSource>();
            myAudio.Play();
            // use an animation to open the door
        }

        anim.SetBool("doorIsOpened", isOpen);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.CompareTag("Enemy")) {
            isOpen = true;
            anim.SetBool("doorIsOpened", isOpen);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.gameObject.CompareTag("Enemy")) {
            isOpen = false;
            anim.SetBool("doorIsOpened", isOpen);
        }
    }
}
