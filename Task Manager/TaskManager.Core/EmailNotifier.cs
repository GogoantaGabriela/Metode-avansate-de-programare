using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class EmailNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task) => Console.WriteLine($"Email sent to user for task: {task.Title} is completed.");
    }

    public class ConsoleNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task) => Console.WriteLine($"Task completed: {task.Title}");
    }
    public class FileLogNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task) => File.AppendAllText("tasks.log", $"[{DateTime.Now}] Done: {task.Title}\n");
    }

    public class SlackNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task) => Console.WriteLine($"Slack notification: {task.Title}");
    }
}
