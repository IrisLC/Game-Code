using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailButtonManager : MonoBehaviour
{
    //Button Prefab should have two text boxes in it, one for name and one for subject
    public GameObject buttonPrefab;

    //Text file (preferably a csv) that has three columns: name, subject, and text
    public TextAsset emailFile;

    //List of all email buttons made from the createButtons function
    List<EmailInfo> buttons = new List<EmailInfo>();

    private int emailCount = 0;

    //Text boxes in the window to the side that show the full email
    public TextMeshProUGUI emailName;
    public TextMeshProUGUI emailSubject;
    public TextMeshProUGUI emailText;

    //Class made to store information about emails used for buttons and the email window
    class EmailInfo
    {
        public string name { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
    }

    //Parses the emails to be used throughout the game, may need to be reworked
    void Start()
    {

        if (emailFile != null)
        {
            //var emails = CsvParser.ParseFromString<EmailInfo>(emailFile.text, true);

            // foreach (EmailInfo info in emails)
            // {
            //     buttons.Add(info);
            // }

        }

        createButtons();
    }

    //Creates a button for each email, going from the newest email to the oldest
    void createButtons()
    {
        //Clears out the old list of emails
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = emailCount; i >= 0; i--)
        {
            GameObject button = (GameObject)Instantiate(buttonPrefab);
            button.transform.SetParent(this.transform);
            makeListener(button, i);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttons[i].name;
            button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buttons[i].subject;
        }

    }

    //Adds the ability to open the email to the side based on the button you clicked
    private void makeListener(GameObject btn, int index)
    {
        btn.GetComponent<Button>().onClick.AddListener(() => { emailWindow(index); });
    }

    //Updates the window to the side that has the full email
    void emailWindow(int index)
    {
        emailName.text = buttons[index].name;
        emailSubject.text = buttons[index].subject;
        emailText.text = buttons[index].text;
    }

    // Update is called once per frame
    void updateList()
    {
        emailCount++;
        createButtons();
    }
}
