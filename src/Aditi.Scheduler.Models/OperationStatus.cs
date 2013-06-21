using System;

namespace Aditi.Scheduler.Models
{
    public enum StatusCode
    {
        Pending,
        Success,
        TimeOut,
        Error
    }
    public class OperationStatus
    {
        public Guid Id;
        public string Location;
        public object Data;
        public StatusCode Status;
    }
}
