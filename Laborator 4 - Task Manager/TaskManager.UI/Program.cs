using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task_Manager.TaskManager.Core;
using Task_Manager.TaskManager.Data;

namespace Task_Manager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            public var services;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            services.AddSingleton<SqlLiteRepository>();

            services.AddSingleton<ITaskRepository>(sp =>
                sp.GetRequiredService<SqlLiteRepository>());

            services.AddSingleton<ITaskReader>(sp =>
                sp.GetRequiredService<SqlLiteRepository>());

            services.AddTransient<TaskValidator>();
            services.AddTransient<TaskService>();
            services.AddTransient<ReportService>();
            services.AddNotifiers();
        }
    }
}
