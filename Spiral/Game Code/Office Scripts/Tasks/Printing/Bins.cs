using UnityEngine;

public class Bins : Interactable
{
    [SerializeField] int TaskWeight;
    /// <summary>
    /// The sortingSpot that this bin corresponds to
    /// </summary>
    [SerializeField] Paper.SortingSpot AssignedSpot;

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
        if (Referencer.OfficeReferences.officeCamera.HeldPaper == null) return;

        Referencer.OfficeReferences.officeCamera.HeldPaper.gameObject.GetComponent<Paper>().OnPlace(EvaluateTask(), AssignedSpot);

        // The paper has been used, so destroy the game object and remove the value from player cam
        Destroy(Referencer.OfficeReferences.officeCamera.HeldPaper.gameObject);
        Referencer.OfficeReferences.officeCamera.HeldPaper = null;
    }

    public bool EvaluateTask()
    {
        return Referencer.OfficeReferences.officeCamera.HeldPaper.CheckRightSpot(AssignedSpot);
    }

}
