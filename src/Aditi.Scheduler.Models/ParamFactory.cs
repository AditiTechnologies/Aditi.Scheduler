using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aditi.Scheduler.Models;
namespace Aditi.Scheduler.Models
{
    public static class ParamFactory
    {

        public static WebHookParamBuilder CreateWebHookParamBuilder(string url)
        {
            return new WebHookParamBuilder(url);
        }

        public static AzureQueueParamBuilder CreateAzureQueueParamBuilder(string accountName, string queueName, string sasToken)
        {
            return new AzureQueueParamBuilder(accountName, queueName, sasToken);
        }

    }
   
}
