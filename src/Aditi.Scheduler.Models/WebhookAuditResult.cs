using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aditi.Scheduler.Models
{
    public class WebhookAuditResult
    {
        public List<WebhookAudit> Audits { get; set; }
        public string NextToken { get; set; }
        public bool HasMore { get; set; }
    }
}
