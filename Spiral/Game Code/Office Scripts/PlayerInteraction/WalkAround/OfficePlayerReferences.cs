using UnityEngine;

/// <summary>
/// Monobehavior that acts as a middleman for the other player scripts, so they only need to have a reference to this rather than one another.
/// </summary>
[RequireComponent(typeof(OfficePlayerLook))]
[RequireComponent(typeof(OfficePlayerMain))]
[RequireComponent(typeof(OfficePlayerInputHandler))]
public class OfficePlayerReferences : MonoBehaviour
{
    public OfficePlayerInputHandler PlayerInputs;
    public OfficePlayerMain Main;
    public OfficePlayerLook Look;
    public Camera OfficePlayerCamera;

    void Awake()
    {
        PlayerInputs = GetComponent<OfficePlayerInputHandler>();
        Main = GetComponent<OfficePlayerMain>();
        Look = GetComponent<OfficePlayerLook>();

    }
}
