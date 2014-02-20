using System;

namespace Aditi.Scheduler.Models
{
    public class WebhookAudit
    {
        public string Id { get; set; }
        public int HttpStatusCode { get; set; }
        public string Response { get; set; }
        public Guid SubscriptionId { get; set; }
        public double ExecutionTimeMs { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public string Error { get; set; }
        public Guid TaskId { get; set; }
        public bool Success { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string JobName { get; set; }
    }
}
