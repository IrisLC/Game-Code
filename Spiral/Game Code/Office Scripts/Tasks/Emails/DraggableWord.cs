using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class DraggableWord : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    /// The background image
    /// </summary>
    Image image;
    /// <summary>
    /// The rectTransform component of the word
    /// </summary>
    RectTransform rect;
    /// <summary>
    /// The starting position of the word, storing the rect.anchoredPosition
    /// </summary>
    public Vector3 startPos;
    /// <summary>
    /// Whether or not the word is slotted into a WordDropBox
    /// </summary>
    public bool isInSlot;
    /// <summary>
    /// The WordDropBox that this word is slotted into
    /// </summary>
    public WordDropBox activeSlot;
    /// <summary>
    /// The RectTransform of the activeSlot
    /// </summary>
    public RectTransform slotTransform;
    /// <summary>
    /// The center position of the activeSlot
    /// </summary>
    Vector3 slotPos { get => slotTransform.TransformPoint(slotTransform.rect.center); }

    /// <summary>
    /// The text of the email that this DraggableWord is a part of
    /// </summary>
    public TextMeshProUGUI EmailText;

    /// <summary>
    /// The GridLayoutGroup of the parent word bank
    /// </summary>
    GridLayoutGroup grid;

    /// <summary>
    /// The wordType that this word represents
    /// </summary>
    public EmailLogic.WordTypes WordType;
    /// <summary>
    /// If this word is the correct word of the corresponding WordType
    /// </summary>
    public bool IsCorrect;

    void Awake()
    {

        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        grid = transform.parent.GetComponent<GridLayoutGroup>();

    }

    void OnEnable()
    {
        EmailText?.ForceMeshUpdate(false, true);
        if (grid != null && EmailText != null)
        {
            grid.cellSize = grid.cellSize.With(y: EmailText.fontSize + 6);
            StartCoroutine(SetStartPos());
        }
    }

    /// <summary>
    /// Sets the startPos of this word, delayed a frame to account for delays in ForceMeshUpdates
    /// </summary>
    /// <returns></returns>
    IEnumerator SetStartPos()
    {
        yield return null;
        startPos = rect.anchoredPosition;
        if (isInSlot) rect.position = slotPos;
    }

    /// <summary>
    /// Called when the word is dragged with a mouse
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 point = eventData.position;
        transform.position = new Vector3(point.x, point.y, 0);
    }

    /// <summary>
    /// picks up the word from its current location
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // So the word doesn't get in the way of checking for dropping on a slot
        image.raycastTarget = false;

        isInSlot = false;
        if (activeSlot == null) return;
        activeSlot.hasBlock = false;
        activeSlot.Word = null;
        activeSlot = null;
    }

    /// <summary>
    /// When the word is dropped, sets its new position.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {

        if (isInSlot)
        {
            rect.position = slotPos;
        }
        else
        {
            rect.anchoredPosition = startPos;
        }

        image.raycastTarget = true;
    }

}

