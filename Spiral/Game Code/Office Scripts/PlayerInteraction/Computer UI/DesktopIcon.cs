using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DesktopIcon : MonoBehaviour, IPointerClickHandler
{
    public WindowIconGroup iconGroup;
    [SerializeField] float doubleClickWindowLength;
    bool doubleClickWindowActive;
    Image background;

    void Awake()
    {
        iconGroup.desktopIcon = this;
        background = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (!GameManager.PlayerInputInitialized) GameManager.SetupInputs();
        GameManager.OfficeInputs.Interact.performed += TurnOffIconBackground;
    }

    void OnDisable()
    {
        GameManager.OfficeInputs.Interact.performed -= TurnOffIconBackground;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (doubleClickWindowActive)
        {
            Debug.Log("Clicked on icon");
            iconGroup.window.OpenWindow();
            //doubleClickWindowActive = false;
        }
        else
        {
            Debug.Log("Clicked on icon");
            doubleClickWindowActive = true;
            StartCoroutine(doubleClickWindow());
            //background.enabled = true;
        }
    }

    IEnumerator doubleClickWindow()
    {
        yield return new WaitForSeconds(doubleClickWindowLength);
        doubleClickWindowActive = false;
    }

    public void TurnOffIconBackground(InputAction.CallbackContext context)
    {
        //background.enabled = false;
    }
}
