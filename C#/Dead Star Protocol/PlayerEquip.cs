using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public class PlayerEquip : MonoBehaviour
{
    public enum Equipped
    {
        Remote, Radio, Empty
    }
    public Equipped equippedItem;

    /// <summary>
    /// 0: remote
    /// 1: radio
    /// 2: empty
    /// </summary>
    [SerializeField] private GameObject[] itemObjects;

    private InputAction prev;
    private InputAction next;
    private InputAction empty;
    public AudioClip swapSFX;
    AudioManager am;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        equippedItem = Equipped.Empty;

        PlayerController pm = GetComponent<PlayerController>();
        prev = pm.playerActions.Previous;
        next = pm.playerActions.Next;
        empty = pm.playerActions.Empty;

        prev.Enable();
        next.Enable();
        empty.Enable();
        am = FindAnyObjectByType<AudioManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameTimeManager.GetIsGamePaused())
        {
            return;
        }

        if (prev.IsPressed())
        {
            am.PlaySFX(swapSFX);
            equippedItem = Equipped.Remote;
            GetComponent<PlayerController>().RepeatRadio();
            changeItem();
        }
        else if (next.IsPressed())
        {
            am.PlaySFX(swapSFX);
            equippedItem = Equipped.Radio;
            GetComponent<PlayerController>().RepeatRadio();
            changeItem();
        }
        else if (empty.IsPressed())
        {
            am.PlaySFX(swapSFX);
            equippedItem = Equipped.Empty;
            changeItem();
        }

    }

    void changeItem()
    {
        if (equippedItem == Equipped.Remote)
        {
            itemObjects[0].SetActive(true);
            itemObjects[1].SetActive(false);
        }
        else if (equippedItem == Equipped.Radio)
        {
            itemObjects[0].SetActive(false);
            itemObjects[1].SetActive(true);
        }
        else
        {
            itemObjects[0].SetActive(false);
            itemObjects[1].SetActive(false);
        }
    }
}
