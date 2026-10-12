using UnityEngine;

[CreateAssetMenu(fileName = "WindowIconGroup", menuName = "Scriptable Objects/WindowIconGroup")]
public class WindowIconGroup : ScriptableObject
{
    public ComputerWindow window;
    public DesktopIcon desktopIcon;
    public ToolbarIcon toolbarIcon;
}
