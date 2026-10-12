using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class ComputerWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public WindowIconGroup iconGroup;
    [SerializeField] protected List<GameObject> TabBodies;
    protected int currentTab = 0;

    ComputerWindowHeader header;

    protected enum ActiveState
    {
        Closed, Minimized, Open
    }
    protected ActiveState state = ActiveState.Closed;
    const float EdgeWidth = 20;
    protected RectTransform rectTransform;
    public enum WindowEdgeHorizontal
    {
        Null, Left, Right
    }
    public enum WindowEdgeVertical
    {
        Null, Top, Bottom
    }
    WindowEdgeHorizontal hoveringEdgeHorizontal;
    WindowEdgeVertical hoveringEdgeVertical;
    bool isHoveringOnEdge { get => !(hoveringEdgeHorizontal == WindowEdgeHorizontal.Null && hoveringEdgeVertical == WindowEdgeVertical.Null); }
    bool isDragging;
    /// <summary>
    /// The window currently in front
    /// </summary>
    protected static ComputerWindow frontWindow;
    Vector2 initialWidthHeight;

    Vector2 OriginalOffsetMin;
    Vector2 OriginalOffsetMax;
    Vector2 LastSetOffsetMin;
    Vector2 LastSetOffsetMax;
    public bool isFullscreen;

    bool shouldClear;
    public delegate void ResizeCursor(WindowEdgeHorizontal hori, WindowEdgeVertical verti);
    public static event ResizeCursor ResizeCursorOn;
    public static event Action ResizeCursorOff;

    void Awake()
    {
        ParentAwake();
    }

    protected void ParentAwake()
    {
        rectTransform = GetComponent<RectTransform>();
        iconGroup.window = this;
        header = gameObject.GetComponentInChildren<ComputerWindowHeader>();
        header.Parent = gameObject;

        OriginalOffsetMin = rectTransform.offsetMin;
        OriginalOffsetMax = rectTransform.offsetMax;

        StartCoroutine(DelayClose());
    }

    IEnumerator DelayClose()
    {
        yield return null;
        gameObject.SetActive(false);
    }

    public virtual void MinimizeWindow()
    {
        state = ActiveState.Minimized;
        gameObject.SetActive(false);
    }

    public virtual void CloseWindow()
    {
        state = ActiveState.Closed;
        gameObject.SetActive(false);
        iconGroup.toolbarIcon.gameObject.SetActive(false);
        rectTransform.offsetMin = OriginalOffsetMin;
        rectTransform.offsetMax = OriginalOffsetMax;
    }

    public void Fullscreen()
    {
        if (isFullscreen)
        {
            rectTransform.offsetMin = LastSetOffsetMin;
            rectTransform.offsetMax = LastSetOffsetMax;

            isFullscreen = false;
        }
        else
        {
            LastSetOffsetMin = rectTransform.offsetMin;
            LastSetOffsetMax = rectTransform.offsetMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            isFullscreen = true;
        }
    }

    public virtual void OpenWindow()
    {
        if (state == ActiveState.Closed)
        {
            OpenTab(0);
            iconGroup.toolbarIcon.gameObject.SetActive(true);
        }
        state = ActiveState.Open;
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        frontWindow = this;
    }

    /// <summary>
    /// Sets the given tab to active, whilst setting all other tabs to inactive
    /// </summary>
    /// <param name="Tab">The tab type to open</param>
    public virtual void OpenTab(int tabIndex)
    {
        currentTab = tabIndex;
        ClearPages();

        TabBodies[tabIndex].SetActive(true);
    }

    public void ClearPages()
    {
        foreach (GameObject tab in TabBodies)
        {
            if (tab) tab.SetActive(false);
        }
    }

    void Update()
    {
        if (!isDragging)
        {
            CheckForOnEdge();
        }
    }

    // Hovering
    #region
    void CheckForOnEdge()
    {
        WindowEdgeHorizontal tempH = hoveringEdgeHorizontal;
        WindowEdgeVertical tempV = hoveringEdgeVertical;

        hoveringEdgeHorizontal = WindowEdgeHorizontal.Null;
        hoveringEdgeVertical = WindowEdgeVertical.Null;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
            Mouse.current.position.ReadValue(), Camera.main, out Vector2 localMousePoint) && rectTransform.rect.Contains(localMousePoint))
        {
            float distToLeftEdge = localMousePoint.x - rectTransform.rect.xMin;
            float distToRightEdge = rectTransform.rect.xMax - localMousePoint.x;
            float distToBottomEdge = localMousePoint.y - rectTransform.rect.yMin;
            float distToTopEdge = rectTransform.rect.yMax - localMousePoint.y;

            if (distToLeftEdge < EdgeWidth)
            {
                hoveringEdgeHorizontal = WindowEdgeHorizontal.Left;
            }
            else if (distToRightEdge < EdgeWidth)
            {
                hoveringEdgeHorizontal = WindowEdgeHorizontal.Right;
            }

            if (distToTopEdge < EdgeWidth)
            {
                //hoveringEdgeVertical = WindowEdgeVertical.Top;
            }
            else if (distToBottomEdge < EdgeWidth)
            {
                hoveringEdgeVertical = WindowEdgeVertical.Bottom;
            }

        }

        if (tempV != hoveringEdgeVertical || tempH != hoveringEdgeHorizontal)
        {
            if (shouldClear)
            {
                ResizeCursorOff?.Invoke();
                shouldClear = false;
            }
            if (isHoveringOnEdge)
            {
                ResizeCursorOn?.Invoke(hoveringEdgeHorizontal, hoveringEdgeVertical);
                shouldClear = true;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // If we aren't on an edge, or if there's a window on top of this and 
        //  it is also registering as the mouse being on the edge, 
        //  don't start resizing
        if (!isHoveringOnEdge || (frontWindow != this && frontWindow.isHoveringOnEdge))
        {
            return;
        }

        isDragging = true;

        Vector2 newPivotPoint = new Vector2(0.5f, 0.5f);

        switch (hoveringEdgeHorizontal)
        {
            case WindowEdgeHorizontal.Left:
                newPivotPoint = newPivotPoint.With(x: 1);
                break;
            case WindowEdgeHorizontal.Right:
                newPivotPoint = newPivotPoint.With(x: 0);
                break;
        }

        switch (hoveringEdgeVertical)
        {
            case WindowEdgeVertical.Top:
                newPivotPoint = newPivotPoint.With(y: 0);
                break;
            case WindowEdgeVertical.Bottom:
                newPivotPoint = newPivotPoint.With(y: 1);
                break;
        }

        rectTransform.SetPivot(newPivotPoint);

        initialWidthHeight = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        Resize(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || !isHoveringOnEdge)
        {
            return;
        }

        Resize(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Resize(eventData);
        isDragging = false;
        rectTransform.SetPivot(new Vector2(0.5f, 0.5f));
    }

    void Resize(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
            eventData.position, Camera.main, out Vector2 position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
            eventData.pressPosition, Camera.main, out Vector2 pressPosition);

        Vector2 mouseDelta = position - pressPosition;

        if (hoveringEdgeHorizontal == WindowEdgeHorizontal.Left)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialWidthHeight.x - mouseDelta.x);
        }
        else if (hoveringEdgeHorizontal == WindowEdgeHorizontal.Right)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initialWidthHeight.x + mouseDelta.x);
        }

        if (hoveringEdgeVertical == WindowEdgeVertical.Bottom)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWidthHeight.y - mouseDelta.y);
        }
        else if (hoveringEdgeVertical == WindowEdgeVertical.Top)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, initialWidthHeight.y + mouseDelta.y);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        frontWindow = this;
        rectTransform.SetAsLastSibling();
    }
    #endregion
}

