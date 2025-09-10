using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBillboard : MonoBehaviour
{
    /// <summary>
    /// 0 is forward
    /// 1 is right
    /// 2 is back
    /// 3 is left
    /// </summary>
    
    private int direction;
    private Player p;
    private EnemyBrain e;
    private Animator ar;

    [SerializeField] private bool shouldRotate = true;

    [SerializeField] private GameObject obj;


    void Start()
    {

        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        e = GetComponentInParent<EnemyBrain>();

        if(e != null) {
            obj = e.gameObject;
        } else {
            obj = gameObject;
        }

        ar = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldRotate) transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        changeSide();
        
        ar.SetInteger("direction", direction);
    }

    private void changeSide() {
        Vector3 ePos = obj.transform.position;
        Vector3 pPos = p.transform.position;
        
                
        float cos = Vector3.Dot(obj.transform.forward, (pPos - ePos).normalized);
        Vector3 cross = Vector3.Cross((pPos - ePos).normalized, obj.transform.forward);
        float sin = Vector3.Magnitude(cross);

        if(cross.y < 0) {
            sin *= -1;
        }
        

        if(Math.Abs(sin) <= (Math.Sqrt(2) / 2)) {
            if(cos >= 0) {
                // Use the front view
                direction = 0;
            } else {
                // Use back View
                direction = 2;
            }
        } else {
            if(sin > (Math.Sqrt(2) / 2)) {
                // Left View
                direction = 3;
            } else if(sin < - (Math.Sqrt(2) / 2)){
                // Right View
                direction = 1;
            }
        }
        

    }


}
