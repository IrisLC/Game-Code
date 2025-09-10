using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projSpeed = 3f;
    [SerializeField] private bool isFiredByPlayer;
    private int power;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeathTimer());
    }

    //CALL THIS ON INSTANTIATION
    //will not deal damage otherwise
    public void setPower(int p)
    {
        power = p;
    }

    public void setSpeed(float p) {
        projSpeed = p;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * projSpeed * Time.deltaTime;
    }

    private IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(6);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFiredByPlayer)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<Player>().DamagePlayer(power);

            }
        }
        else
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<SlimeCollider>(out SlimeCollider sc))
                {
                    sc.parent.dealDamage(power);
                } else if (other.TryGetComponent<ParentCollider>(out ParentCollider pc)) {
                    pc.child.dealDamage(power);
                }
                else
                {
                    other.GetComponent<EnemyBase>().dealDamage(power);
                }



                // if (other.TryGetComponent<MeleeEnemy>(out MeleeEnemy mEnemyScript)) // if melee
                // {
                //     mEnemyScript.dealDamage(power);
                // }
                // else if (other.TryGetComponent<RangedEnemy>(out RangedEnemy rEnemyScript)) // if range
                // {
                //     rEnemyScript.dealDamage(power);
                // }
            }
        }

        Destroy(gameObject);
    }
}
