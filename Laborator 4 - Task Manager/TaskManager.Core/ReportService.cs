using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class ReportService 
    {
        private readonly ITaskReader _taskReader;
        ReportService(ITaskReader _reader) {
            _taskReader = _reader;
        }

        public string GenerateSummary()
        {
            var tasks = _taskReader.GetAll();

            int total = tasks.Count();

            int done = tasks.Count(t => t.Status == TaskStatus.Done);

            return $"Total tasks: {total}, Done tasks: {done}";
        }
    }
}
