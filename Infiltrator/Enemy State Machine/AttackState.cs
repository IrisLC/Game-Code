using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AttackState : IState
{

    public EnemyReference eRef;

    private Vector3 spread;
    public TrailRenderer bulletTrail;
    private float coolDown = 0.5f;
    private float shootTime = 0.0f;

    public AttackState(EnemyReference eRef, Vector3 spreadRange) {
        this.eRef = eRef;
        spread = spreadRange;
        bulletTrail = eRef.bulletTrail;
    }

    public void OnEnter()
    {
        eRef.nav.stoppingDistance = 5;
        eRef.brain.hasBeenShot = false;
        
        
    }

    public void OnExit()
    {
        eRef.nav.stoppingDistance = 0;
    }

    public void Tick()
    {

        if(eRef.brain.lastSeenPosition == new Vector3(0, 0, 0)) {
            eRef.brain.lastSeenPosition = eRef.p.gameObject.transform.position;
        }

        // Chase the player
        if(eRef.nav.isActiveAndEnabled) eRef.nav?.SetDestination(eRef.brain.lastSeenPosition);

        if(eRef.vision.distance <= eRef.nav.stoppingDistance) {
            
            eRef.e.transform.LookAt(eRef.brain.lastSeenPosition);
        }

        if(eRef.vision.isSeeingPlayer) {
            if(Time.time > shootTime) {
                Shoot();
                shootTime = Time.time + coolDown;
            }
            
        }
    }

    private void Shoot() {
        Vector3 direction = GetDirection();

        if(Physics.Raycast(eRef.shootPoint.position, direction, out RaycastHit hit, float.MaxValue, eRef.layerMask)) {
            Debug.DrawLine(eRef.shootPoint.position, eRef.shootPoint.position + direction * 10f, Color.red, 1f);

            TrailRenderer trail = MonoBehaviour.Instantiate(bulletTrail, eRef.gunPoint.position, Quaternion.identity);
            eRef.brain.StartCoroutine(SpawnTrail(trail, hit));

            eRef.brain.fireAmmo();

            if(hit.collider.gameObject.CompareTag("Player")) {
                hit.collider.gameObject.GetComponent<Player>().TakeDamage(20);
                //Debug.Log("Shot Player");
            }
        }
        
    }

    private Vector3 GetDirection() {
        Vector3 direction = eRef.e.transform.forward;
        
        direction += new Vector3(
            Random.Range(-spread.x, spread.x),
            Random.Range(-spread.y, spread.y),
            Random.Range(-spread.z, spread.z)
        );

        direction.Normalize();

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit) {
        float time = 0f;
        Vector3 startPosition = trail.transform.position;

        while (time < 0.5f) {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime/trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        MonoBehaviour.Destroy(trail.gameObject, trail.time);
    }
}
