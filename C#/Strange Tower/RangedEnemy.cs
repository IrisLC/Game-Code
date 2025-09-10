using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject backward;
    [SerializeField] private GameObject projSpawn;
    [SerializeField] private float projectileSpeed = 10;
    [SerializeField] private AudioSource audio; 
    [SerializeField] private AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {

        if (player != null)
        {
            p = player.GetComponent<Player>();
        }
    }

    protected override void move()
    {
        if(speed != 0) {
            float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);

            if (distance > 10)
            {
                agent.destination = player.transform.position;
            }
            else if (distance <= 10 && distance >= 9)
            {
                agent.destination = transform.position;

            }
            else
            {
                agent.destination = backward.transform.position;

            }
        }

        Vector3 targetPosition = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
        gameObject.transform.LookAt(targetPosition);
        
    }

    protected override void attack()
    {
        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(attackCooldownTimer());
            //The direction to shoot in
            projSpawn.transform.LookAt(player.transform.position);

            // Vector3 fireDirection = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 1);
            //Creates the projectile and sets its power to be that of the enemy shooting it
            GameObject proj = Instantiate(projectile, projSpawn.transform.position, projSpawn.transform.rotation);
            proj.GetComponent<Projectile>().setPower(power);
            proj.GetComponent<Projectile>().setSpeed(projectileSpeed);
            audio.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.FindGameObjectWithTag("Explosion"))
        {
            dealDamage(GameObject.FindGameObjectWithTag("Explosion").GetComponent<Explosion>().GetDamage());
            
        }
        
    }

    public void handleCollisions(Collider other)
  {
    OnTriggerEnter(other);
  }

}
