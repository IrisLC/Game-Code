
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The main script for the Player in the Anxiety Maze
/// </summary>
[RequireComponent(typeof(PlayerReferences))]
[RequireComponent(typeof(CharacterController))]
public class PlayerMain : MonoBehaviour
{
    /// <summary>
    /// Movement speed of player
    /// </summary>
    [SerializeField] float speed;
    /// <summary>
    /// The script holding references to other objects on the player
    /// </summary>
    PlayerReferences pRef;
    /// <summary>
    /// The character controller component
    /// </summary>
    CharacterController Controller;

    /// <summary>
    /// The oxygen bar image
    /// </summary>
    [Header("Oxygen")]
    [SerializeField] Image OxygenBar;
    /// <summary>
    /// The player's starting oxygen
    /// </summary>
    public const float StartingOxygen = 100;
    /// <summary>
    /// The current oxygen level of the player
    /// </summary>
    [SerializeField] float CurrentOxygen;
    /// <summary>
    /// The timer for how long until the player runs out of oxygen
    /// </summary>
    CountdownTimer OxygenTimer;

    /// <summary>
    /// The speed of the oxygen timer, for use with Dev commands
    /// </summary>
    internal float TimerSpeed { get => OxygenTimer.Speed; set => OxygenTimer.Speed = value; }
    /// <summary>
    /// The initial size of the oxygen texture
    /// </summary>
    float OxygenBarInitialScale;
    /// <summary>
    /// Whether or not the oxygen level should not tick
    /// </summary>
    bool OxygenPaused;
    /// <summary>
    /// How much oxygen is lost when hit
    /// </summary>
    [SerializeField] float DamagePenalty = 10;
    /// <summary>
    /// The color gradient for the oxygen bar as it depletes
    /// </summary>
    public Gradient gradient;

    /// <summary>
    /// How close the player must be to an interactable object to use it
    /// </summary>
    [Header("Interactions")]
    [SerializeField] float InteractionCheckDistance = 10f;

    /// <summary>
    /// Fires an event telling the MazeManager to finish the level
    /// </summary>
    Action PlayerDies = () => MazeManager.FinishLevel(false);

    void Awake()
    {
        pRef = GetComponent<PlayerReferences>();
        Controller = gameObject.GetOrAdd<CharacterController>();

        OxygenTimer = new CountdownTimer(StartingOxygen);

        OxygenBarInitialScale = OxygenBar.rectTransform.localScale.x;

        OxygenBar.color = gradient.Evaluate(1);
    }

    void OnEnable()
    {
        OxygenTimer.OnTimerStop += PlayerDies;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pRef.PlayerInputs.InteractPressed += Interact;

        OxygenTimer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        OxygenTimerTick();
        if (pRef.PlayerInputs.MovePerformed)
        {
            Move(pRef.PlayerInputs.MoveDirection);
        }
    }

    void Move(Vector2 MovementVector)
    {
        Controller.Move(MovementVector.AsForwardV3(transform) * speed * Time.deltaTime);
    }

    /// <summary>
    /// Ticks the oxygen timer and updates the OxygenBar UI 
    /// </summary>
    void OxygenTimerTick()
    {
        if (!OxygenPaused) OxygenTimer.Tick(Time.deltaTime);

        CurrentOxygen = OxygenTimer.Time;

        OxygenBar.rectTransform.localScale =
             OxygenBar.rectTransform.localScale.With
                (x: OxygenTimer.Time.Remap(0, StartingOxygen, 0, OxygenBarInitialScale));

        OxygenBar.color = gradient.Evaluate(CurrentOxygen / StartingOxygen);
    }

    /// <summary>
    /// Adds oxygen to the player, maxes out at the starting oxygen value
    /// </summary>
    /// <param name="AmountToAdd">the amount to increase the player's current Oxygen level by</param>
    public void AddOxygen(float AmountToAdd)
    {
        CurrentOxygen = Mathf.Clamp(CurrentOxygen + AmountToAdd, 0, StartingOxygen);

        OxygenTimer.Reset(CurrentOxygen);
    }

    public void PauseOxygen() => OxygenPaused = true;
    public void UnPauseOxygen() => OxygenPaused = false;

    /// <summary>
    /// Interacts with an item
    /// </summary>
    void Interact()
    {
        if (InteractionCheck(out Interactable interactable))
        {
            interactable.Use();
        }
    }

    /// <summary>
    /// Checks to see if the player can interact with an item
    /// </summary>
    /// <param name="OutedInteractable">The interactable that can be interacted with. Null if none found</param>
    /// <returns>true if the player can interact with an item</returns>
    bool InteractionCheck(out Interactable OutedInteractable)
    {
        Transform CameraTransform = pRef.PlayerCamera.transform;
        if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out RaycastHit hit, InteractionCheckDistance))
        {
            if (hit.transform.gameObject.TryGetComponent(out Interactable interactable))
            {
                if (interactable.type == Interactable.InteractionType.LookAt)
                {
                    OutedInteractable = interactable;
                    return true;
                }
            }
        }
        OutedInteractable = null;
        return false;
    }

    public void damagePlayer()
    {
        OxygenTimer.Reset(CurrentOxygen -= DamagePenalty);
    }

    void OnDisable()
    {
        pRef.PlayerInputs.InteractPressed -= Interact;

        OxygenTimer.OnTimerStop -= PlayerDies;
    }
}
