using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    /// <summary>
    /// 0 is forward
    /// 1 is right
    /// 2 is back
    /// 3 is left
    /// </summary>
    [SerializeField] private Sprite[] spriteCollection;
    private Sprite activeSprite;
    private Sprite previousSprite;
    private bool multiple;
    private Player p;
    private EnemyBrain e;
    private SpriteRenderer sr;

    [SerializeField] private bool shouldRotate = true;

    [SerializeField] private GameObject obj;


    void Start()
    {

        
        multiple = spriteCollection.Length > 1;

        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        e = GetComponentInParent<EnemyBrain>();

        if(e != null) {
            obj = e.gameObject;
        } else {
            obj = gameObject;
        }

        activeSprite = spriteCollection[0];
        previousSprite = activeSprite;

        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = activeSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldRotate) transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);

        if(multiple) {
            changeSide();

            if(previousSprite != activeSprite) {
                sr.sprite = activeSprite;
                previousSprite = activeSprite;
            }
        }
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
                activeSprite = spriteCollection[0];
            } else {
                // Use back View
                activeSprite = spriteCollection[2];
            }
        } else {
            if(sin > (Math.Sqrt(2) / 2)) {
                // Left View
                activeSprite = spriteCollection[3];
            } else if(sin < - (Math.Sqrt(2) / 2)){
                // Right View
                activeSprite = spriteCollection[1];
            }
        }
        

    }


}
