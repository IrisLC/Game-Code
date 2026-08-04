using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Viewpoint edges")]
    // Measurements held in [-1, 1]
    [SerializeField] private float mainView;
    [SerializeField] private float peripheralView;

    [Header("Other stats")]
    [SerializeField] private float viewDistance;
    public float defaultAlertIncrease;
    [SerializeField] private float alertUpdateDelay = 0.2f;
    public LayerMask mask;

    public GameObject lookPoint;


    private EnemyReference eRef;

    /// <summary>
    /// This is to enable cameras to make use of this script 
    /// </summary>
    public bool canWidenView = true;

    public float distance;
    private float dot;
    private float alertLevelConeIncrease = 0;
    private float alertUpdateDeadline;

    public bool isSeeingPlayer;
    public bool isSeeingInvestigatable;


    // Start is called before the first frame update
    void Awake()
    {
        eRef = GetComponent<EnemyReference>();
        lookPoint = gameObject;
    }

    // Update is called once per frame
    public void look()
    {

        Vector3 ePos = lookPoint.transform.position;
        Vector3 pPos = eRef.p.transform.position;
        isSeeingPlayer = false;
        isSeeingInvestigatable = false;

        // If the player is within the view range of the enemy
        // Ensures calculations only occur if the player could be seen
        distance = Vector3.Distance(ePos, pPos);
        if (distance <= viewDistance)
        {

            // Draws a ray from the enemy to the player, then checks if the player was the first thing hit
            // Allows player to use cover
            /*TODO: draw multiple rays so player must be fully in cover*/
            RaycastHit hit;
            

            if (Physics.Raycast(ePos, pPos - ePos, out hit, viewDistance, mask, QueryTriggerInteraction.Ignore))
            {
                if (eRef.brain.isDebugging)
                {
                    Debug.Log(hit.collider.gameObject);
                }

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    // Checks the dot product of the angle between the player and enemy
                    // if the enemy is facing the player will give 1, -1 if behind, 0 if beside
                    dot = Vector3.Dot(lookPoint.transform.forward, (pPos - ePos).normalized);


                    // Increase alert level based on where the player was seen, both position and distance 
                    if (dot > mainView - alertLevelConeIncrease)
                    {
                        isSeeingPlayer = true;

                        tryToRaise(defaultAlertIncrease);
                        
                        //Debug.DrawRay(ePos, pPos - ePos, Color.red, 10f);
                    }
                    else if (dot > peripheralView - alertLevelConeIncrease)
                    {
                        isSeeingPlayer = true;

                        tryToRaise(defaultAlertIncrease / 2);
                        
                        //Debug.DrawRay(ePos, pPos - ePos, Color.blue, 10f);
                    }
                }
            }
            



        }

        /*TODO - Don't check every frame */
        if (!eRef.brain.isInvestigating)
        {
            List<Investigatable> invests = checkInvestigatables();
            float nearestDist = float.MaxValue;
            Investigatable nearestObj = null;
            foreach (Investigatable inv in invests)
            {
                RaycastHit hit;
                Physics.Raycast(ePos, inv.transform.position - ePos, out hit);
                if (hit.collider == null) break;
                if (hit.collider.gameObject == inv.gameObject)
                {
                    // Checks the dot product of the angle between the item and enemy
                    // if the enemy is facing the item will give 1, -1 if behind, 0 if beside
                    dot = Vector3.Dot(lookPoint.transform.forward, (inv.transform.position - ePos).normalized);


                    if (dot > peripheralView - alertLevelConeIncrease)
                    {
                        if (eRef.brain.getAlertLvl() >= inv.minToInterest)
                        {

                            float dist = Vector3.Distance(ePos, inv.transform.position);
                            if (dist < float.MaxValue)
                            {
                                nearestDist = dist;
                                nearestObj = inv;
                            }
                        }
                    }
                }
            }

            if (nearestDist != float.MaxValue)
            {
                isSeeingInvestigatable = true;
                eRef.brain.investigate.interest = nearestObj;
            }
        }
    }

    /// <summary>
    /// calls a raise alert level, if enough time has passed since the last alert increase
    /// </summary>
    /// <param name="lvl">The level to raise by</param>
    private void tryToRaise(float lvl)
    {

        if (Time.time >= alertUpdateDeadline)
        {
            alertUpdateDeadline += alertUpdateDelay;

            eRef.brain.increaseAlert(lvl / distance);
        }

    }


    public void widenView(float modifier)
    {
        if (!canWidenView) return;

        //will move alertLvl from range [0-100] into [0-0.2]
        modifier /= 500;
        // makes it so the higher the alert is the wider the cone is
        alertLevelConeIncrease += modifier;
    }

    private List<Investigatable> checkInvestigatables()
    {
        List<Investigatable> nearbyInvests = new List<Investigatable>();
        float distanceChecked = viewDistance;
        // While it has not found any waypoints

        RaycastHit[] hits = Physics.SphereCastAll(eRef.e.transform.position, distanceChecked, Vector3.forward, 0);

        //checks everything that was hit, and if it was a waypoint, adds it to the list
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.GetComponentInParent<Investigatable>() && !hit.transform.GetComponentInParent<Investigatable>().isSound && hit.transform.GetComponentInParent<Investigatable>().isOfInterest)
            {
                nearbyInvests.Add(hit.transform.GetComponentInParent<Investigatable>());
            }
        }


        return nearbyInvests;
    }

    /// <summary>
    /// Changes both the main and peripheral view to be the same value, as well as changes the distance, used for cameras
    /// </summary>
    /// <param name="view"></param>
    public void changeParameters(float view, float distance)
    {
        mainView = view;
        peripheralView = view;

        viewDistance = distance;
    }
}
