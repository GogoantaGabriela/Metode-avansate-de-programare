using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class TaskItem
    {

        public int Id { get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
        DateTime Created { get; set; }
        
        public string Status { get; set; } = "Pending";

        public int Priority { get; set; } = 0;

        public string NotificationType { get; set; }

        public TaskItem()
        {
        }
        public TaskItem(int id, string title, string description, DateTime created, string notificationType )
        {
            Id = id;
            Title = title;
            Description = description;
            Created = created;
            NotificationType = notificationType;
            Status = "Pending";
        }

        public TaskItem(string title)
        {
            Title = title;
        }

        public void Complete()
        {
            if (Status == "Done")
                throw new Exception("Task is already completed!");

            CompleteCore();

            if (Status == "Overdue")
                throw new InvalidOperationException("Postcondition violated!");

            if (Status == "Done" && Status == "Overdue")
                throw new InvalidOperationException("Invariant violated!");
        }
            protected virtual void CompleteCore()
            {
                Status = "Done";
            }
    }
}
