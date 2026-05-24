using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Manager.TaskManager.Core
{
    public class TaskService : ITaskRepository
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

        public void Add(TaskItem task)
        {
            _validator.Validate(task);
            _repository.Add(task);
            _notifier[0].Notify(task);
        }

        public void Delete(TaskItem task)
        {
            _repository.Delete(task);
        }

        public void Update(TaskItem task)
        {
            _repository.Update(task);
        }

        public TaskItem GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<TaskItem> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
