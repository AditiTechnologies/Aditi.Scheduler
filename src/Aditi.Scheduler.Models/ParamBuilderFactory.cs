namespace Aditi.Scheduler.Models
{
    public static class ParamBuilderFactory
    {

        public static WebHookParamBuilder WebHookBuilder(string url)
        {
            return new WebHookParamBuilder(url);
        }

        public static AzureQueueParamBuilder AzureQueueBuilder(string accountName, string queueName, string sasToken)
        {
            return new AzureQueueParamBuilder(accountName, queueName, sasToken);
        }

    }
   
}
