
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// The different types of Interactable
    /// </summary>
    public enum InteractionType { LookAt, Area }

    /// <summary>
    /// The type of interactable this is
    /// </summary>
    [Header("Function")]
    public InteractionType type;
    /// <summary>
    /// If the interactable should continue existing after it has been interacted with.
    /// False means interactable is deleted after use.
    /// </summary>
    [SerializeField] protected bool remainOnInteraction;

    /// <summary>
    /// The collider of the interactable
    /// </summary>
    protected SphereCollider col;
    /// <summary>
    /// How long the player must wait inside the area of the Interactable for it to activate
    /// </summary>
    protected float waitTime = 2f;
    /// <summary>
    /// The coroutine keeping track of how long the player is in the interactable
    /// </summary>
    protected bool CoroutineRunning;
    /// <summary>
    /// The player gameObject
    /// </summary>
    protected GameObject Player;

#if UNITY_EDITOR
    /// <summary>
    /// If Debug.Log commands should be called, modified with DebugCommands
    /// </summary>
    internal static bool isDebugging;
#endif

    void Awake()
    {
        if (type == InteractionType.Area)
        {
            col = gameObject.GetOrAdd<SphereCollider>();
            col.isTrigger = true;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.CurrentSceneType == GameManager.SceneTypes.Office)
        {
            Player = Referencer.OfficeReferences.officeCamera.gameObject;
        }
        else if (GameManager.CurrentSceneType == GameManager.SceneTypes.Anxiety)
        {
            Player = Referencer.AnxietyReferences.PlayerGameObject;
        }

        CustomStart();
    }

    /// <summary>
    /// Method used by child scripts for special things to happen on start, as we always want this start to be called
    /// </summary>
    protected virtual void CustomStart() { }

    /// <summary>
    /// Handles the player interacting with the interactable
    /// </summary>
    public virtual void Use()
    {
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("Interactable: Player is Using");
#endif

        if (!remainOnInteraction)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (type != InteractionType.Area) return;

        if (other.transform.gameObject == Player && !CoroutineRunning)
        {
#if UNITY_EDITOR
            if (isDebugging) Debug.Log("Player Enter Trigger");
#endif
            StartCoroutine("StayInTrigger");
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject == Player)
        {
#if UNITY_EDITOR
            if (isDebugging) Debug.Log("Player Exit Trigger");
#endif
            StopCoroutine("StayInTrigger");
            CoroutineRunning = false;
        }
    }

    /// <summary>
    /// Timer for delay based interactables
    /// </summary>
    /// <returns></returns>
    IEnumerator StayInTrigger()
    {
#if UNITY_EDITOR
        if (isDebugging) Debug.Log("Interactable: Start StayInTrigger Timer");
#endif
        CoroutineRunning = true;
        yield return new WaitForSeconds(waitTime);
        CoroutineRunning = false;
        Use();
    }

}
