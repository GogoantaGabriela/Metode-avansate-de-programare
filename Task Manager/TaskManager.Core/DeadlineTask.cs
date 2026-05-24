using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class DeadlineTask : TaskItem
    {
        public TaskItem Item { get; set; }
        public DateTime DueDate { get; set; }

        public int Priority { get; set; } = 1;


        public DeadlineTask(int id, string title, string description, DateTime created, string status, TaskItem item, DateTime dueDate)
        : base(id, title, description, created, status)
        {
            Item = item;
            DueDate = dueDate;
        }

        public DeadlineTask(string title, DateTime dueDate)
        {
            Title = title;
            DueDate = dueDate;
        }

        protected override void CompleteCore()
        {
            Status = "Done";

            Console.WriteLine($"Deadline task {Title} completed before {DueDate}.");

        }
    }
}
