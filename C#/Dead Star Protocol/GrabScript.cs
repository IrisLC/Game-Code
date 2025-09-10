using UnityEngine;
using UnityEngine.InputSystem;

public class GrabScript : MonoBehaviour
{
    private InputSystem_Actions.PlayerActions playerActions;
    private InputSystem_Actions playerInput;


    [SerializeField] private Transform holdPos;
    [SerializeField] private float grabRange = 2f;
    private GameObject heldObj = null;
    // [SerializeField] private Player player;
    private Color originalColor;
    [SerializeField] private LayerMask environmentLayer;

    private InputAction interactActions;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = new InputSystem_Actions();

        playerActions = playerInput.Player;
        interactActions = playerActions.Interact;
        interactActions.Enable();

        interactActions.started += pickup;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameTimeManager.GetIsGamePaused())
        {
            return;
        }

        // If the player is holding an object...
        if (heldObj != null)
        {
            // Held object follows player
            heldObj.transform.position = holdPos.transform.position;
            heldObj.transform.rotation = gameObject.transform.rotation;

            // Turn object red if it cannot be released
            if (Physics.CheckBox(heldObj.transform.position, heldObj.GetComponent<Collider>().bounds.extents, Quaternion.identity, environmentLayer))
            {
                heldObj.GetComponent<MeshRenderer>().material.color = new Color(100, 0, 0);
            } else
            {
                heldObj.GetComponent<MeshRenderer>().material.color = originalColor;
            }
        }


    }


    void pickup(InputAction.CallbackContext context) {

        Debug.Log("press");

        // If the player is not holding an object
        if (heldObj == null)
        {
            RaycastHit hit;
            // If an object was within grab range...
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, grabRange))
            {


                // If the object can be held...
                if (hit.transform.gameObject.tag == "canHold")
                {
                    // Hold the object
                    heldObj = hit.transform.gameObject;
                    heldObj.GetComponent<Rigidbody>().useGravity = false;
                    heldObj.GetComponent<Rigidbody>().isKinematic = true;
                    originalColor = heldObj.GetComponent<MeshRenderer>().material.color;
                    heldObj.layer = 6;
                    // // Disable the player's weapon while holding the object
                    // if (player.activeItem != null)
                    // {
                    //     player.activeItem.SetActive(false);
                    // }
                }
            }
            else
            {
                Debug.Log("Object cannot be released inside another object.");
            }
        }
        else
        {
            // If there is nothing in the way of the held object...
            if (!Physics.CheckBox(heldObj.transform.position, heldObj.GetComponent<Collider>().bounds.extents, Quaternion.identity, environmentLayer))
            {
                // Release the object
                heldObj.GetComponent<Rigidbody>().useGravity = true;
                heldObj.GetComponent<Rigidbody>().isKinematic = false;
                heldObj.layer = 0;
                heldObj = null;
                // // Re-activate the player's equipped item
                // if (player.activeItem != null)
                // {
                //     player.activeItem.SetActive(true);
                // }
            }
            else
            {
                Debug.Log("Object cannot be released inside another object.");
            }
        }
    }

}

