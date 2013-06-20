using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aditi.Scheduler.Models
{
    public class AzureQueueParamBuilder:IParamBuilder
    {
        private readonly Dictionary<string, Object> _params = null; 

        public AzureQueueParamBuilder(string accountName, string queueName, string sasToken)
        {
            _params = new Dictionary<string, object>
                {
                    {ParamBuilderConstants.Verb, Enum.GetName(typeof(HttpVerb), HttpVerb.POST)},
                    {ParamBuilderConstants.AccountName, accountName},
                    {ParamBuilderConstants.QueueName, queueName},
                    {ParamBuilderConstants.SasToken, sasToken}
                    
                };
        }
        public AzureQueueParamBuilder QueueMessage(string queueMessage)
        {
            _params.Add(ParamBuilderConstants.QueueMessage, queueMessage);
            return this;
        }
        public AzureQueueParamBuilder IsBinary(bool isBinary)
        {
            _params.Add(ParamBuilderConstants.IsBinary, isBinary);
            return this;
        }
        public Dictionary<string, object> Build()
        {
            return _params;
        }
    }
}
