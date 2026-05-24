using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public interface ITaskReader 
    {
       
        public TaskItem GetById(int id)
        {
            return GetById(id);
        }

        public IEnumerable<TaskItem> GetAll()
        {
            return GetAll();
        }
    }
}
