
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Block : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public UnityEngine.UI.Image image;
    public TextMeshProUGUI tmp;
    public Vector3 startPos;
    public Vector3 slotPos;
    public bool inSlot;

    public GameManager.Inputs input;

    public Slot activeSlot;

    private RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.position;
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(point.x, point.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        tmp.raycastTarget = false;

        inSlot = false;
        if (activeSlot == null) return;
        activeSlot.hasBlock = false;
        activeSlot.heldBlock = GameManager.Inputs.None;
        activeSlot = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (inSlot)
        {
            rect.position = slotPos;
        }
        else
        {
            rect.position = startPos;
        }
        
        image.raycastTarget = true;
    }
}
