using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
  [SerializeField] private AudioSource audio; 
  private bool isAttacking = false;
  private bool once = false;
  // Start is called before the first frame update
  void Start()
  {

  }

  protected override void attack()
  {
    if (canAttack)
    {
      canAttack = false;
      isAttacking = true;
      if (p != null)
      {
        p.DamagePlayer(power);
        StartCoroutine(attackCooldownTimer());
      }

    }
  }

  protected override void move()
  {
    if(!audio.isPlaying && !once) {
      audio.Play();
      once = true;
    }


    if (agent != null)
      if (Vector3.Distance(gameObject.transform.position, player.transform.position) >= 1)
      {
        agent.destination = player.transform.position;
      }
      else
      {
        agent.destination = gameObject.transform.position;
      }
    else
    {
      Debug.LogWarning("Navmesh has not been set");
    }
  }

  public void handleCollisions(Collider other)
  {
    OnTriggerEnter(other);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject == GameObject.FindGameObjectWithTag("Explosion"))
    {
      dealDamage(GameObject.FindGameObjectWithTag("Explosion").GetComponent<Explosion>().GetDamage());

    }

    if (isAttacking)
    {
      if (other.gameObject == player)
      {
        p.DamagePlayer(power);
        isAttacking = false;
      }
    }
  }
}
