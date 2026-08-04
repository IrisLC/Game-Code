
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] GameObject playButton, stopButton;
    [SerializeField] GameObject buttonBlock;

    [SerializeField] InputReader ir;
    [SerializeField] GameManager gm;

    public int activeIndex;

    Slot lastActive;
    Color activeSlotColor = new Color(0.6226415f, 0.6226415f, 0.6226415f, .7372549f);
    Color inactiveSlotColor = new Color(0.6226415f, 0.6226415f, 0.6226415f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        ir = GameObject.FindGameObjectWithTag("InputReader").GetComponent<InputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ir.slots[0].heldBlock == GameManager.Inputs.None)
        {
            buttonBlock.SetActive(true);
        }
        else
        {
            buttonBlock.SetActive(false);
        }

        if (gm.isPlaying)
        {
            playButton.SetActive(false);
            stopButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(true);
            stopButton.SetActive(false);
        }

        if (gm.isPlaying)
        {
            Slot activeSlot = ir.usedSlots[activeIndex];
            if (lastActive == null || activeSlot != lastActive)
            {
                activeSlot.transform.parent.GetComponent<UnityEngine.UI.Image>().color = activeSlotColor;

                if (lastActive != null)
                {
                    lastActive.transform.parent.GetComponent<UnityEngine.UI.Image>().color = inactiveSlotColor;
                }
                lastActive = activeSlot;
            }
        }
        else
        {
            if (lastActive != null)
            {
                lastActive.transform.parent.GetComponent<UnityEngine.UI.Image>().color = inactiveSlotColor;
            }
        }
    }

    public void Reset()
    {
        gm.Reset();
    }
}
