using UnityEngine;

[CreateAssetMenu(fileName = "IncomingEmailData", menuName = "Scriptable Objects/IncomingEmailData")]
public class IncomingEmailData : PercentNotifyData
{
    public string Sender { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }

    public void Init(string Sender, string Subject, string Body, float PercentThroughDayToSend = -1)
    {
        this.PercentThroughDayToSend = PercentThroughDayToSend;
        this.Sender = Sender;
        this.Subject = Subject;
        this.Body = Body;
    }


}
