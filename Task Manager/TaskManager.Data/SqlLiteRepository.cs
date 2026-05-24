using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Manager.TaskManager.Core;
using Microsoft.Data.Sqlite;
namespace Task_Manager.TaskManager.Data
{
    public class SqlLiteRepository : ITaskRepository
    {
        private readonly string _connectionstring = "Data Source = task.db";

        public SqlLiteRepository()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using(var connection =  new SqliteConnection(_connectionstring))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                    @"CREATE TABLE IF NOT EXISTS Tasks(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        Status TEXT NOT NULL,
                        Priority INTEGER NOT NULL,
                        TaskType TEXT NOT NULL,
                        NotificationType TEXT NOT NULL,        
                        DueDate DATETIME NULL,
                        RecurrenceInterval INTEGER NULL,
                        CreatedAt TEXT NOT NULL
                    );
                ";

                command.ExecuteNonQuery();
            }
        }

        public void Add(TaskItem task)
        {
            using (var connection = new SqliteConnection(_connectionstring))
            {
                connection.Open();

                var command = connection.CreateCommand();

                string taskType = "Standard";
                DateTime? duedate = null;

                if(task is DeadlineTask deadlineTask)
                {
                    taskType = "Deadline";
                    duedate = deadlineTask.DueDate;
                }

                else if(task is RecurringTask)
                {
                    taskType = "Recurring";
                }

                command.CommandText =
                    @"
                        INSERT INTO Tasks(Title, TaskType, DueDate)
                        VALUES($title, $type, $dueDate)
                     ";

                command.Parameters.AddWithValue("$title", task.Title);
                command.Parameters.AddWithValue("$type", taskType);

                command.Parameters.AddWithValue("$dueDate", duedate.HasValue ? duedate.Value.ToString("o") : (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }
        public void Update(TaskItem task)
        {
            using (var connection = new SqliteConnection(_connectionstring))
            {
                connection.Open();

                var command = connection.CreateCommand();

                string taskType = "Standard";
                DateTime? dueDate = null;

                if (task is DeadlineTask deadlineTask)
                {
                    taskType = "Deadline";
                    dueDate = deadlineTask.DueDate;
                }
                else if (task is RecurringTask)
                {
                    taskType = "Recurring";
                }

                command.CommandText =
                @"
            UPDATE Tasks
            SET Title = $title,
                TaskType = $type,
                DueDate = $dueDate,
                Status = $status,
                Priority = $priority,
                NotificationType = $notificationType
            WHERE Id = $id
        ";

                command.Parameters.AddWithValue("$id", task.Id);
                command.Parameters.AddWithValue("$title", task.Title);
                command.Parameters.AddWithValue("$type", taskType);
                command.Parameters.AddWithValue("$status", task.Status.ToString());
                command.Parameters.AddWithValue("$priority", (int)task.Priority);
                command.Parameters.AddWithValue("$notificationType", task.NotificationType.ToString());

                command.Parameters.AddWithValue("$dueDate",
                    dueDate.HasValue ? dueDate.Value.ToString("o") : (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(TaskItem task)
        {
            using (var connection = new SqliteConnection(_connectionstring))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                @"
            DELETE FROM Tasks
            WHERE Id = $id
        ";

                command.Parameters.AddWithValue("$id", task.Id);

                command.ExecuteNonQuery();
            }
        }

        public TaskItem GetById(int id)
        {
            using (var connection = new SqliteConnection(_connectionstring))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                @"
            SELECT *
            FROM Tasks
            WHERE Id = $id
        ";

                command.Parameters.AddWithValue("$id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    string taskType = reader.GetString(reader.GetOrdinal("TaskType"));
                    string title = reader.GetString(reader.GetOrdinal("Title"));

                    TaskItem task;

                    if (taskType == "Deadline")
                    {
                        DateTime dueDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("DueDate")));
                        task = new DeadlineTask(title, dueDate);
                    }
                    else if (taskType == "Recurring")
                    {
                        task = new RecurringTask(title);
                    }
                    else
                    {
                        task = new TaskItem(title);
                    }

                    task.Id = reader.GetInt32(reader.GetOrdinal("Id"));

                    return task;
                }
            }
        }

        public IEnumerable<TaskItem> GetAll()
        {
            var tasks = new List<TaskItem>();

            using (var connection = new SqliteConnection(_connectionstring))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                @"
            SELECT *
            FROM Tasks
        ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string taskType = reader.GetString(reader.GetOrdinal("TaskType"));
                        string title = reader.GetString(reader.GetOrdinal("Title"));

                        TaskItem task;

                        if (taskType == "Deadline")
                        {
                            DateTime dueDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("DueDate")));
                            task = new DeadlineTask(title, dueDate);
                        }
                        else if (taskType == "Recurring")
                        {
                            task = new RecurringTask(title);
                        }
                        else
                        {
                            task = new TaskItem(title);
                        }

                        task.Id = reader.GetInt32(reader.GetOrdinal("Id"));

                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }
    }
}
