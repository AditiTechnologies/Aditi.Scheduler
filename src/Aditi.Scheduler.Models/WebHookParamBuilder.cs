using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aditi.Scheduler.Models
{
    public class WebHookParamBuilder : IParamBuilder
    {
        private readonly  Dictionary<string, Object> _params = null; 
        

        public WebHookParamBuilder(string url)
        {
            _params = new Dictionary<string, object>
                {
                    {ParamBuilderConstants.Url, url}
                };
            
        }

        public WebHookParamBuilder Verb(HttpVerb verb)
        {
            _params.Add(ParamBuilderConstants.Verb, Enum.GetName(typeof(HttpVerb), verb));
            return this;
        }
        public WebHookParamBuilder ValidateSsl(bool validateSsl)
        {
            _params.Add(ParamBuilderConstants.ValidateSsl, validateSsl);
            return this;
        }
        public WebHookParamBuilder Auth(string userName, string password)
        {
            var authParam = new Dictionary<string,string>
                {
                   {ParamBuilderConstants.Type, Enum.GetName(typeof(AuthType), AuthType.Basic)},
                   {ParamBuilderConstants.UserName, userName},
                   {ParamBuilderConstants.Password, password}
                };

            _params.Add(ParamBuilderConstants.Auth, authParam);
            
            return this;
        }
        public WebHookParamBuilder Content(ContentType type, string data)
        {
              var contentParam = new Dictionary<string,string>
                {
                   {ParamBuilderConstants.DataType, Enum.GetName(typeof(ContentType), type).ToLower()},
                   {ParamBuilderConstants.Data, data},
                };

            _params.Add(ParamBuilderConstants.Content, contentParam);
            return this;
        }
       
        public Dictionary<string, object> Build()
        {
            return this._params;
        }
    }
}