using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordDropBox : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// The WordType that this box expects
    /// </summary>
    public EmailLogic.WordTypes RequiredWordType;
    /// <summary>
    /// If this drop box has a DraggableWord in it
    /// </summary>
    public bool hasBlock;
    /// <summary>
    /// The DraggableWord in the box
    /// </summary>
    public DraggableWord Word;
    /// <summary>
    /// This box's rectTransform component
    /// </summary>
    RectTransform rectTransform;

    /// <summary>
    /// The body text for the email
    /// </summary>
    public TextMeshProUGUI TextContainer;
    /// <summary>
    /// The index of the '|' character that this box ties its location to. 
    ///  Index is in terms of the TextContainer.textInfo.characterInfo
    /// </summary>
    public int boxWordLastIndex;
    /// <summary>
    /// The RectTransform of the parent gameObject
    /// </summary>
    RectTransform parentRectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (TextContainer && TextContainer.textInfo.characterInfo.Length > 8)
        {
            Vector3 Position = (TextContainer.textInfo.characterInfo[boxWordLastIndex].topRight
                + TextContainer.textInfo.characterInfo[boxWordLastIndex].bottomRight) / 2;

            parentRectTransform.anchoredPosition = Position;
        }
    }

    void OnEnable()
    {
        TextContainer?.ForceMeshUpdate();
        if (rectTransform && TextContainer)
        {
            rectTransform.sizeDelta = rectTransform.sizeDelta.With(y: TextContainer.fontSize + 6);
        }
    }

    /// <summary>
    /// Called if a DraggabledWord that is being dragged is dropped on top of this box, sets the DraggableWord to be in this box
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        DraggableWord word = eventData.pointerDrag.GetComponent<DraggableWord>();

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

    /// <summary>
    /// Checks if the word being held in this box is the correct word.
    /// </summary>
    /// <returns>true iff the Word in the box is of the right type and is the correct word of that type</returns>
    public bool CheckWord()
    {
        // No word is auto incorrect
        if (Word == null) return false;

        if (Word.WordType == RequiredWordType && Word.IsCorrect)
        {
            return true;
        }

        return false;
    }
}
