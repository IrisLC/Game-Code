using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyReference : MonoBehaviour
{
    /// <summary>
    /// The Player in the scene
    /// </summary>
    public Player p;





    // Internal Enemy Scripts
    public EnemyBrain brain;
    public EnemyVision vision;

    public NavMeshAgent nav;

    // Used for state machine states, the gameObject

    public GameObject e;

    // Locations for shooting calculations and effects

    public Transform shootPoint;
    public Transform gunPoint;
    public TrailRenderer bulletTrail;
    public LayerMask layerMask;
    public BaseAlert baseAlert;

    // Start is called before the first frame update
    void Awake()
    {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        baseAlert = GameObject.FindGameObjectWithTag("AlertSystem").GetComponent<BaseAlert>();
        brain = GetComponent<EnemyBrain>();
        vision = GetComponent<EnemyVision>();
        nav = GetComponent<NavMeshAgent>();
        e = gameObject;

        if(nav!= null)nav.areaMask = layerMask;
    }
}
