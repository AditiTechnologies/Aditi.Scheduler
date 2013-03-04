using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aditi.Scheduler;
using Aditi.Scheduler.Models;

namespace NugetWPFAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScheduledTasks scheduledTask;

        public MainWindow()
        {
            InitializeComponent();
            scheduledTask = new ScheduledTasks("43336f8e-fa22-4251-bd49-447eeb10f5ab",
                                                          "/HUccm4Bxnsy9UkYIVxijVJ8qjnk1vQeZ0FFT+/uKyE=");
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Clear();

            txtStatus.Text += "task creation started";
            List<Guid> opIds =  await CreatTasksAsync();

            txtStatus.Text += "Noew getting status for operation ids";
            List<OperationStatus> opStatus =  await GetOperationStatusAsync(opIds);
            
           // txtStatus.Text += "\r\nControl returned to startButton_Click.\r\n";
            
        }

        private async Task<List<Guid>> CreatTasksAsync()
        {
            List<Guid> operationIds = null;
            Guid currentOperationId;
            var tasks = new TaskModel[5];
            tasks = GetTasks();

            //Synchronous Create task
            try
            {
                scheduledTask.CreateTask(tasks[2]);
            }
            catch (SchedulerModelValidationException exp)
            {
                throw exp;
            }


            ////asynchronous
            //if (tasks.Length > 0)
            //{
            //    operationIds = new List<Guid>();
            //    for (int taskCount = 0; taskCount < tasks.Length; taskCount++)
            //    {
            //        try
            //        {
            //            currentOperationId = await scheduledTask.CreateTaskAsync(tasks[taskCount]);
            //            operationIds.Add(currentOperationId);
            //        }
            //        catch (SchedulerModelValidationException exp)
            //        {
            //            //write to log
            //        }
            //        catch (SchedulerException exp)
            //        {
            //            //write to log
            //        }

            //    }

            //}

            

           // //1. list of taskModels
           // //this is contained in tasks

           // //2. Create a query. 
           // IEnumerable<Task<Guid>> createSchedTasksQuery = from task in tasks
           //                                                 select scheduledTask.CreateTaskAsync(task);

           // try
           // {
           //     //3. Use ToArray to execute the query and start creating tasks.
           //     Task<Guid>[] createSchedTasks = createSchedTasksQuery.ToArray();
           //     // You can do other work here before awaiting.

           //     // Await the completion of all the running tasks. 
           //     operationIds = await Task.WhenAll(createSchedTasks);

           // }
           // catch (SchedulerException exp)
           // {

           // }

           // //// The previous line is equivalent to the following two statements. 
           // //Task<OperationStatus[]> whenAllTask = Task.WhenAll(getOperationStatusTasks);
           // //OperationStatus[] operationStatus = await whenAllTask; 
           // txtStatus.Text += "\nGot OperationIDs";
           //// txtStatus.Text += string.Format("\n{0}\n{1}\n{2}", operationIds[0], operationIds[1], operationIds[2]);

            return operationIds;

        }

        private async Task<List<OperationStatus>> GetOperationStatusAsync(List<Guid> opIds)
        {
            OperationStatus currentStatus;
            List<OperationStatus> opStatus = new List<OperationStatus>();

            for (int operationCount = 0; operationCount < opIds.Count; operationCount++)
            {
                try
                {
                    currentStatus = await scheduledTask.GetOperationStatusAsync(opIds[operationCount],true);
                    opStatus.Add(currentStatus);
                }
                catch (SchedulerException exp)
                {
                    //write log
                }
            }


            ////1. list of operatioIds
                ////this is contained in opId array

                ////2. Create a query. 
                //IEnumerable<Task<OperationStatus>> getOperationStatusQuery = from opId in opIds
                //                                                           select scheduledTask.GetOperationStatusAsync(opId);

                ////3. Use ToArray to execute the query and start creating tasks.
                //Task<OperationStatus>[] getOperationStatus = getOperationStatusQuery.ToArray();

                //// You can do other work here before awaiting.

                //// Await the completion of all the running tasks. 
                //opStatus = await Task.WhenAll(getOperationStatus);

                ////// The previous line is equivalent to the following two statements. 
                ////Task<OperationStatus[]> whenAllTask = Task.WhenAll(getOperationStatusTasks);
                ////OperationStatus[] operationStatus = await whenAllTask; 

                //for (int startCount = 0; startCount < opIds.Length; startCount++)
                //{
                //    txtStatus.Text += string.Format("\n{0}:{1}", opIds[startCount], opStatus[startCount].Status);
                //}

                return opStatus;
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
                End = DateTime.Now.Subtract(new TimeSpan(3, 0, 0))

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
    }
}
