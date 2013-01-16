using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aditi.Scheduler.Models;
using Aditi.SignatureAuth;

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

        public async Task<IEnumerable<ScheduledTask>> GetTasks()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = await client.GetAsync(_uri);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<IEnumerable<ScheduledTask>>();
        }

        public async Task<ScheduledTask> GetTask(Guid taskId)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            var response = await client.GetAsync(_uri + taskId.ToString());

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<ScheduledTask>();
        }

        public async Task<ScheduledTask> CreateTask(ScheduledTask task)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                new Signature(this._tenantId, this._secretKey).ToString());

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonFormatter = new JsonMediaTypeFormatter();
            var content = new ObjectContent<ScheduledTask>(task, jsonFormatter);
            var response = client.PostAsync(_uri, content).Result;

            return await response.Content.ReadAsAsync<ScheduledTask>();
        }

        public async Task<string> DeleteTask(Guid taskId)
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
