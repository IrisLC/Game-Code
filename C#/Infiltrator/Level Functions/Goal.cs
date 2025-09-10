using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public bool needsKeyCard;
    public String[] requiredKeyCardName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if(other.transform.gameObject.CompareTag("Player")) {
            Debug.Log("Player Found");
            if(checkIfCanComplete(other.transform.gameObject.GetComponent<Player>())) {


#if UNITY_EDITOR
                // Go to win screen
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(3);
#endif
                // Go to win screen
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(3);
            }
        }
    }

    private bool checkIfCanComplete(Player p) {
        bool hasAll = true;
        if(needsKeyCard) {
            for(int i = 0; i < requiredKeyCardName.Length; i++) {
                if(!p.heldKeyCards.Contains(requiredKeyCardName[i])) {
                    
                    hasAll = false;

                    // TODO add a list of what is needed so the player knows

                    if(requiredKeyCardName[i].Equals("Rifle") || requiredKeyCardName[i].Equals("pistol")) {
                        Debug.Log("test");
                        UI ui = GameObject.Find("UI").GetComponent<UI>();
                        ui.SetSpeaker("Infiltrator");
                        ui.StartCoroutine(ui.DisplayText("My gun's still here somewhere. I don't stand a chance out there without it."));
                    }

                }
            }
            
            return hasAll;
            
        }

        return true;
    }
}
