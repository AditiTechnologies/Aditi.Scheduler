using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aditi.Scheduler;
using Aditi.Scheduler.Models;



namespace TestNugetConsole
{
    class NugetClient
    {
        static ScheduledTasks scheduledTask = new ScheduledTasks("43336f8e-fa22-4251-bd49-447eeb10f5ab",
                                                             "/HUccm4Bxnsy9UkYIVxijVJ8qjnk1vQeZ0FFT+/uKyE=");
        static  void Main(string[] args)
        {
          
            var tasks = new TaskModel[5];

            tasks = GetTasks();

            //valid task[0]
            //valid task[1]
            //invalid task[2]
            //invalid task[3]
            //valid task[4]
            
            var list = new List<Guid>();
            var listOpStatus = new List<OperationStatus>();
            OperationStatus status;

            for (int i = 0; i < tasks.Length; i++)
            {
                try
                {
                    var op = scheduledTask.CreateTask(tasks[i]);
                    list.Add(op);
                }
                catch (SchedulerException mve)
                {
                    //display error with model
                }
            }
        
            GetOperationStatus(list);
        
        }

        private static TaskModel[] GetTasks()
        {
            var tasks = new TaskModel[5];

            //valid
            tasks[0] = new TaskModel
            {
                Name = "ZZ first Nuget Task",
                JobType = JobType.Webhook,
                CronExpression = "0 0 12 1/1 * ? *", // run every 5 minutes; http://cronmaker.com/
                Params = new Dictionary<string, object>
                {
                    {"url", "http:/local/"}
                }
            };

            //valid
            tasks[1] = new TaskModel
            {
                Name = "ZZ second Nuget Task",
                JobType = JobType.Webhook,
                CronExpression = "0 0 12 1/1 * ? *", // run every 5 minutes; http://cronmaker.com/
                Params = new Dictionary<string, object>
                {
                    {"url", "http:/local/"}
                }
            };

            //invalid
            tasks[2] = new TaskModel
            {
                Name = "ZZ third Nuget Task",
                JobType = JobType.Webhook,
                CronExpression = "ABCD", // run every 5 minutes; http://cronmaker.com/
                Params = new Dictionary<string, object>
                {
                    {"url", "http:/local/"}
                },
                Start = DateTime.Now,
                End = DateTime.Now.Subtract(new TimeSpan(3,0,0))
                
            };

            //invalid
            tasks[3] = new TaskModel
            {
                Name = "",
                JobType = JobType.Webhook,
                CronExpression = "ABCD", // run every 5 minutes; http://cronmaker.com/
                Params = new Dictionary<string, object>
                {
                    {"url", "http:/local/"}
                }
            };

            //valid
            tasks[4] = new TaskModel
            {
                Name = "ZZ fifth Nuget Task",
                JobType = JobType.Webhook,
                CronExpression = "0 0 12 1/1 * ? *", // run every 5 minutes; http://cronmaker.com/
                Params = new Dictionary<string, object>
                {
                    {"url", "http:/local/"}
                }
            };

            return tasks;
        }


        private static async void GetOperationStatus(List<Guid> list)
        {
            OperationStatus[] operationStatus;
            //1. list of operationIds
            //this is contained in list

            //2. Create a query. 
            IEnumerable<Task<OperationStatus>> getOperationStatusQuery = from opId in list
                                                                         select scheduledTask.GetOperationStatusAsync(opId);

            //3. Use ToArray to execute the query and start the download tasks.
            Task<OperationStatus>[] getOperationStatusTasks = getOperationStatusQuery.ToArray();

            // You can do other work here before awaiting. 

            try
            {
                // Await the completion of all the running tasks. 
                operationStatus = await Task.WhenAll(getOperationStatusTasks);

                //// The previous line is equivalent to the following two statements. 
                //Task<OperationStatus[]> whenAllTask = Task.WhenAll(getOperationStatusTasks);
                //OperationStatus[] operationStatus = await whenAllTask; 

                //display Status of all Tasks
            }
            catch (Exception exp)
            {
                throw;
            }
            

            for (int startCount = 0; startCount < operationStatus.Length; startCount++)
            {
                Console.WriteLine(string.Format("OrrationID: {0}    Status: {1}", operationStatus[startCount].Id, operationStatus[startCount].Status));
            }
       }

      
    }
}
