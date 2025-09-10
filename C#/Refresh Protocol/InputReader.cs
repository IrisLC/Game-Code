using System.Collections.Generic;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public Slot[] slots;
    public List<GameManager.Inputs> inputs;
    public List<Slot> usedSlots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputs = new List<GameManager.Inputs>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<GameManager.Inputs> readInputs()
    {
        inputs.Clear();
        foreach (Slot s in slots)
        {
            if (s.heldBlock == GameManager.Inputs.None) continue;
            inputs.Add(s.heldBlock);
            usedSlots.Add(s);
        }

        return inputs;
    }
}
