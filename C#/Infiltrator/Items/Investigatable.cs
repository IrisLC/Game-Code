using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Investigatable : MonoBehaviour
{
    /// <summary>
    /// The minimum alertness needed to investigate the object
    /// </summary>
    public float minToInterest;
    /// <summary>
    /// How much to raise the alert level of an enemy that finds the object
    /// </summary>
    public float alertRaiseAmount;

    /// <summary>
    /// If the object is in a state to be investigated
    /// 
    /// I.e. doors
    /// </summary>
    public bool isOfInterest;

    /// <summary>
    /// The max distance for the enemy to search around the object
    /// </summary>
    public float searchDistance;

    public bool isSound = false;

    public virtual void use() {
        Destroy(gameObject);
    }
}
