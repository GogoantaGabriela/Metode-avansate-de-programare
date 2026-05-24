using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class RecurringTask : TaskItem
    {
        public int Priority { get; set; } = 2;
        private DateTime DueDate { get; set; }
        private TimeSpan ReurrenceInterval { get; set; } = TimeSpan.FromDays(7); 
        public RecurringTask(int id, string title, string description, DateTime created, string notificationType, DateTime dueDate, TimeSpan recurrenceInterval) : base(id, title, description, created, notificationType )
        {
            DueDate = dueDate;
            ReurrenceInterval = recurrenceInterval;
        }

        public RecurringTask(string title)
        {
            Title = title;
        }

        protected override void CompleteCore()
        {
            Status = "Done";

            DueDate = DueDate.Add(ReurrenceInterval);

            Console.WriteLine($"Task {Title} completed. Next occurrence scheduled for {DueDate}");

        }   
    }
}
