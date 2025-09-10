using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateState : IState
{
    private EnemyReference eRef;
    public Investigatable interest;
    public CombAreaState comb;

    private Vector3 intPos;

    public InvestigateState(EnemyReference eRef, CombAreaState comb) {
        this.eRef = eRef;
        this.comb = comb;
        
    }

    public void OnEnter()
    {
        interest.isOfInterest = false;
        eRef.brain.isInvestigating = true;
        intPos = interest.transform.position;
        eRef.nav.SetDestination(intPos);
        
    }

    public void OnExit()
    {
        eRef.brain.increaseAlert(interest.alertRaiseAmount);
        comb.interest = interest;
    }

    public void Tick()
    {
       
    }

    public bool hasArrived() {
        return Vector3.Distance(eRef.e.transform.position, intPos) <= 1f;
    }

}
