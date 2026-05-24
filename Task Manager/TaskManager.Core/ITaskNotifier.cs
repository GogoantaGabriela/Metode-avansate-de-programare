using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public interface ITaskNotifier
    {
        void Notify(TaskItem task);
    }
}
