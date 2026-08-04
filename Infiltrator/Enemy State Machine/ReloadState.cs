using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : IState
{
    private bool done = true;

    private EnemyReference eRef;

    public ReloadState(EnemyReference eRef) {
        this.eRef = eRef;
    }

    public void OnEnter()
    {
        eRef.brain.StartCoroutine(reload());
        done = false;
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }

    private IEnumerator reload() {

        yield return new WaitForSeconds(2);

        eRef.brain.reloadAmmo();

        done = true;
    }

    public bool isDone() {
        return done;
    }
}
