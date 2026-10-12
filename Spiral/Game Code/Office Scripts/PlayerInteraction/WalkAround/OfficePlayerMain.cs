using UnityEngine;

/// <summary>
/// The main script for the Player in the Anxiety Maze
/// </summary>
[RequireComponent(typeof(OfficePlayerReferences))]
[RequireComponent(typeof(CharacterController))]
public class OfficePlayerMain : MonoBehaviour
{
    /// <summary>
    /// Movement speed of player
    /// </summary>
    [SerializeField] float speed;
    /// <summary>
    /// The script holding references to other objects on the player
    /// </summary>
    OfficePlayerReferences pRef;
    /// <summary>
    /// The character controller component
    /// </summary>
    CharacterController Controller;

    /// <summary>
    /// How close the player must be to an interactable object to use it
    /// </summary>
    [Header("Interactions")]
    [SerializeField] float InteractionCheckDistance = 10f;

    void Awake()
    {
        pRef = GetComponent<OfficePlayerReferences>();
        Controller = gameObject.GetOrAdd<CharacterController>();


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        pRef.PlayerInputs.InteractPressed += Interact;
    }

    // Update is called once per frame
    void Update()
    {
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
        Transform CameraTransform = pRef.OfficePlayerCamera.transform;
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

    void OnDisable()
    {
        pRef.PlayerInputs.InteractPressed -= Interact;
    }
}
