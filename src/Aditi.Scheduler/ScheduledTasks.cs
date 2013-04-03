using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aditi.Scheduler.Models;
using Aditi.SignatureAuth;
using Newtonsoft.Json;
using ServiceStack.Text;


namespace Aditi.Scheduler
{
    public static class SchedulerConstants
    {
        public const string Modelstate = "ModelState";
        public const string RequestJsonContentType = "application/json";
        public const string ErrorMessage = "Message";
        //TODO: Change this to production URL when deploying? Can this be taken from any configuration file. 
        //for local 
#if !(DEBUG_BUILD || RELEASE_BUILD)
        public const string SchedulerTaskUri = "http://127.0.0.2/api/task/";
#endif
        //dev test
#if DEBUG_BUILD 
        public const string SchedulerTaskUri = "https://schedulerdev.aditicloud.com/";
#endif
#if RELEASE_BUILD
        public const string SchedulerTaskUri = "https://scheduler.aditicloud.com/";
#endif

    }

    public class ScheduledTasks
    {
        private readonly Uri _uri;
        private readonly string _tenantId;
        private readonly string _secretKey;
        
        private const int MaxRetryCount = 5;

        public ScheduledTasks(Uri uri, string tenantId, string secretKey)
        {
            this._uri = uri;
            this._tenantId = tenantId;
            this._secretKey = secretKey;
        
        }

        public ScheduledTasks(string tenantId, string secretKey)
        {
            this._uri = new Uri(SchedulerConstants.SchedulerTaskUri);
            this._tenantId = tenantId;
            this._secretKey = secretKey;
        
        }

        private HttpWebRequest CreateWebApiRequest(String relativeUri)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(_uri, relativeUri));
            request.ContentType = SchedulerConstants.RequestJsonContentType;
            request.Headers.Add("Authorization",
                                new Signature(this._tenantId, this._secretKey).ToString());
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            return request;
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static SchedulerException CreateSchedulerException(WebException we)
        {
            return new SchedulerModelValidationException(we.Message, we);
        }

        private static SchedulerException CreateSchedulerException(string responseMessage)
        {
            return new SchedulerModelValidationException(responseMessage);
        }

       
        private Guid GetOperationId(HttpWebResponse response)
        {
            string operationId = response.Headers["Location"].Split('/')[3];
            return Guid.Parse(operationId);
        }

        private Guid GetOperationId(HttpResponseMessage response)
        {
            var operationId = response.Headers.Location.ToString().Split('/')[3];
            return Guid.Parse(operationId);
        }

        private async Task<OperationStatus> PollAsync(string pollingUrl)
        {
            int pollCount = 0;
            OperationStatus operationStatus = null;
            //start polling
            while (pollCount != MaxRetryCount)
            {
                pollCount++;

                var pollingWebRequest = CreateWebApiRequest(pollingUrl);
                pollingWebRequest.Method = HttpMethod.Get.Method;

                try
                {
                    var polledWebResponse = (HttpWebResponse) await pollingWebRequest.GetResponseAsync();
                    using (var sr = new StreamReader(polledWebResponse.GetResponseStream()))
                    {
                        var jsonResponse = sr.ReadToEnd();
                        operationStatus = JsonConvert.DeserializeObject<OperationStatus>(jsonResponse);
                        if (operationStatus.Status != StatusCode.Pending)
                            return operationStatus;
                    }
                    Thread.Sleep(1000);
                }
                catch (WebException we)
                {
                    throw CreateSchedulerException(we);
                }
            }

            if (operationStatus != null && operationStatus.Status == StatusCode.Pending)
                operationStatus.Status = StatusCode.TimeOut;

            return operationStatus;
        }

        private OperationStatus Poll(string pollingUrl)
        {
            int pollCount = 0;
            OperationStatus operationStatus = null;
            //start polling
            while (pollCount != MaxRetryCount)
            {
                pollCount++;
                var pollingWebRequest = CreateWebApiRequest(pollingUrl);
                pollingWebRequest.Method = HttpMethod.Get.Method;

                try
                {
                    var polledWebResponse = (HttpWebResponse) pollingWebRequest.GetResponse();
                    using (var sr = new StreamReader(polledWebResponse.GetResponseStream()))
                    {
                        var jsonResponse = sr.ReadToEnd();
                        operationStatus = JsonConvert.DeserializeObject<OperationStatus>(jsonResponse);
                        if (operationStatus.Status != StatusCode.Pending)
                            return operationStatus;
                    }
                    Thread.Sleep(1000);
                }
                catch (WebException we)
                {
                    throw CreateSchedulerException(we);
                }
            }

            if (operationStatus != null && operationStatus.Status == StatusCode.Pending)
                operationStatus.Status = StatusCode.TimeOut;

            return operationStatus;
        }

