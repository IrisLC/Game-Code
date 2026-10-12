
using System.Collections.Generic;
using UnityEngine;

public class SubmittedIRQueue : MonoBehaviour
{
    struct IRData
    {
        public IRData(string Name, string Text, bool Completion)
        {
            name = Name;
            text = Text;
            completion = Completion;
        }

        public string name;
        public string text;
        public bool completion;
    }
    static Queue<IRData> SubmittedIRs = new();
    public static string CurrentName { get => SubmittedIRs.Peek().name; }
    public static bool CurrentCompletion { get => SubmittedIRs.Peek().completion; }
    public static bool IsEmpty { get => SubmittedIRs.Count == 0; }

    void Awake()
    {
        SubmittedIRs.Clear();
    }

    public static void AddIncidentReport(string Name, string Text, bool Completion)
    {
        SubmittedIRs.Enqueue(new IRData(Name, Text, Completion));

        if (SubmittedIRs.Count == 1)
        {
            OnChange();
        }
    }

    static void OnChange()
    {
        if (IsEmpty)
        {
            Referencer.OfficeReferences.PaperRendererText.SetText("");
        }
        else
        {
            Referencer.OfficeReferences.PaperRendererText.SetText(SubmittedIRs.Peek().text);
        }


    }

    public static void RemoveIncidentReport()
    {
        SubmittedIRs.Dequeue();

        OnChange();
    }
}
