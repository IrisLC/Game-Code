using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public GameManager.Inputs heldBlock = GameManager.Inputs.None;

    public bool hasBlock;

    public void OnDrop(PointerEventData eventData)
    {
        Block block = eventData.pointerDrag.GetComponent<Block>();

        if (block != null)
        {
            block.slotPos = transform.position;
            block.inSlot = true;

            heldBlock = block.input;
            block.activeSlot = this;
            hasBlock = true;
        }
    }
}
