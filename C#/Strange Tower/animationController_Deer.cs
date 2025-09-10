using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController_Deer : MonoBehaviour
{
    private RangedEnemy re;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        re = gameObject.GetComponentInParent<RangedEnemy>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (re.getHasSeen())
        {
            anim.SetBool("hasSeen", true);
        }
    }
}
