using System.Collections.Generic;
using UnityEngine;


public class ComputerScript : MonoBehaviour
{
    public TasksWindow tasksWindow;
    /// <summary>
    /// The button to close the computer
    /// </summary>
    public GameObject ExitButton;

    /// <summary>
    /// Whether or not the computer is on
    /// </summary>
    public bool IsComputerOn { get; private set; }

    /// <summary>
    /// A type of event fired when the computer is turned on or off.
    /// </summary>
    public delegate void ComputerModified();
    /// <summary>
    /// The event fired when the computer is turned on.
    /// </summary>
    public static event ComputerModified ComputerTurnedOn;
    /// <summary>
    /// The event fired when the computer is turned off.
    /// </summary>
    public static event ComputerModified ComputerTurnedOff;

    [SerializeField] List<WindowIconGroup> windowIconGroups = new();

    void Awake()
    {
    }

    /// <summary>
    /// Method that handles behavior when the players closes the computer
    /// </summary>
    public void ExitComputer()
    {
        IsComputerOn = false;
        Referencer.OfficeReferences.ComputerUI.gameObject.SetActive(false);
        ComputerTurnedOff?.Invoke();
    }

    /// <summary>
    /// Method that handles behavior when the players opens the computer
    /// </summary>
    public void OpenComputer()
    {
        IsComputerOn = true;
        Referencer.OfficeReferences.ComputerUI.gameObject.SetActive(true);
        ComputerTurnedOn?.Invoke();
    }


}
