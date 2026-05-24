using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class TaskValidator
    {
        public void Validate(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new Exception("Title must not be empty!");

            if (task.Title.Length > 200)
                throw new Exception("Title is too long(max 200 characters)!");

            if(task is DeadlineTask deadlineTask)
            {
                if (deadlineTask.DueDate <= DateTime.Now)
                    throw new Exception("Deadline must be set in the future!");
            }    
        }

    }
}
