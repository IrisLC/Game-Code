using UnityEngine;

public class SitStand : Interactable
{
    GameObject standBody;
    Camera standCamera;
    Camera sitCamera;
    Transform startingTransform;

    public static bool DisableStanding;

    public static bool standing = false;

    void Awake()
    {
        type = InteractionType.LookAt;
        remainOnInteraction = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        standBody = Referencer.OfficeReferences.WalkBody;
        standCamera = Referencer.OfficeReferences.WalkCamera;
        sitCamera = Referencer.OfficeReferences.officeCamera.mCamera;
        startingTransform = standBody.transform;
        standBody.SetActive(false);
        standCamera.enabled = false;
        sitCamera.gameObject.SetActive(true);
    }

    public override void Use()
    {
        if (DisableStanding && !standing)
        {
            return;
        }

        if (StressManager.CurrentStressStage == StressManager.TotalStressStages && !standing)
        {
            OfficeManager.GetOfficeManager().EnterSpiral();
            return;
        }

        standing = !standing;

        standBody.SetActive(standing);
        standCamera.enabled = standing;
        sitCamera.gameObject.SetActive(!standing);

        if (standing)
        {
            standBody.transform.SetPositionAndRotation(startingTransform.position, startingTransform.rotation);
        }
    }


}
