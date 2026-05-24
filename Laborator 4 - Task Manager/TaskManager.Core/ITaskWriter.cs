using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public interface ITaskWriter
    {
       
        public void Add(TaskItem task)
        {
            Add(task);
        }
        public void Update(TaskItem task)
        {
           Update(task);
        }
        public void Delete(int id)
        {
            Delete(id);
        }
    }
}
