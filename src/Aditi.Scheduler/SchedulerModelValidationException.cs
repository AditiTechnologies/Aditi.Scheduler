using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ServiceStack.Text;

namespace Aditi.Scheduler
{
    public  class SchedulerModelValidationException : SchedulerException
    {
        public Dictionary<string, string> ModelValidationErrors { get; private set; }

        public SchedulerModelValidationException(string message, WebException we)
            : base(message, we)
        {
            object responseData;
            var response = (System.Net.HttpWebResponse)(we.Response);
            if (response == null) return;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseData = JsonObject.Parse(sr.ReadToEnd());
            }
            if (responseData != null && ((System.Collections.Generic.Dictionary<string, string>)responseData).ContainsKey(SchedulerConstants.Modelstate))
            {
                ValidationErrors(((System.Collections.Generic.Dictionary<string, string>)responseData)[SchedulerConstants.Modelstate]);
            }
        }

        public SchedulerModelValidationException(string response)
        {
            if (string.IsNullOrEmpty(response))
                return;

            object responseData = JsonObject.Parse(response);
            if (responseData != null && ((System.Collections.Generic.Dictionary<string, string>)responseData).ContainsKey(SchedulerConstants.Modelstate))
            {
                ValidationErrors(((System.Collections.Generic.Dictionary<string, string>)responseData)[SchedulerConstants.Modelstate]);
            }
        }

        private void ValidationErrors(string jsonModelErrors)
        {
            object errors;
            //jSon Message  The request is invalid
            //Json Error ModelState {"task.Start":["Time must be in the future"],"task.End":["Time must be in the future"],
            //"task.CronExpression":["Cron expression is invalid"]}
            errors = JsonObject.Parse(jsonModelErrors);
            ModelValidationErrors =  ((System.Collections.Generic.Dictionary<string, string>) errors);
        }

        
    }
}
