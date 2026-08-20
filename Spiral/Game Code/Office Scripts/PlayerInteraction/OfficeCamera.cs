using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OfficeCamera : MonoBehaviour
{

    InputAction InteractAction;
    Camera mCamera;

    public enum CameraDirection { Left, Middle, Right }
    CameraDirection currDirection = CameraDirection.Middle;
    bool onComputer = false; // if player is on the computer turn true
    /// <summary>
    /// Whether the player was hovering over an IClickable gameObject in the office
    /// </summary>
    bool IsHoveringOffice;
    /// <summary>
    /// Whether the player was hovering over an IClickable UI object on the computer
    /// </summary>
    bool IsHoveringComputer;

    /// <summary>
    /// The speed at which the camera can rotate on the Y axis
    /// </summary>
    [SerializeField] float CameraRotationSpeed;
    /// <summary>
    /// The rotational limits of the camera on the Y axis, min and max.
    /// </summary>
    [SerializeField] Vector2 CameraBounds;

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


    void Awake()
    {
        mCamera = Camera.main;
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
        if (InteractAction == null) InteractAction = GameManager.OfficeInputs.Interact;
        InteractAction.performed += Interact;

        ComputerScript.ComputerTurnedOff += CloseComputer;
    }

    void OnDisable()
    {
        InteractAction.performed -= Interact;

        ComputerScript.ComputerTurnedOff -= CloseComputer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!onComputer) // only move camera if player is not on computer
        {
            PanCamera();
            CheckHoverOffice();
        }
        else
        {
            CheckHoverComputer();
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

        if (Physics.Raycast(ray, out RaycastHit rayHit))
        {

            switch (rayHit.collider.gameObject.tag)
            {
                case "Computer":
                    if (!onComputer) OpenComputer();
                    break;

                case "Paper":
                    PickupPaper(rayHit.collider.gameObject);
                    break;

                case "PaperBin":
                    rayHit.collider.gameObject.GetComponent<Bins>().Use();
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
        onComputer = true;
        helperText.SetActive(false);

        foreach (GameObject go in Referencer.OfficeReferences.DisappearOnComputerOpen)
        {
            go.SetActive(false);
        }

        Referencer.OfficeReferences.Computer.OpenComputer();

    }

    /// <summary>
    /// Turns on any objects set to turn off when the computer opens
    /// </summary>
    void CloseComputer()
    {
        onComputer = false;
        foreach (GameObject go in Referencer.OfficeReferences.DisappearOnComputerOpen)
        {
            go.SetActive(true);
        }
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
                return;
        }

        float newRotation = mCamera.transform.rotation.eulerAngles.y + rotationModifier;
        // This gets around the way that Euler angles are converted too and from Quaternions 
        if (newRotation > 180) newRotation -= 360;
        //Debug.Log(newRotation);

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
}
