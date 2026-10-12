using TMPro;
using UnityEngine;

public class IncomingEmailProcessor : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI SenderField;
    [SerializeField] TextMeshProUGUI SubjectField;
    [SerializeField] TextMeshProUGUI BodyField;

    public GameObject button;

    public void FillEmail(IncomingEmailData email)
    {
        SenderField?.SetText(email.Sender);
        SubjectField?.SetText(email.Subject);
        BodyField?.SetText(email.Body);
    }

    void OnDestroy()
    {
        Destroy(button);
    }
}
