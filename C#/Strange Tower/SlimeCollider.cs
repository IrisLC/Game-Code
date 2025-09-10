using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCollider : MonoBehaviour
{
    public MeleeEnemy parent;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("test");
        parent = GetComponentInParent<MeleeEnemy>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        parent.handleCollisions(other);
    }
}
