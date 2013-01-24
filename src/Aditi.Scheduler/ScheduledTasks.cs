using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Aditi.Scheduler.Models;
using Aditi.SignatureAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Aditi.Scheduler
{
    public class ScheduledTasks
    {
        private readonly Uri _uri;
        private readonly string _tenantId;
        private readonly string _secretKey;

        public ScheduledTasks(Uri uri, string tenantId, string secretKey)
        {
            this._uri = uri;
            this._tenantId = tenantId;
            this._secretKey = secretKey;
        }

        public ScheduledTasks(string tenantId, string secretKey)
        {
            this._uri = new Uri("http://scheduler.aditicloud.com/api/task/");
            this._tenantId = tenantId;
            this._secretKey = secretKey;
        }

        public IEnumerable<TaskModel> GetTasks()
        {
            var request = (HttpWebRequest)WebRequest.Create(_uri);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = (HttpWebResponse)request.GetResponse();

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
            var request = (HttpWebRequest)WebRequest.Create(_uri + taskId.ToString());

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = (HttpWebResponse)request.GetResponse();

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

        public TaskModel CreateTask(TaskModel task)
        {
            var request = (HttpWebRequest)WebRequest.Create(_uri);
            request.Method = "POST";

            request.ContentType = "application/json";
            request.Headers.Add("Authorization",
                                new Signature(this._tenantId, this._secretKey).ToString());

            string json = JsonConvert.SerializeObject(task);

            string jsonResponse;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = streamReader.ReadToEnd();
                }
            }

            return JsonConvert.DeserializeObject<TaskModel>(jsonResponse);
        }

        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonFormatter = new JsonMediaTypeFormatter();
            var content = new ObjectContent<TaskModel>(task, jsonFormatter);
            var response = client.PostAsync(_uri, content).Result;

            return await response.Content.ReadAsAsync<TaskModel>();
        }

        public TaskModel UpdateTask(TaskModel task)
        {
            var request = (HttpWebRequest)WebRequest.Create(_uri.ToString() + task.Id.ToString());
            request.Method = "PUT";

            request.ContentType = "application/json";
            request.Headers.Add("Authorization",
                                new Signature(this._tenantId, this._secretKey).ToString());

            string json = JsonConvert.SerializeObject(task);

            string jsonResponse;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = streamReader.ReadToEnd();
                }
            }

            return JsonConvert.DeserializeObject<TaskModel>(jsonResponse);
        }

        public async Task<TaskModel> UpdateTaskAsync(TaskModel task)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonFormatter = new JsonMediaTypeFormatter();
            var content = new ObjectContent<TaskModel>(task, jsonFormatter);
            var response = client.PutAsync(_uri.ToString() + task.Id.ToString(), content).Result;

            return await response.Content.ReadAsAsync<TaskModel>();
        }

        public string DeleteTask(Guid taskId)
        {
            var request = (HttpWebRequest)WebRequest.Create(_uri + taskId.ToString());

            request.Method = "DELETE";
            request.Headers.Add("Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = (HttpWebResponse)request.GetResponse();

            string result;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        public async Task<string> DeleteTaskAsync(Guid taskId)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = client.DeleteAsync(_uri + taskId.ToString()).Result;

            return await response.Content.ReadAsStringAsync();
        }
    }
}