        private HttpWebResponse GetResponse(HttpWebRequest clientRequest)
        {
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse) clientRequest.GetResponse();
            }
            catch (WebException exp)
            {
                throw CreateSchedulerException(exp);
            }
            return response;
        }

        public IEnumerable<TaskModel> GetTasks()
        {
            var request = CreateWebApiRequest("");
            request.Method = "GET";

            var response = (HttpWebResponse) request.GetResponse();

            string jsonResponse;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<TaskModel>>(jsonResponse);
        }

        public async Task<IEnumerable<TaskModel>> GetTasksAsync()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = await client.GetAsync(_uri);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<IEnumerable<TaskModel>>();
        }

        public TaskModel GetTask(Guid taskId)
        {
            var request = CreateWebApiRequest("/api/task/" + taskId.ToString());
            request.Method = HttpMethod.Get.Method;

            var response = (HttpWebResponse) request.GetResponse();

            string jsonResponse;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                jsonResponse = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TaskModel>(jsonResponse);
        }

        public async Task<TaskModel> GetTaskAsync(Guid taskId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());
            var response = await client.GetAsync(_uri + taskId.ToString());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<TaskModel>();
        }

        /// <exception cref="Aditi.Scheduler.SchedulerModelValidationException">Thrown when Task model is invalid</exception>
        /// <exception cref="Aditi.Scheduler.SchedulerException"></exception>
        public Guid CreateTask(TaskModel task)
        {
            var request = CreateWebApiRequest("/api/task");
            request.Method = HttpMethod.Post.Method;

            string json = JsonConvert.SerializeObject(task);

            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    HttpWebResponse response = GetResponse(request);
                    if (response.StatusCode == HttpStatusCode.Accepted)
                        return GetOperationId(response);
                }
            }
            catch (WebException we)
            {
                throw CreateSchedulerException(we);
            }

            //TODO: Is there any scenario code block will reach this?
            return Guid.Empty;
        }

        /// <exception cref="Aditi.Scheduler.SchedulerModelValidationException">Thrown when Task model is invalid</exception>
        /// <exception cref="Aditi.Scheduler.SchedulerException">T</exception>
        public async Task<Guid> CreateTaskAsync(TaskModel task)
        {
            var client = CreateHttpClient();

            var jsonFormatter = new JsonMediaTypeFormatter();
            var content = new ObjectContent<TaskModel>(task, jsonFormatter);
            var response = client.PostAsync(_uri.ToString() + task.Id.ToString(), content).Result;

           
            if (response.StatusCode == HttpStatusCode.Accepted)
                return GetOperationId(response);

            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //check for model state errors
                string responseMessage = await response.Content.ReadAsStringAsync();
                //Json Message received from server
                //{"Message":"The request is invalid.","ModelState":{"task.Start":["Time must be in the future"],
                //"task.End":["Time must be in the future"],"task.CronExpression":["Cron expression is invalid"]}}
                if (responseMessage.Contains(SchedulerConstants.Modelstate))
                {
                    throw CreateSchedulerException(responseMessage);
                }
                else
                {
                    throw new SchedulerException(responseMessage);
                }
            }

            //TODO: Is there any scenario code block will reach this?
            return Guid.Empty;
        }

        /// <exception cref="Aditi.Scheduler.SchedulerModelValidationException">Thrown when Task model is invalid</exception>
        /// <exception cref="Aditi.Scheduler.SchedulerException"></exception>
        public Guid UpdateTask(TaskModel task)
        {
            var request = CreateWebApiRequest("/api/task/" + task.Id.ToString());
            request.Method = HttpMethod.Put.Method;

            string json = JsonConvert.SerializeObject(task);

            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    //get response
                    HttpWebResponse response = GetResponse(request);

                    if (response.StatusCode == HttpStatusCode.Accepted)
                        return GetOperationId(response);
                }
            }
            catch (WebException we)
            {
                throw CreateSchedulerException(we);
            }


            //TODO: Is there any scenario code block will reach this?
            return Guid.Empty;
        }

        /// <exception cref="Aditi.Scheduler.SchedulerModelValidationException">Thrown when Task model is invalid</exception>
        /// <exception cref="Aditi.Scheduler.SchedulerException"></exception>
        public async Task<Guid> UpdateTaskAsync(TaskModel task)
        {
            var client = CreateHttpClient();

            var jsonFormatter = new JsonMediaTypeFormatter();
            var content = new ObjectContent<TaskModel>(task, jsonFormatter);
            var response = client.PutAsync(_uri.ToString() + task.Id.ToString(), content).Result;

            if (response.StatusCode == HttpStatusCode.Accepted)
                return GetOperationId(response);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //check for model state errors
                string responseMessage = await response.Content.ReadAsStringAsync();
                //Json Message received from server
                //{"Message":"The request is invalid.","ModelState":{"task.Start":["Time must be in the future"],
                //"task.End":["Time must be in the future"],"task.CronExpression":["Cron expression is invalid"]}}
                if (responseMessage.Contains(SchedulerConstants.Modelstate))
                {
                    throw CreateSchedulerException(responseMessage);
                }
                else
                {
                    throw new SchedulerException(responseMessage);
                }
            }

            //TODO Is there any scenario code block will reach this?
            return Guid.Empty;
        }

        /// <exception cref="Aditi.Scheduler.SchedulerException"></exception>
        public Guid DeleteTask(Guid taskId)
        {
            var request = CreateWebApiRequest("/api/task" + taskId.ToString("N"));
            request.Method = HttpMethod.Delete.Method;

            HttpWebResponse response = GetResponse(request);
            return response.StatusCode == HttpStatusCode.Accepted ? GetOperationId(response) : Guid.Empty;
        }

        /// <exception cref="Aditi.Scheduler.SchedulerException"></exception>
        public async Task<Guid> DeleteTaskAsync(Guid taskId)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = client.DeleteAsync(_uri + taskId.ToString()).Result;

            if (response.StatusCode == HttpStatusCode.Accepted)
                return GetOperationId(response);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //check for model state errors
                string responseMessage = await response.Content.ReadAsStringAsync();
                throw CreateSchedulerException(responseMessage);
            }

            //TODO Is there any scenario code block will reach this?
            return Guid.Empty;
        }

        public async Task<OperationStatus> GetOperationStatusAsync(Guid operationId, bool blocked = false)
        {
            OperationStatus operationStatus = null;
            var statusUrl = "/api/Status/" + operationId.ToString();

            var statusWebRequest = CreateWebApiRequest(statusUrl);
            statusWebRequest.Method = HttpMethod.Get.Method;
            try
            {
                var statusWebResponse = (HttpWebResponse) await statusWebRequest.GetResponseAsync();
                using (var sr = new StreamReader(statusWebResponse.GetResponseStream()))
                {
                    var jsonResponse = await sr.ReadToEndAsync();
                    operationStatus = JsonConvert.DeserializeObject<OperationStatus>(jsonResponse);
                }
            }
            catch (WebException we)
            {
                CreateSchedulerException(we);
            }

            if (operationStatus != null && operationStatus.Status == StatusCode.Pending)
            {
                if (blocked)
                    operationStatus = await PollAsync(statusUrl);
            }
            if (operationStatus != null  && operationStatus.Data != null)
                operationStatus.Data = JsonObject.Parse(operationStatus.Data.ToString());

            return operationStatus;
        }
        
        public OperationStatus GetOperationStatus(Guid operationId, bool blocked = false)
        {
            OperationStatus operationStatus = null;
            var statusUrl = "/api/Status/" + operationId.ToString();

            var statusWebRequest = CreateWebApiRequest(statusUrl);
            statusWebRequest.Method = HttpMethod.Get.Method;
            try
            {
                 var statusWebResponse = (HttpWebResponse) statusWebRequest.GetResponse();
                using (var sr = new StreamReader(statusWebResponse.GetResponseStream()))
                {
                    var jsonResponse = sr.ReadToEnd();
                    operationStatus = JsonConvert.DeserializeObject<OperationStatus>(jsonResponse);
                }
            }
            catch (WebException we)
            {
                throw CreateSchedulerException(we);
            }
            if (operationStatus != null && operationStatus.Status == StatusCode.Pending)
            {
                if (blocked)
                    operationStatus = Poll(statusUrl);
            }
           
            if(operationStatus != null && (operationStatus.Status == StatusCode.Success || operationStatus.Status == StatusCode.Error) )
                operationStatus.Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(operationStatus.Data.ToString());

            return operationStatus;
        }
      
        public WebhookAuditResult GetTaskHistory(Guid taskId, string token = null)
        {
            WebhookAuditResult historyResult;
            string historyUrl = "/api/task/" + taskId.ToString() + "/History/";
            var historyWebRequest = CreateWebApiRequest(historyUrl);
            historyWebRequest.Method = HttpMethod.Get.Method;

            try
            {
                var historyWebResponse = (HttpWebResponse)historyWebRequest.GetResponse();
                using (var sr = new StreamReader(historyWebResponse.GetResponseStream()))
                {
                    var jsonResponse = sr.ReadToEnd();
                    historyResult = JsonConvert.DeserializeObject<WebhookAuditResult>(jsonResponse);
                }
            }
            catch (WebException webExp)
            {
                throw CreateSchedulerException(webExp);
            }
            return historyResult;
        }

        public async Task<WebhookAuditResult> GetTaskHistoryAsync(string taskId, string token = null)
        {
            WebhookAuditResult historyResult;
            
            string historyUrl = string.Concat(SchedulerConstants.SchedulerTaskUri, taskId, "/History/");
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());
            var response = await client.GetAsync(historyUrl);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<WebhookAuditResult>();
        }

    }
}
