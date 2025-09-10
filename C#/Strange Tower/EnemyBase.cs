using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
  [SerializeField] protected int health;
  //damage dealt
  [SerializeField] protected int power;
  [SerializeField] protected float speed;
  //range of attack
  [SerializeField] protected float range;
  //time between attacks
  [SerializeField] protected float attackCooldown;
  //Distance to notice player
  [SerializeField] protected float detectionRange = 10;
  protected bool hasSeenPlayer = false;

  protected bool canAttack = true;
  public NavMeshAgent agent;
  public GameObject player;
  public Player p;

  [SerializeField] private float flashTime = 0.25f;
  Color origColor;
  public GameObject meshHolder;
  [SerializeField] public Material mat;


  // Start is called before the first frame update
  void Awake()
  {
    //Gets the mesh renderer or skinned mesh renderer and then assigns the first material of that to the mat variable
    if(meshHolder.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer sk)) {
      mat = GetComponentInChildren<SkinnedMeshRenderer>(true).materials[0];
    } else {
      mat = GetComponentInChildren<MeshRenderer>(true).materials[0];
    }

    

    origColor = mat.color;

    player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
      p = player.GetComponent<Player>();
    }
    else
    {
      Debug.Log("player not found");
    }

  }

  // Update is called once per frame
  void Update()
  {
    //if the player does not exist, then check again every frame
    if (player == null)
    {

      player = GameObject.FindGameObjectWithTag("Player");
      if (player != null)
      {
        p = player.GetComponent<Player>();
      }
      else
      {
        Debug.Log("No Player Found");
      }
      //only runs scripts that use player positions if the player
    }
    else
    {
      if (hasSeenPlayer)
      {
        move();
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= range)
        {
          attack();
        }
      }
      else
      {
        lookForPlayer();
      }

    }



    if (!checkHealth())
    {
      if(transform.parent!= null) {
        if(transform.parent.TryGetComponent<ParentCollider>(out ParentCollider pc)) {
          Destroy(pc.gameObject);
        }
        
      }

      Destroy(gameObject);
    }
  }

  private void lookForPlayer()
  {
    if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= detectionRange)
    {
      hasSeenPlayer = true;
    }
  }

  private bool checkHealth()
  {
    if (health <= 0)
    {
      return false;
    }

    return true;
  }

  protected IEnumerator attackCooldownTimer()
  {
    yield return new WaitForSeconds(attackCooldown);
    canAttack = true;
  }

  /* --- abstract methods --- */
  protected abstract void attack();

  protected abstract void move();

  /* --- public methods (called by player) --- */

  // deals damage to the enemy
  public void dealDamage(int dmg)
  {
    hasSeenPlayer = true;
    health -= dmg;
    Debug.Log("OWWW " + gameObject.name + " Heath = " + health);
    StartCoroutine(flash());
  }

  public int getHealth()
  {
    return health;
  }


  private IEnumerator flash()
  {
    mat.color = Color.red;
    yield return new WaitForSeconds(flashTime);
    mat.color = origColor;
  }

  
    public bool getHasSeen()
    {
        return hasSeenPlayer;
    }
}
