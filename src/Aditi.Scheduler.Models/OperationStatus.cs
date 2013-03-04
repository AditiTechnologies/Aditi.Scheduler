using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
