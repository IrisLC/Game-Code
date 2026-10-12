using TMPro;
using UnityEngine;

public class IncomingEmailSender : PercentNotifyReceiver<IncomingEmailData>
{

    static GameObject EmailPrefab;
    static GameObject ButtonPrefab;

    public delegate void OnEmailSend(IncomingEmailData data, GameObject email);
    public static event OnEmailSend EmailSent;

    protected override void Awake()
    {
        base.Awake();
        EmailPrefab = Resources.Load("IncomingEmailTemplate") as GameObject;
        ButtonPrefab = Resources.Load("InboxButton") as GameObject;
    }

    /// <summary>
    /// Allows external scripts to add an email to the list
    /// </summary>
    /// <param name="email"></param>
    public static void AddEmail(IncomingEmailData email)
    {
        float percent = email.PercentThroughDayToSend;

        if (percent < 0)
        {
            SendEmail(email);
            return;
        }

        SubscribeToNotify(percent);

        AddDataToDictionary(email);
    }

    static void SendEmail(IncomingEmailData email)
    {
        DataSpawnedToday.Add(email);
        GameObject emailObject = CreateIncomingEmailObject(email);
        CreateIncomingEmailButton(emailObject, email);

        EmailSent?.Invoke(email, emailObject);
    }

    public static GameObject CreateIncomingEmailObject(IncomingEmailData email)
    {
        GameObject emailObject = Instantiate(EmailPrefab, Referencer.OfficeReferences.EmailHolder.transform);
        emailObject.GetComponent<IncomingEmailProcessor>().FillEmail(email);
        return emailObject;
    }

    public static GameObject CreateIncomingEmailButton(GameObject emailObject, IncomingEmailData email)
    {
        GameObject button = Instantiate(ButtonPrefab, Referencer.OfficeReferences.IncomingEmailButtonHolder.transform);
        button.transform.SetAsFirstSibling();
        button.GetComponent<IncomingEmailButton>().email = emailObject;
        emailObject.GetComponent<IncomingEmailProcessor>().button = button;
        button.GetComponentInChildren<TextMeshProUGUI>().SetText(email.Subject);
        return button;
    }

    protected override void SpawnData(IncomingEmailData data)
    {
        // Send email needs to remain static so AddEmail can be static and 
        //   thus called by outside scripts without a reference, so this just passes along the data
        SendEmail(data);
    }
}
