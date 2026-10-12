using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public abstract class ADraggableWord : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
    /// The starting size of the word, how it is set when in the word bank
    /// </summary>
    public Vector2 startSize;
    public Transform startParent;
    /// <summary>
    /// Whether or not the word is slotted into a WordDropBox
    /// </summary>
    public bool isInSlot;
    /// <summary>
    /// The WordDropBox that this word is slotted into
    /// </summary>
    public AWordDropBox activeSlot;
    /// <summary>
    /// The RectTransform of the activeSlot
    /// </summary>
    public RectTransform slotTransform;
    /// <summary>
    /// The center position of the activeSlot
    /// </summary>
    Vector3 slotPos { get => slotTransform.TransformPoint(slotTransform.rect.center); }

    /// <summary>
    /// The GridLayoutGroup of the parent word bank
    /// </summary>
    GridLayoutGroup grid;

    /// <summary>
    /// The text that this DraggableWord is a part of, used for resizing
    /// </summary>
    public TextMeshProUGUI ParentText;

    TextMeshProUGUI Text;

    public static event System.Action<ADraggableWord> WordPlaced;
    public string Word { get => Text.text; }
    /// <summary>
    /// If this word is the correct word of the corresponding WordType
    /// </summary>
    public bool IsCorrect;

    void Awake()
    {

        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        grid = transform.parent.GetComponent<GridLayoutGroup>();
        Text = GetComponentInChildren<TextMeshProUGUI>();
        startParent = transform.parent;
    }

    protected virtual void OnEnable()
    {
        ParentText?.ForceMeshUpdate(false, true);
        if (grid != null && ParentText != null)
        {
            grid.cellSize = grid.cellSize.With(y: ParentText.fontSize + 8);
            StartCoroutine(SetStartVals());
        }
    }

    void Update()
    {
        if (isInSlot)
        {
            rect.position = slotPos;
        }
    }

    /// <summary>
    /// Sets the startPos of this word, delayed a frame to account for delays in ForceMeshUpdates
    /// </summary>
    /// <returns></returns>
    IEnumerator SetStartVals()
    {
        yield return null;
        startSize = rect.rect.size;
        if (isInSlot) rect.position = slotPos;
    }

    /// <summary>
    /// Called when the word is dragged with a mouse
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect.parent as RectTransform, eventData.position, Camera.main, out Vector2 point);
        rect.localPosition = new Vector3(point.x, point.y, 0);
    }

    /// <summary>
    /// picks up the word from its current location
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // So the word doesn't get in the way of checking for dropping on a slot
        image.raycastTarget = false;
        image.maskable = false;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().maskable = false;

        isInSlot = false;
        if (activeSlot == null) return;
        transform.SetParent(startParent, false);
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
            transform.SetParent(activeSlot.transform, false);
            rect.anchoredPosition = Vector2.zero;

            WordPlaced?.Invoke(this);
        }
        else
        {
            rect.anchoredPosition = Vector2.one * 0.5f;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startSize.x);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startSize.y);

        }

        LayoutRebuilder.MarkLayoutForRebuild(rect.parent as RectTransform);

        image.maskable = true;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().maskable = true;
        image.raycastTarget = true;
    }

}
