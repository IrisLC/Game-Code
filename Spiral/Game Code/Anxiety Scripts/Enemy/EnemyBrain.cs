using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : IStateMachineUser
{
    /// <summary>
    /// The state that this AI will start in.
    /// </summary>
    IState InitialState;
    /// <summary>
    /// The state machine for this enemy.
    /// </summary>
    StateMachine stateMachine;

    /// <summary>
    /// The body that this brain is attached to.
    /// </summary>
    public IControllable Body;
    /// <summary>
    /// The gameObject for the player's body.
    /// </summary>
    public GameObject Player { get; private set; }

    /// <summary>
    /// A shortcut for the transform of the body the brain is attached to.
    /// </summary>
    Transform transform { get => Body.GetTransform(); }

    //TODO: Change floats to scriptable objects.

    /// <summary>
    /// How far away the AI can see other entities.
    /// </summary>
    float VisionDistance = 30f;
    /// <summary>
    /// The angle of the vision cone for this AI. Measured in degrees.
    /// </summary>
    float VisionAngle = 90f;
    /// <summary>
    /// Whether or not the AI sees the player.
    /// </summary>
    public bool SeesPlayer { get; private set; }
    /// <summary>
    /// Whether or not the AI is still interested enough in the player to chase them after losing sight.
    /// </summary>
    public bool IsInterestedInPlayer { get; private set; }
    /// <summary>
    /// The timer that marks how long the AI will continue to chase after the player before giving up after losing line of sight.
    /// </summary>
    CountdownTimer InterestTimer;
    /// <summary>
    /// How long (in seconds) the InterestTimer is.
    /// </summary>
    float InterestTimerLength = 5f;

    /// <summary>
    /// The timer that marks the cooldown for the entity's attack.
    /// </summary>
    CountdownTimer AttackTimer;
    /// <summary>
    /// How long the cooldown of the entity's attack is.
    /// </summary>
    float TimeBetweenAttacks = 1.75f;
    /// <summary>
    /// How close the entity must be to the target before it can attack.
    /// </summary>
    float AttackDistance = 2f;
    /// <summary>
    /// Whether or not the entity's attack is off cooldown
    /// </summary>
    bool CanAttack = true;

#if UNITY_EDITOR
    //Debug variables
    internal static bool ShowEnemyVisionCones;
#endif

    /// <summary>
    /// Constructor, creates the state machine and timers
    /// </summary>
    /// <param name="Parent">The Body this brain is attached to</param>
    /// <param name="initial">the state this enemy will start in</param>
    /// <param name="transitions">the list of transitions around which to build the state machine</param>
    public EnemyBrain(IControllable Parent, IState initial, List<StateTransition> transitions)
    {
        Body = Parent;
        InitialState = initial;
        stateMachine = new StateMachine(this, transitions);

        if (Referencer.AnxietyReferences.PlayerGameObject)
        {
            Player = Referencer.AnxietyReferences.PlayerGameObject;
        }
        // Backup incase the Referencer value was not set
        else
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        // Setup the timers that dictate the AI's actions
        InterestTimer = new CountdownTimer(InterestTimerLength);
        InterestTimer.OnTimerStop += () => IsInterestedInPlayer = false;

        AttackTimer = new CountdownTimer(TimeBetweenAttacks);
        AttackTimer.OnTimerStop += () => CanAttack = true;
    }

    public void Tick()
    {
        Vision();

        TimerTicks();

        stateMachine.Tick();

        AttackCheck();
    }

    /// <summary>
    /// Updates the timers
    /// </summary>
    void TimerTicks()
    {
        if (IsInterestedInPlayer) InterestTimer.Tick(Time.deltaTime);

        if (!CanAttack) AttackTimer.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Looks for the player
    /// </summary>
    public void Vision()
    {
        SeesPlayer = false;

#if UNITY_EDITOR
        // Draws lines showing the enemy's vision cones
        if (ShowEnemyVisionCones)
        {
            Color VisionColor = SeesPlayer ? Color.green : Color.red;
            Debug.DrawRay(transform.position, transform.forward * VisionDistance, VisionColor);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(VisionAngle / 2, Vector3.up) * (transform.forward * VisionDistance), VisionColor);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(VisionAngle / -2, Vector3.up) * (transform.forward * VisionDistance), VisionColor);
        }
#endif

        //If the player does not exist then don't run calculations
        if (!Player) return;

        // Gets the enemy and player's positions
        Vector3 pos = transform.position;
        Vector3 pPos = Player.transform.position;
        // Calculate the distance between the two and check if the player is in range
        float distanceToP = Vector3.Distance(pos, pPos);
        if (distanceToP > VisionDistance) return;

        // Gets the angle between the direction the enemy is facing, and the direction to the player
        float angle = Vector3.Angle(transform.forward, Vector3.Normalize(pPos - pos));

        // We are expecting the enemy vision to be input as the full vision cone,
        //  but for calculations we need to check one half of that value. (\/ vs \|)
        float visionHalfAngle = VisionAngle / 2;
        if (angle <= visionHalfAngle)
        {
            if (CheckVisionRays(pos, pPos, VisionDistance, Player))
            {
                // We have seen the player if we get here
#if UNITY_EDITOR
                if (EnemyBody.isDebugging) Debug.Log($"EnemyBrain: {GetParentTransform().gameObject} Sees Player");
#endif

                SeesPlayer = true;
                IsInterestedInPlayer = true;

                InterestTimer.Reset();
                InterestTimer.Start();
            }
        }
    }

    /// <summary>
    /// Sees if the player is close enough for the enemy to attack
    /// </summary>
    void AttackCheck()
    {
        if (CanAttack && Vector3.Distance(transform.position, Player.transform.position) < AttackDistance)
        {
            Body.Attack();
            CanAttack = false;
            AttackTimer.Reset();
            AttackTimer.Start();
        }
    }

    // Basic Getter Methods

    public IState GetInitialState() => InitialState;
    public StateMachine GetStateMachine() => stateMachine;
    public Transform GetParentTransform() => transform;

    // Static Methods

    /// <summary>
    /// Passes along a move call to the body
    /// </summary>
    /// <param name="target">The target to move to</param>
    public static void MoveBody(EnemyBrain Brain, Vector3 target)
    {
        Brain.Body.Move(target);
    }

    /// <summary>
    /// Casts rays to the player and sees if they hit.
    /// 
    /// TODO: Add multiple rays
    /// </summary>
    /// <param name="position">the enemy's position</param>
    /// <param name="playerPosition">the player's position</param>
    /// <returns>true iff the enemy has line of sight of the player</returns>
    static bool CheckVisionRays(Vector3 position, Vector3 playerPosition, float visionDistance, GameObject target)
    {

        if (Physics.Raycast(position, playerPosition - position, out RaycastHit hit, visionDistance))
        {
            if (hit.transform.gameObject == target) return true;
        }

        return false;
    }
}
