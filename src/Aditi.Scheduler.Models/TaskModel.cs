using System;
using System.Collections.Generic;

namespace Aditi.Scheduler.Models
{
    public class TaskModel
    {
        public TaskModel()
        {
            Enabled = true;
            TimeZoneId = TimeZoneInfo.Utc.Id;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public JobType JobType { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int? RepeatEveryMins { get; set; }
        public string CronExpression { get; set; }
        public bool Enabled { get; set; }
        public string TimeZoneId { get; set; }
        public Dictionary<string, object> Params { get; set; }

    }
}
