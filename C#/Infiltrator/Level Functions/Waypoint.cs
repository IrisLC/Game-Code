using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{

    public GameObject[] waypoints;
    private int oldLength;

    public int maxCapacity = 1;
    public int activeCapacity = 0;


    // Start is called before the first frame update
    void Start()
    {
        

        
        
    }

    // Update is called once per frame
    // void OnValidate()
    // {


    //     for(int i = 0; i < waypoints.Length; i++ ) {
    //         if(i != 0 && waypoints[i] == waypoints[i - 1]) {
    //             waypoints[i] = null;
    //         }
    //         if(waypoints[i] == null) {
                
    //             waypoints[i] = new GameObject("waypoint" + i);
    //             waypoints[i].transform.position = gameObject.transform.position;
    //             waypoints[i].transform.parent = gameObject.transform;
                
    //         }
            
    //     }

    //     if(oldLength > waypoints.Length) {
    //         String name = "waypoint" + waypoints.Length;
    //         // I don't know why this works, not my code, but seemignly only way to do this.
    //         // Hate this engine
    //         UnityEditor.EditorApplication.delayCall+=()=>
    //         {
    //             DestroyImmediate(GameObject.Find(name));
    //         };
    //     }
        
    //     oldLength = waypoints.Length;
    // }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, Vector3.one);

        try {
            Gizmos.color = Color.cyan;
            for(int i = 0; i < waypoints.Length - 1; i++) {
                Vector3 pos = waypoints[i].transform.position;
            
                Gizmos.DrawSphere(pos, 0.5f);
                Gizmos.DrawRay(pos, waypoints[i + 1].transform.position - pos);
            }

            Vector3 pos1 = waypoints[waypoints.Length - 1].transform.position;
            
            Gizmos.DrawSphere(pos1, 0.5f);
            Gizmos.DrawRay(pos1, waypoints[0].transform.position - pos1);
        } catch(Exception) {

        }
        
    }
}
