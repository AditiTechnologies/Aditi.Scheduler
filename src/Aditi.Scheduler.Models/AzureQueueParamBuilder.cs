using System;
using System.Collections.Generic;

namespace Aditi.Scheduler.Models
{
    public class AzureQueueParamBuilder:IParamBuilder
    {
        private readonly Dictionary<string, Object> _params = null; 

        public AzureQueueParamBuilder(string accountName, string queueName, string sasToken)
        {
            _params = new Dictionary<string, object>
                {
                    {TaskParamKeys.Verb, "POST"},
                    {TaskParamKeys.AccountName, accountName},
                    {TaskParamKeys.QueueName, queueName},
                    {TaskParamKeys.SasToken, sasToken}
                    
                };
        }
        public AzureQueueParamBuilder QueueMessage(string queueMessage)
        {
            _params.Add(TaskParamKeys.QueueMessage, queueMessage);
            return this;
        }
        public AzureQueueParamBuilder IsBinary(bool isBinary)
        {
            _params.Add(TaskParamKeys.IsBinary, isBinary);
            return this;
        }
        public Dictionary<string, object> Build()
        {
            return _params;
        }
    }
}
