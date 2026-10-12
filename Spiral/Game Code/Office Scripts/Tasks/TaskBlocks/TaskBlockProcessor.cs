
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskBlockProcessor : MonoBehaviour
{
    TaskBlock taskBlock;
    HashSet<string> names = new();
    int completedTasks = 0;
    bool isDueToday;
    bool isOverdue;
    public static int OverdueTasks { get; private set; }

    [SerializeField] TextMeshProUGUI DueDate;
    [SerializeField] TextMeshProUGUI ListText;
    [SerializeField] Image Background;
    [SerializeField] Image progressBar;
    public Gradient gradient;

    int RemainingTime { get => ClockCalc.TimeToMinutes(taskBlock.HourDue, taskBlock.MinuteDue) - ClockCalc.GetCurrentMinutes(); }
    [SerializeField] int RemainingTimeToStartGradient;


    public void Init(TaskBlock block)
    {
        taskBlock = block;
        string taskText = "";

        DueDate.SetText($"Due {block.DayDue} at {ClockCalc.DisplayTimeAtGivenTime(block.HourDue, block.MinuteDue, true)}");

        isDueToday = block.DayDue == GameManager.CurrentDay;
        CheckOverdue(this);

        foreach (TaskPrefab task in block.Tasks)
        {
            names.Add(task.Task.TaskName);
            string type = "";

            switch (task.Task.Type)
            {
                case ATask.TaskType.Email:
                    type = "Email";
                    break;
                case ATask.TaskType.IncidentReport:
                    type = "Incident Report";
                    break;
                case ATask.TaskType.Weather:
                    type = "Weather";
                    break;
            }


            taskText += $"\u2022 {type}: {task.Task.TaskName}\n";
        }

        ListText.SetText(taskText);
        progressBar.fillAmount = 0;
    }

    void Awake()
    {
        ATask.OnTaskFinish += CheckTask;
    }

    void OnDestroy()
    {
        ATask.OnTaskFinish -= CheckTask;

        if (isOverdue)
        {
            --OverdueTasks;
        }
    }

    void Update()
    {
        if (isDueToday && !isOverdue && RemainingTime <= RemainingTimeToStartGradient)
        {
            float normalizedTime = ((float)RemainingTime).Remap(RemainingTimeToStartGradient, 0, 0, 1);
            Background.color = gradient.Evaluate(normalizedTime);
            CheckOverdue(this);
        }
    }

    void CheckTask(ATask Task, bool Correct, int Weight)
    {
        CrossOffTask(Task.TaskName);
    }

    public void CrossOffTask(string name)
    {
        if (names.Contains(name))
        {
            ++completedTasks;

            if (completedTasks == taskBlock.Tasks.Count)
            {
                Destroy(gameObject);
                return;
            }

            int index = 0;
            while (index < ListText.text.Length)
            {
                int newIndex = ListText.text.IndexOf('\n', index);
                string line = ListText.text.Substring(index, newIndex - index);

                if (line.Contains(name))
                {
                    ListText.text = ListText.text.Replace(line, $"<s>{line}</s>");
                    break;
                }

                index = newIndex + 1;
            }

            progressBar.fillAmount = (float)completedTasks / taskBlock.Tasks.Count;
        }
    }


    static void CheckOverdue(TaskBlockProcessor processor)
    {
        // if it's already overdue then we dont need to check again
        if (processor.isOverdue) return;

        if (GameManager.CurrentDay > processor.taskBlock.DayDue)
        {
            OnOverdue(processor);
            return;
        }
        // if its due on a future day then its not overdue
        if (!processor.isDueToday)
        {
            return;
        }

        // If the time its due is less than the current time then it is late
        if (processor.RemainingTime <= 0)
        {
            OnOverdue(processor);
        }
    }

    static void OnOverdue(TaskBlockProcessor processor)
    {
        processor.isOverdue = true;
        ++OverdueTasks;
        processor.Background.color = processor.gradient.Evaluate(1);
        processor.DueDate.SetText("OVERDUE");
    }
}

