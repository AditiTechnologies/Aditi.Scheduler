namespace Aditi.Scheduler.Models
{
    public static class ParamBuilderFactory
    {

        public static WebHookParamBuilder GetParamBuilder(string url)
        {
            return new WebHookParamBuilder(url);
        }

        public static AzureQueueParamBuilder GetParamBuilder(string accountName, string queueName, string sasToken)
        {
            return new AzureQueueParamBuilder(accountName, queueName, sasToken);
        }

    }
   
}
