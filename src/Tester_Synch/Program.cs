using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aditi.Scheduler;
using Aditi.Scheduler.Models;

namespace Tester_Synch
{
    class Program
    {
        private static string tenantId = "0007057b-0d06-4825-a812-42b1d896cc95";
        private static string secretKey = "nQOSFjqiEwIrymp5g+iaDRx1UmUgM+UMeOP9MkTaw0k=";
        private static Uri uri = new Uri("http://schedulerdev.aditicloud.com/api/task/");

        static void Main(string[] args)
        {
            var scheduledTasks = new ScheduledTasks(uri, tenantId, secretKey);

            // create a task
            var task = new TaskModel
            {
                Name = "My first Scheduler job",
                JobType = JobType.Webhook,
                CronExpression = "0 0/5 * 1/1 * ? *", // run every 5 minutes
                Params = new Dictionary<string, object>
                {
                    {"url", "http://www.microsoft.com"}
                }
            };

            var newTask = scheduledTasks.CreateTask(task);

            var tasks = scheduledTasks.GetTasks();

            var getTask = scheduledTasks.GetTask(newTask.Id);

            newTask.Name = "new name";

            var updTask = scheduledTasks.UpdateTask(newTask);

            var delTask = scheduledTasks.DeleteTask(newTask.Id);

        }
    }
}
