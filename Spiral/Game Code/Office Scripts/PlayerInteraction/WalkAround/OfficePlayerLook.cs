using UnityEngine;

/// <summary>
/// Handles camera control of player in anxiety scenes
/// </summary>
[RequireComponent(typeof(OfficePlayerReferences))]
public class OfficePlayerLook : MonoBehaviour
{
    /// <summary>
    /// The script holding references to other objects on the player
    /// </summary>
    OfficePlayerReferences pRef;

    /// <summary>
    /// Mouse sensitivity
    /// </summary>
    [Range(0, 2f)]
    public float Sensitivity;
    float Pitch;

    void Awake()
    {
        pRef = GetComponent<OfficePlayerReferences>();
        gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
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

        pRef.OfficePlayerCamera.transform.localEulerAngles = pRef.OfficePlayerCamera.transform.localEulerAngles.With(x: Pitch);
    }

    void OnDisable()
    {
        pRef.PlayerInputs.LookPerformed -= Look;
    }
}
