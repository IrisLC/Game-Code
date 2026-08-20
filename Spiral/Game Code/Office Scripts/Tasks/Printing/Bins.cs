using UnityEngine;

public class Bins : Interactable
{
    [SerializeField] int TaskWeight;
    /// <summary>
    /// The sortingSpot that this bin corresponds to
    /// </summary>
    [SerializeField] Paper.SortingSpot AssignedSpot;

    /// <summary>
    /// Player's camera
    /// </summary>
    OfficeCamera officeCam = Referencer.OfficeReferences.officeCamera;

    /// <summary>
    /// The active PrintTask that paper sorted into this Bin will be sent to
    /// </summary>
    public PrintTask assignedTask;

    void OnValidate()
    {
        // Assign parent script values
        remainOnInteraction = true;
        type = Interactable.InteractionType.LookAt;
    }


    /// <summary>
    /// Takes the paper held by the player
    /// </summary>
    public override void Use()
    {
        // don't do anything if there is not paper
        if (officeCam.HeldPaper == null) return;

        assignedTask.OnPlace(EvaluateTask(), AssignedSpot);

        // The paper has been used, so destroy the game object and remove the value from player cam
        Destroy(officeCam.HeldPaper.gameObject);
        officeCam.HeldPaper = null;
    }

    public bool EvaluateTask()
    {
        return officeCam.HeldPaper.CheckRightSpot(AssignedSpot);
    }

}
