using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

/// <summary>
/// The Monobehaviour for the basic enemy type
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBody : MonoBehaviour, IControllable
{
    /// <summary>
    /// The EnemyBrain component that controls this body
    /// </summary>
    EnemyBrain Controller;
    /// <summary>
    /// The animation controller for this body
    /// </summary>
    Animator AnimationController;

    /// <summary>
    /// The list of StateTransitions that this Enemy can do
    /// </summary>
    [SerializeField] List<StateTransition> Transitions;

    /// <summary>
    /// How fast the body can move
    /// </summary>
    [Header("Navigation")]
    [SerializeField] float Speed;
    /// <summary>
    /// The NavMeshAgent on this body
    /// </summary>
    public NavMeshAgent Nav { get; private set; }

    /// <summary>
    /// The state the enemy will start in
    /// </summary>
    [SerializeReference] IState InitialState;

    /// <summary>
    /// The player in the maze
    /// </summary>
    PlayerMain Player;

#if UNITY_EDITOR
    /// <summary>
    /// If Debug.Log commands should be called, modified with DebugCommands
    /// </summary>
    internal static bool isDebugging;
#endif

    void Awake()
    {
        // Makes sure initialState has been set
        Assert.IsNotNull(InitialState);

        Nav = gameObject.GetComponent<NavMeshAgent>();
        AnimationController = GetComponent<Animator>();

        // Create the controller
        Controller = new EnemyBrain(this, InitialState, Transitions);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = Referencer.AnxietyReferences.PlayerScript;
    }

    // Update is called once per frame
    void Update()
    {
        Controller?.Tick();
    }

    public void Move(Vector3 target)
    {
        Nav.SetDestination(target);
    }

    public Transform GetTransform() => transform;

    public void Attack()
    {
#if UNITY_EDITOR
        if (isDebugging) Debug.Log(AnimationController.GetBool("isAttacking?"));
#endif
        AnimationController.SetBool("isAttacking?", true);
    }

    /// <summary>
    /// Damages the player and turns off attack animation
    /// </summary>
    public void DamagePlayer()
    {
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("Attack");
#endif
        Player.damagePlayer();
        AnimationController.SetBool("isAttacking?", false);
    }
}
