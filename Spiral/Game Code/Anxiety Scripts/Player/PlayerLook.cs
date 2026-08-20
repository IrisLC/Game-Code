using UnityEngine;

/// <summary>
/// Handles camera control of player in anxiety scenes
/// </summary>
[RequireComponent(typeof(PlayerReferences))]
public class PlayerLook : MonoBehaviour
{
    /// <summary>
    /// The script holding references to other objects on the player
    /// </summary>
    PlayerReferences pRef;

    /// <summary>
    /// Mouse sensitivity
    /// </summary>
    [Range(0, 2f)]
    public float Sensitivity;
    float Pitch;

    void Awake()
    {
        pRef = GetComponent<PlayerReferences>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pRef.PlayerInputs.LookPerformed += Look;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Look(Vector2 LookVector)
    {
        if (PauseManager.isGamePaused) return;


        // X Rotation
        transform.localEulerAngles = transform.localEulerAngles.Add(y: LookVector.x * Sensitivity);
        // Y Rotation
        Pitch -= LookVector.y * Sensitivity;
        Pitch = Mathf.Clamp(Pitch, -90, 90);

        pRef.PlayerCamera.transform.localEulerAngles = pRef.PlayerCamera.transform.localEulerAngles.With(x: Pitch);
    }

    void OnDisable()
    {
        pRef.PlayerInputs.LookPerformed -= Look;
    }
}
