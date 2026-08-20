using NUnit.Framework.Constraints;
using UnityEngine;

public class OxygenBubble : Interactable
{

    /// <summary>
    /// How long the player must wait before getting air
    /// </summary>
    [SerializeField] float TimeTillAir;
    /// <summary>
    /// How much oxygen the bubble gives
    /// </summary>
    [SerializeField] float OxygenAmount;

    protected override void CustomStart()
    {
        waitTime = TimeTillAir;
    }

    public override void Use()
    {
        Referencer.AnxietyReferences.PlayerScript.AddOxygen(OxygenAmount);
        base.Use();
    }

    void OnTriggerEnter(Collider other)
    {
        Referencer.AnxietyReferences.PlayerScript.PauseOxygen();
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        Referencer.AnxietyReferences.PlayerScript.UnPauseOxygen();
    }
}
