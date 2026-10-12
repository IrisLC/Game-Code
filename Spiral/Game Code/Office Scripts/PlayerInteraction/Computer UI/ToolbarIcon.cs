using UnityEngine;
using UnityEngine.EventSystems;

public class ToolbarIcon : MonoBehaviour, IPointerClickHandler
{
    public WindowIconGroup iconGroup;

    void Awake()
    {
        iconGroup.toolbarIcon = this;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        iconGroup.window.OpenWindow();
    }
}
