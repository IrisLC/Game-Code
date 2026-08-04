using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

/// <summary>
/// The default state for enemies when game starts, makes them find the nearest patrol point and follows it
/// </summary>

public class PatrolState : IState
{

    private GameObject[] waypoints;
    private int currIndex;
    private Vector3 currDest;
    public EnemyReference eRef;
    private Waypoint wp;

    private LayerMask mask;

    public PatrolState(EnemyReference eRef) {
        this.eRef = eRef;
        mask = LayerMask.GetMask("Patrol Zone");
    }

    public void OnEnter()
    {
        
        findNearestPatrol();
        
        currIndex = findNearestPoint();
        currDest = new Vector3(waypoints[currIndex].transform.position.x, eRef.e.transform.position.y, waypoints[currIndex].transform.position.z);
        //Debug.Log(currDest);
        eRef.nav.SetDestination(currDest);
    }

    public void OnExit()
    {
        wp.activeCapacity--;
    }

    public void Tick()
    {
        if(currDest.y != eRef.e.transform.position.y) {
            currDest.y = eRef.e.transform.position.y;
        }

        if(Vector3.Distance(eRef.e.transform.position, currDest) <= 1) {
            currIndex++;
            if(currIndex == waypoints.Length) {
                currIndex = 0;
            }
            currDest = new Vector3(waypoints[currIndex].transform.position.x, 
                eRef.e.transform.position.y, waypoints[currIndex].transform.position.z);
            eRef.nav.SetDestination(currDest);
            //Debug.Log(currDest);
        }

    }

    public void findNearestPatrol() {
        List<GameObject> possiblePatrolSpaces = findPatrolSpace();

        wp = findClosest(possiblePatrolSpaces).GetComponent<Waypoint>();

        waypoints = wp.waypoints;

        wp.activeCapacity += 1;
    }

    private List<GameObject> findPatrolSpace() {
        List<GameObject> possibleWaypoints = new List<GameObject>();
        int distanceChecked = 0;
        // While it has not found any waypoints
        do {
            // increases the space being checked each time
            distanceChecked += 5;

            RaycastHit[] hits = Physics.SphereCastAll(eRef.e.transform.position, distanceChecked, Vector3.forward, 0, mask);

            //checks everything that was hit, and if it was a waypoint, adds it to the list
            foreach(RaycastHit hit in hits) {
                if(hit.transform.GetComponentInParent<Waypoint>()) {
                    Waypoint wp = hit.transform.GetComponentInParent<Waypoint>();
                    //if the patrol space is not full
                    if(wp.activeCapacity < wp.maxCapacity){
                        possibleWaypoints.Add(hit.transform.gameObject);
                    }
                }
            }
            if(distanceChecked > 500000) {
                Debug.Log("Limit reached");
                break;
            }
        } while(possibleWaypoints.Count == 0);

        return possibleWaypoints;
    }

    private GameObject findClosest(List<GameObject> possibleWaypoints) {
        GameObject minDistObj;

        if(possibleWaypoints.Count != 1) {

            minDistObj = possibleWaypoints[0];
            float minDist = Vector3.SqrMagnitude(eRef.e.transform.position 
                - minDistObj.transform.position);

            foreach(GameObject gm in possibleWaypoints) {
                float dist = Vector3.SqrMagnitude(eRef.e.transform.position 
                - gm.transform.position);

                if(dist < minDist) {
                    minDist = dist;
                    minDistObj = gm;
                }
            }

           

        } else {
            minDistObj = possibleWaypoints[0];
        }

        return minDistObj;
    }

    private int findNearestPoint() {
        List<GameObject> nodes = new List<GameObject>();

        foreach(GameObject node in waypoints) {
            nodes.Add(node);
        }

        

        GameObject gm = findClosest(nodes);

        // Debug.Log(gm);

        return Array.IndexOf(waypoints, gm);
    }

    

}
