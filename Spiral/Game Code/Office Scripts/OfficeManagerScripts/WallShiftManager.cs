using UnityEngine;

public class WallShiftManager : MonoBehaviour
{
    [SerializeField] Material material;
    float stress { get => StressManager.GetStress(); }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_Stress", stress);
    }
}
