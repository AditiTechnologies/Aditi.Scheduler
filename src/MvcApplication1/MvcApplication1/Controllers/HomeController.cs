using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Aditi.Scheduler;
using Aditi.Scheduler.Models;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private Uri uri = new Uri("http://devschedule.cloudapp.net/api/task/");
        private string tenantId = "62022e61-1bd5-4361-bd0d-dff613bbc2dd";
        private string secretKey = "BWDlg/thd6evmgF256bBJw9HT8KSIqQjAWR/pPqPDUE=";

        public async Task<ActionResult> Index()
        {
            await GetTasks();

            var task = new TaskModel
            {
                Name = "Wade.Test",
                JobType = JobType.Webhook,
                CronExpression = "0 0/1 * 1/1 * ? *",
                Params = new Dictionary<string, object>
                {
                    {"url", "http://www.microsoft.com"}
                }
            };

            //var response = await tasks.CreateTask(task);

            //var id = response.Id;

            //var o2 = await tasks.GetTasks();

            //var o3 = await tasks.DeleteTask(id);

            //var o4 = await tasks.GetTasks();

            return View();
        }

        private async Task<IEnumerable<TaskModel>> GetTasks()
        {
            var tasks = new Tasks(uri, tenantId, secretKey);
            var response = await tasks.GetTasks();
           
            return response;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
