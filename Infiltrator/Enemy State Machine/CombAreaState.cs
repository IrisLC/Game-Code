using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CombAreaState : IState
{
    private EnemyReference eRef;
    private List<Vector3> pathPoints;
    private Vector3 currDest;
    private int currIndex;

    public Investigatable interest;

    private bool done;

    public CombAreaState(EnemyReference eRef) {
        this.eRef = eRef;
    }

    public void OnEnter()
    {
        Debug.Log("combing");
        currIndex = 0;
        done = false;
        pathPoints = new List<Vector3>();
        calculateRoute();
        if(pathPoints.Count == 0) {
            done = true;
            return;
        }
        currDest = pathPoints[0];
        eRef.nav.SetDestination(currDest);
    }

    public void OnExit()
    {
        
        eRef.brain.isInvestigating = false;
        eRef.brain.isHearing = false;
        interest.isOfInterest = false;
        if(interest.isSound) {
            interest.gameObject.transform.Translate(new Vector3(0, 9999, 0));
        }

        interest.use();
    }

    public void Tick()
    {
        
        if(currDest.y != eRef.e.transform.position.y) {
            currDest.y = eRef.e.transform.position.y;
            
        }

        if(eRef.e.transform.position == currDest) {
            currIndex++;
            if(currIndex >= pathPoints.Count) {
                
                done = true;
                return;
            }
            Debug.Log(currIndex);
            Debug.Log(pathPoints.Count);
            currDest = pathPoints[currIndex];
            eRef.nav.SetDestination(currDest);
            //Debug.Log(currDest);
        }
    }

    private void calculateRoute() {
        NavMeshPath[] paths = new NavMeshPath[4];

        for(int i = 0; i < paths.Length; i++) {
            paths[i] = new NavMeshPath();
        }
        //float t = Time.time;
        //Calculates and stores the paths to move a distance away from the object in 4 cardinal directions
        float shorteningFunction = 0;
        int breakLimit = 0;
        while(!NavMesh.CalculatePath(eRef.e.transform.position, 
            eRef.e.transform.position + eRef.e.transform.forward * (interest.searchDistance - shorteningFunction), 
            NavMesh.AllAreas, paths[0]) && interest.searchDistance - shorteningFunction > 1) {
            shorteningFunction += 1;
                
            breakLimit++;
            if(breakLimit > 999) {
                Debug.Log("Broke1");
                return;
            }
        };
        shorteningFunction = 0;
        while(!NavMesh.CalculatePath(eRef.e.transform.position, 
            eRef.e.transform.position + eRef.e.transform.right * (interest.searchDistance - shorteningFunction), 
            NavMesh.AllAreas, paths[1]) && interest.searchDistance - shorteningFunction > 1) {
            shorteningFunction += 1;
                breakLimit++;
            if(breakLimit > 999) {
                Debug.Log("Broke2");
                return;
            }
            };
        shorteningFunction = 0;
        while(!NavMesh.CalculatePath(eRef.e.transform.position, 
            eRef.e.transform.position + (-eRef.e.transform.forward * (interest.searchDistance - shorteningFunction)),
             NavMesh.AllAreas, paths[2]) && interest.searchDistance - shorteningFunction > 1) {
            shorteningFunction += 1;
                breakLimit++;
            if(breakLimit > 999) {
                Debug.Log("Broke3");
                return;
            }
        };
        shorteningFunction = 0;
        while(!NavMesh.CalculatePath(eRef.e.transform.position, 
            eRef.e.transform.position + (-eRef.e.transform.right * (interest.searchDistance - shorteningFunction)), 
            NavMesh.AllAreas, paths[3]) && interest.searchDistance - shorteningFunction > 1) {
            shorteningFunction += 1;
                breakLimit++;
            if(breakLimit > 999) {
                Debug.Log("Broke4");
                return;
            }
            };
        //Debug.Log(Time.time - t);
        foreach(NavMeshPath path in paths) {
            float totalDist = 0f;
            int index = 0;
            for(index = 1; index < path.corners.Length; index++) {
                //Debug.DrawLine(path.corners[index - 1], path.corners[index], Color.red, 10f);
                float dist = Vector3.Distance(path.corners[index - 1], path.corners[index]);
                if(totalDist + dist < interest.searchDistance) {
                    totalDist += dist;
                } else {
                    break;
                }
            }
            if(index >= path.corners.Length) {
                index = path.corners.Length - 1;
            }
            if(index < 0) index = 0;
            if(path.corners.Length != 0) {
                //Debug.Log(path.corners.Length);
                //Debug.Log(index);
                Debug.Log(path.corners[index]);
                pathPoints.Add(path.corners[index]);
            }
            
        }
        
    }

    public bool isDone() {
        return done;
    }
}
