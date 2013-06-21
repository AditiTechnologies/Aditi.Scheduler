using System.Collections.Generic;

namespace Aditi.Scheduler.Models
{
    public class WebhookAuditResult
    {
        public List<WebhookAudit> Audits { get; set; }
        public string NextToken { get; set; }
        public bool HasMore { get; set; }
    }
}
