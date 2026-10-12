using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComputerWindowHeader : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    /// The background image
    /// </summary>
    Image image;
    public GameObject Parent;
    public ComputerWindow parentWindow;
    RectTransform rect;
    Vector2 StartPosition;

    void Awake()
    {
        image = GetComponent<Image>();
        parentWindow = Parent.GetComponent<ComputerWindow>();
        rect = Parent.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Called when the window is dragged with a mouse
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform,
            eventData.position, Camera.main, out Vector2 mousePosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform,
            eventData.pressPosition, Camera.main, out Vector2 pressPosition);

        Vector2 MouseDelta = mousePosition - pressPosition;

        rect.localPosition = StartPosition + MouseDelta;
    }

    /// <summary>
    /// picks up the window from its current location
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentWindow.isFullscreen = false;
        image.raycastTarget = false;
        StartPosition = rect.localPosition;
    }

    /// <summary>
    /// When the word is dropped, sets its new position.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {

        image.raycastTarget = true;
    }
}
