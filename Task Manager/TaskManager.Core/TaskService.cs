using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class TaskService 
    {
        public enum NotificationType
        {
            Email,
            Console,
            FileLog,
            Slack
        }

        private readonly ITaskRepository _repository;
        private readonly TaskValidator _validator;
        private readonly IReadOnlyDictionary<NotificationType, ITaskNotifier> _notifier;

        public TaskService(ITaskRepository repository, TaskValidator validator, IReadOnlyDictionary<NotificationType, ITaskNotifier> notifier)
        {
            _repository = repository;
            _validator = validator;
            _notifier = notifier;
        }

    }
}
