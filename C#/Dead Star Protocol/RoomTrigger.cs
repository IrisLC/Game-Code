using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public bool isOccupied;

    public Light[] lights;

    void OnTriggerEnter(Collider other)
    {
        isOccupied = true;
    }

    void OnTriggerExit(Collider other)
    {
        isOccupied = false;
    }
}
