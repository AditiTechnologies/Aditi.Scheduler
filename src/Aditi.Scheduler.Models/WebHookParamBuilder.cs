using System;
using System.Collections.Generic;

namespace Aditi.Scheduler.Models
{
    public class WebHookParamBuilder : IParamBuilder
    {
        private readonly  Dictionary<string, Object> _params = null; 
        

        public WebHookParamBuilder(string url)
        {
            _params = new Dictionary<string, object>
                {
                    {TaskParamKeys.Url, url}
                };
            
        }

        public WebHookParamBuilder ValidateSsl(bool validateSsl)
        {
            _params[TaskParamKeys.ValidateSsl] =  validateSsl;
            return this;
        }
        public WebHookParamBuilder Auth(string userName, string password)
        {
            var authParam = new Dictionary<string,string>
                {
                   {TaskParamKeys.Type, "basic"},
                   {TaskParamKeys.UserName, userName},
                   {TaskParamKeys.Password, password}
                };

            _params[TaskParamKeys.Auth] = authParam;
            
            return this;
        }
        public WebHookParamBuilder Post(ContentType type, string data)
        {
            _params[TaskParamKeys.Verb] = "POST";
              var contentParam = new Dictionary<string,string>
                {
                   {TaskParamKeys.DataType, type.ToString().ToLower()},
                   {TaskParamKeys.Data, data},
                };

            _params[TaskParamKeys.Content] = contentParam;
            return this;
        }
       
        public Dictionary<string, object> Build()
        {
            return this._params;
        }
    }
}