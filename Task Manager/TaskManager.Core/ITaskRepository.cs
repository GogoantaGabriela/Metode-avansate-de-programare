using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public interface ITaskRepository : ITaskReader, ITaskWriter
    {
        IEnumerable<TaskItem> GetAll();
        TaskItem GetById(int id);
        
    }
}
