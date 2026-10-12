using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AWordDropBox : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// If this drop box has a DraggableWord in it
    /// </summary>
    public bool hasBlock;
    /// <summary>
    /// The DraggableWord in the box
    /// </summary>
    public ADraggableWord Word;
    /// <summary>
    /// This box's rectTransform component
    /// </summary>
    RectTransform rectTransform;

    /// <summary>
    /// The body text this box sits in
    /// </summary>
    public TextMeshProUGUI TextContainer;

    /// <summary>
    /// The RectTransform of the parent gameObject
    /// </summary>
    protected RectTransform parentRectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        TextContainer?.ForceMeshUpdate();
        if (rectTransform && TextContainer)
        {
            rectTransform.sizeDelta = rectTransform.sizeDelta.With(y: TextContainer.fontSize + 8);
        }
    }

    /// <summary>
    /// Called if a DraggabledWord that is being dragged is dropped on top of this box, sets the DraggableWord to be in this box
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        ADraggableWord word = eventData.pointerDrag.GetComponent<ADraggableWord>();

        if (word != null)
        {
            // Marks this as the slot for the word to be in
            word.activeSlot = this;
            word.slotTransform = rectTransform;
            word.isInSlot = true;

            // Marks the word as the word in this box
            hasBlock = true;
            Word = word;
        }
    }
}
