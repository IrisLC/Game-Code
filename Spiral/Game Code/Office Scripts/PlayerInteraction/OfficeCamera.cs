using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OfficeCamera : MonoBehaviour
{

    InputAction InteractAction;
    public Camera mCamera { get; private set; }

    public enum CameraDirection { Left, Middle, Right }
    CameraDirection currDirection = CameraDirection.Middle;
    bool ZoomComputer = false; // if player is on the computer turn true
    bool ComputerOn { get => Referencer.OfficeReferences.Computer.IsComputerOn; }
    /// <summary>
    /// Whether the player was hovering over an IClickable gameObject in the office
    /// </summary>
    bool IsHoveringOffice;
    /// <summary>
    /// Whether the player was hovering over an IClickable UI object on the computer
    /// </summary>
    bool IsHoveringComputer;

    [Header("Coworker rotation data")]
    /// <summary>
    /// How much slower the passive camera rotation should be as compared to the normal pan speed. (divides CameraRotationSpeed by this)
    /// </summary>
    [SerializeField] float coworkerPassiveCameraSpeedModifier = 3;
    /// <summary>
    /// How many seconds it should take for the camera to snap to the coworker when they spawn in
    /// </summary>
    [SerializeField] float initialCoworkerSpawnCameraRotationTime = 1;
    /// <summary>
    /// The coworker currently in the scene
    /// </summary>
    GameObject coworker;
    /// <summary>
    /// The rotation of the camera at the point the coworker is spawned
    /// </summary>
    Quaternion initialLookDirectionAtCoworkerSpawn;
    /// <summary>
    /// The rotation the player should have at the end of the coworker spawn snap
    /// </summary>
    Quaternion coworkerRotationSnapTarget;
    /// <summary>
    /// The Time.time at the moment the coworker is spawned
    /// </summary>
    float spawnTime;
    /// <summary>
    /// True iff we should be actively rotating to the coworker at the moment they spawn
    /// </summary>
    bool isRotatingToCoworker;

    /// <summary>
    /// The speed at which the camera can rotate on the Y axis
    /// </summary>
    [SerializeField] float CameraRotationSpeed;
    /// <summary>
    /// The rotational limits of the camera on the Y axis, min and max.
    /// </summary>
    [SerializeField] Vector2 CameraBounds;
    [SerializeField] Vector2 ZoomComputerBounds;

    /// <summary>
    /// The spot at which a picked up piece of paper will be held
    /// </summary>
    [SerializeField] Transform PaperHoldPosition;
    /// <summary>
    /// Tutorial text for turn on the computer
    /// </summary>
    [SerializeField] GameObject helperText;

    /// <summary>
    /// The paper object being held
    /// </summary>
    public Paper HeldPaper;

    /// <summary>
    /// If this is the first time for the player in the game
    /// </summary>
    public bool IsFirstTime = false;

    /// <summary>
    /// Events called when the player hovers over or hovers off an IClickable object
    /// </summary>
    public delegate void HoverToggle();
    public static event HoverToggle HoverOn;
    public static event HoverToggle HoverOff;

    /// <summary>
    /// The current EventSystem in the office
    /// </summary>
    EventSystem eventSystem;
    /// <summary>
    /// The GraphicRaycaster component on the computerCanvas
    /// </summary>
    GraphicRaycaster UIRaycaster;

    [SerializeField] float zoomSpeed = .5f;

    internal static bool isDebugging;
    const float OfficeFOV = 60f;
    const float ComputerFOV = 25f;
    Coroutine coroutine;

    [SerializeField] private AudioSource clickAudioSource;
    [SerializeField] private AudioClip computerClickSound;
    [SerializeField, Range(0f, 1f)] private float clickVolume = 1f;


    void Awake()
    {
        mCamera = gameObject.GetComponent<Camera>();
        eventSystem = EventSystem.current;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Referencer.OfficeReferences.ComputerUI.gameObject.SetActive(false);
        UIRaycaster = Referencer.OfficeReferences.ComputerUI.GetComponent<GraphicRaycaster>();
    }

    void OnEnable()
    {
        if (InteractAction == null)
        {
            if (!GameManager.PlayerInputInitialized) GameManager.SetupInputs();
            InteractAction = GameManager.OfficeInputs.Interact;
        }
        InteractAction.performed += Interact;
        InteractAction.Enable();

        ComputerScript.ComputerTurnedOff += UnZoomComputer;

        Coworker.CoworkerSpawn += OnCoworkerSpawned;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDisable()
    {
        InteractAction.performed -= Interact;

        ComputerScript.ComputerTurnedOff -= UnZoomComputer;

        Coworker.CoworkerSpawn -= OnCoworkerSpawned;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRotatingToCoworker)
        {
            PanCamera();
            if (ZoomComputer) // only move camera if player is not on computer
            {
                CheckHoverComputer();
            }

            CheckHoverOffice();
        }
        else
        {
            LookAtCoworker();
        }

    }

    /// <summary>
    /// Checks to see if the mouse is over an object that can be interacted with, 
    ///  and calls relevent methods based on what the object is.
    /// </summary>
    /// <param name="context"></param>
    void Interact(InputAction.CallbackContext context)
    {

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mCamera.ScreenPointToRay(mousePos);

        if (isDebugging)
        {
            PointerEventData ptr = new(eventSystem);
            ptr.position = Mouse.current.position.ReadValue();
            List<RaycastResult> results = new();
            UIRaycaster.Raycast(ptr, results);
            if (results.Count > 0)
            {
                Debug.Log("Hit object " + results[0].gameObject + " belonging to " + results[0].gameObject.transform.parent.gameObject);
            }
        }

        if (ZoomComputer) {
            if (clickAudioSource != null && computerClickSound != null)
            {
                clickAudioSource.PlayOneShot(computerClickSound, clickVolume);
            }

            return;
        }
        ;

        if (Physics.Raycast(ray, out RaycastHit rayHit))
        {

            switch (rayHit.collider.gameObject.tag)
            {
                case "Computer":
                    OpenComputer();
                    break;

                case "Paper":
                    PickupPaper(rayHit.collider.gameObject);
                    break;

                case "PaperBin":
                    rayHit.collider.gameObject.GetComponent<Bins>().Use();
                    break;
                default:
                    if (rayHit.collider.gameObject.TryGetComponent<Interactable>(out Interactable interactable))
                    {
                        interactable.Use();
                    }
                    break;
            }


        }
    }

    /// <summary>
    /// Checks to see if the player's mouse is hovering over an IClickable gameObject
    /// </summary>
    void CheckHoverOffice()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mCamera.ScreenPointToRay(mousePos);

        OnHover(Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.GetComponent<IClickable>() != null, ref IsHoveringOffice);
    }

    /// <summary>
    /// Checks to see if the player's mouse is hovering over an IClickable UI object
    /// </summary>
    void CheckHoverComputer()
    {
        PointerEventData pointer = new PointerEventData(eventSystem);
        pointer.position = GameManager.OfficeInputs.Look.ReadValue<Vector2>();
        List<RaycastResult> results = new List<RaycastResult>();

        UIRaycaster.Raycast(pointer, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent<IClickable>(out IClickable component))
            {
                OnHover(true, ref IsHoveringComputer);
                return;
            }
        }

        OnHover(false, ref IsHoveringComputer);
    }

    /// <summary>
    /// Invokes either the HoverOn or HoverOff events if the hovering state has changed since last frame
    /// </summary>
    /// <param name="IsOnClickable">Whether or not the player's mouse is over an IClickable object</param>
    /// <param name="Hovering">a reference to the relevant IsHovering variable</param>
    void OnHover(bool IsOnClickable, ref bool Hovering)
    {
        if (IsOnClickable)
        {
            if (!Hovering)
            {
                HoverOn?.Invoke();
                Hovering = true;
            }
        }
        else
        {
            if (Hovering)
            {
                HoverOff?.Invoke();
                Hovering = false;
            }
        }
    }

    /// <summary>
    /// Turns off any objects set to turn off when the computer opens, and tells the ComputerScript to turn on
    /// </summary>
    void OpenComputer()
    {
        if (!ZoomComputer && coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        ZoomComputer = true;

        coroutine = StartCoroutine(ChangeFOV(mCamera.fieldOfView, ComputerFOV));

        if (ComputerOn) return;

        helperText.SetActive(false);

        Referencer.OfficeReferences.Computer.OpenComputer();

    }

    /// <summary>
    /// Turns on any objects set to turn off when the computer opens
    /// </summary>
    void UnZoomComputer()
    {
        if (ZoomComputer && coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        ZoomComputer = false;
        coroutine = StartCoroutine(ChangeFOV(mCamera.fieldOfView, OfficeFOV));
    }

    /// <summary>
    /// Moves the camera left or right, depending on which side of the screen the player's cursor is on
    /// </summary>
    void PanCamera()
    {
        // get the right direction and speed
        float rotationModifier = 0;
        switch (currDirection)
        {
            case CameraDirection.Left:
                rotationModifier = -CameraRotationSpeed * Time.deltaTime;
                break;
            case CameraDirection.Right:
                rotationModifier = CameraRotationSpeed * Time.deltaTime;
                break;
            case CameraDirection.Middle:
                // if there is a coworker active then do the subtle pan
                if (Referencer.OfficeReferences.coworkerManager.isCoworkerActive)
                {
                    rotationModifier = -CameraRotationSpeed / coworkerPassiveCameraSpeedModifier * Time.deltaTime;
                    break;
                }
                return;
        }

        float newRotation = mCamera.transform.rotation.eulerAngles.y + rotationModifier;
        // This gets around the way that Euler angles are converted too and from Quaternions 
        if (newRotation > 180) newRotation -= 360;

        if (ZoomComputer && (newRotation < ZoomComputerBounds.x || newRotation > ZoomComputerBounds.y))
        {
            UnZoomComputer();
        }
        // Don't excede the given rotation limits
        newRotation = Mathf.Clamp(newRotation, CameraBounds.x, CameraBounds.y);

        // Assign new rotation
        mCamera.transform.rotation = Quaternion.Euler(mCamera.transform.rotation.eulerAngles.With(y: newRotation));
    }

    /// <summary>
    /// Sets the current direction to either left or right.
    /// 
    /// Called by the MouseLeft and MouseRight UI objects when hovered over by the mouse
    /// </summary>
    /// <param name="bMoveRight">whether or not the current direction should be right</param>
    public void StartCameraMove(bool bMoveRight)
    {
        if (bMoveRight)
        {
            currDirection = CameraDirection.Right;
        }
        else
        {
            currDirection = CameraDirection.Left;
        }
    }

    /// <summary>
    /// Sets the current direction to Middle.
    /// 
    /// Called by the MouseLeft and MouseRight UI objects when no longer hovered over by the mouse.
    /// </summary>
    public void EndCameraMove()
    {
        currDirection = CameraDirection.Middle;
    }

    /// <summary>
    /// Grabs a paper gameObject
    /// </summary>
    /// <param name="paper">the gaeObject representing the page being grabbed</param>
    void PickupPaper(GameObject paper)
    {
        if (HeldPaper == null)
        {
            HeldPaper = paper.GetComponent<Paper>();
            paper.transform.SetParent(PaperHoldPosition.transform);

            paper.transform.localPosition = Vector3.zero;
            paper.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    /// <summary>
    /// Method called when a coworker is added, sets the necessary variables for auto panning
    /// </summary>
    /// <param name="worker">the gameObject for the coworker that was just spawned</param>
    void OnCoworkerSpawned(GameObject worker)
    {
        coworker = worker;
        isRotatingToCoworker = true;

        initialLookDirectionAtCoworkerSpawn = mCamera.transform.rotation;
        coworkerRotationSnapTarget = Quaternion.LookRotation(coworker.transform.position.With(y: mCamera.transform.position.y)
            - mCamera.transform.position, mCamera.transform.up);
        spawnTime = Time.time;

    }

    /// <summary>
    /// Method handling the snapping of the camera to a newly spawned coworker
    /// </summary>
    void LookAtCoworker()
    {
        // Percentage through the rotation
        float t = (Time.time - spawnTime) / initialCoworkerSpawnCameraRotationTime;

        mCamera.transform.rotation = Quaternion.Slerp(initialLookDirectionAtCoworkerSpawn, coworkerRotationSnapTarget, t);

        // if we've completed the rotation then stop the snap
        if (t >= 1)
        {
            isRotatingToCoworker = false;
        }
    }

    IEnumerator ChangeFOV(float start, float end)
    {
        float time = 0;

        while (mCamera.fieldOfView != end)
        {
            mCamera.fieldOfView = Mathf.Lerp(start, end, time);
            yield return null;
            time = Mathf.Clamp(time + Time.deltaTime / zoomSpeed, 0, 1);
        }
        coroutine = null;
    }
}
