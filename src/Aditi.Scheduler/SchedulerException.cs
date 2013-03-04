using System;
using System.Runtime.Serialization;

namespace Aditi.Scheduler
{
   public class SchedulerException :Exception 
    {

        public SchedulerException()
            : base()
        {
            // Add implementation.
        }

        public SchedulerException(string message)
            : base(message)
        {
            // Add implementation.
        }
        public SchedulerException(string message, Exception inner)
            : base(message, inner)
        {
            // Add implementation.
        }

        // This constructor is needed for serialization.
        protected SchedulerException(SerializationInfo info, StreamingContext context)
        {
            // Add implementation.
        }

 
    }
}
