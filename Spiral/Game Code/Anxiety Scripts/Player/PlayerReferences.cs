using UnityEngine;

/// <summary>
/// Monobehavior that acts as a middleman for the other player scripts, so they only need to have a reference to this rather than one another.
/// </summary>
[RequireComponent(typeof(PlayerLook))]
[RequireComponent(typeof(PlayerMain))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerReferences : MonoBehaviour
{
    public PlayerInputHandler PlayerInputs;
    public PlayerMain Main;
    public PlayerLook Look;
    public GameObject PlayerCamera;

    void Awake()
    {
        PlayerInputs = GetComponent<PlayerInputHandler>();
        Main = GetComponent<PlayerMain>();
        Look = GetComponent<PlayerLook>();
    }
}
