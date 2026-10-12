using UnityEngine;

public class CorrectionEmail : MonoBehaviour
{
    const string Sender = "tdmullb3892@DSOD.com";
    const string SubjectOpener = "IMPORTANT! Re: ";
    const string OpenerPart1 = "Hey Mallory, I couldn’t help but notice on ";
    const string OpenerPart2 = " that you made some errors.No worries, it happened to me all the time when I first started!"
        + "A few pieces of advice for next time, and some specific feedback:\n";
    const string Footer = "Hopefully this clears some things up!\n\n Cheers,\n Todd Mullberry";

    public const string EmailCorrection1 = "You said \"";
    public const string EmailCorrection2 = "\" when you should have said \"";
    public const string EmailCorrectionBlank1 = "You should have said \"";
    public const string EmailCorrectionBlank2 = "\" but instead you said nothing";

    public const string IncidentReportCorrection1 = "You should have censored the following: \n\t";
    public const string IncidentReportCorrection2 = "You incorrectly censored the following: \n\t";

    public const string WeatherCorrection1 = "You incorrectly stated that the weather was ";
    public const string WeatherCorrection2 = "the same.\n";

    public const string PrintCorrection1 = "You placed the paper in ";
    public const string PrintCorrection2 = " when you should have placed it in ";


    const float IndentMultiplier = 2.5f;


    void OnEnable()
    {
        ATask.OnTaskFinish += GetOutput;
    }

    void OnDisable()
    {
        ATask.OnTaskFinish -= GetOutput;
    }

    void Start()
    {
        OfficeManager.GetOfficeManager().taskManager.correctionEmail = this;
    }

    void OnDestroy()
    {
        if (OfficeManager.GetOfficeManager() != null)
        {
            OfficeManager.GetOfficeManager().taskManager.correctionEmail = null;
        }
    }

    void GetOutput(ATask Task, bool Correct, int Weight)
    {
        if (Correct) return;

        string Response = Task.GetResponse();
        string line;
        string formattedResponse = "";
        int index = 0;
        int newIndex;
        int indentNum = 1;
        int indexModifier = 0;
        bool IndentChangeLine = false;
        bool SkipLine = false;

        while (index < Response.Length - 1)
        {
            newIndex = Response.IndexOf('\n', index + indexModifier);
            if (newIndex - index < 0)
            {
                break;
            }
            line = Response.Substring(index + indexModifier, newIndex - index);
            indexModifier = 1;

            if (line == "\n\t" || line == "\n\\" || line == "\n")
            {
                if (IndentChangeLine)
                {
                    SkipLine = true;
                }

                IndentChangeLine = true;
            }
            else
            {
                IndentChangeLine = false;
                SkipLine = false;
            }

            if (indentNum > 0 && !IndentChangeLine)
            {
                line = $"<indent={indentNum * IndentMultiplier}em>\u2022 " + line.Substring(0, line.Length - 1) + "</indent>\n";
            }

            // ending a line with \n\t means to start an indented, bulleted list
            if (newIndex < Response.Length - 1 && Response[newIndex + 1] == '\t')
            {
                ++newIndex;
                ++indentNum;
            }
            // ending a line with \n\\ means to end an indented, bulleted list
            else if (indentNum > 0 && newIndex < Response.Length - 1 && Response[newIndex + 1] == '\\')
            {
                ++newIndex;
                --indentNum;
            }

            if (!SkipLine)
            {
                formattedResponse += line;
            }

            index = newIndex;
        }
        formattedResponse += "\n\n";
        string Output = OpenerPart1 + Task.TaskName + OpenerPart2 + formattedResponse + Footer;

        IncomingEmailData data = ScriptableObject.CreateInstance<IncomingEmailData>();
        data.Init(Sender, SubjectOpener + Task.TaskName, Output, -1);

        IncomingEmailSender.AddEmail(data);
    }
}
