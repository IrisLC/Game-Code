using UnityEngine;
using UnityEngine.EventSystems;

public class IncomingEmailButton : TaskListButton
{
    public GameObject email;
    public override void OnPointerClick(PointerEventData eventData)
    {

        Computer.tasksWindow.ChangeItem(email, true);
    }
}
