using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Newtonsoft.Json;

using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.CastHelpers.Converters;
using ARSnovaPPIntegration.Communication.CastHelpers.Models;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaClickService : IArsnovaClickService
    {
        private readonly string apiUrl;

        private readonly ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel> answerOptionMapper;

        private readonly ObjectMapper<SessionConfigurationWithId, SessionConfiguration> sessionConfigurationMapper;

        public ArsnovaClickService()
        {
            this.answerOptionMapper = new ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel>();

            this.sessionConfigurationMapper = new ObjectMapper<SessionConfigurationWithId, SessionConfiguration>();

            #if DEBUG
                this.apiUrl = "http://localhost:3000/api/";
            #else
                this.apiUrl = "https://arsnova.click/api/";
            #endif
        }

        public List<HashtagInfo> GetAllHashtagInfos()
        {
            var request = this.CreateWebRequest("hashtags", HttpMethod.Get);

            var responseString = this.GetResponseString(request, HttpStatusCode.OK);

            return JsonConvert.DeserializeObject<HashtagInfos>(responseString).hashtags;
        }

        public List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
                {
                    hashtag
                });

            AnswerOptionsReturn jsonConvert;

            try
            {
                var request = this.CreateWebRequest("answerOptions", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                var responseString = this.GetResponseString(request, HttpStatusCode.OK);

                jsonConvert = JsonConvert.DeserializeObject<AnswerOptionsReturn>(responseString);
            }
            catch (JsonReaderException exception)
            {
                // not convertable -> not the object we expected. Possible reasons: arsnova.click api changed or hashtag not active
                throw new CommunicationException("Json-Object not mappable", exception);
            }

            var answerOptions = new List<AnswerOptionModel>();

            foreach (var answerOptionModelWithId in jsonConvert.answeroptions)
            {
                var answerOptionModel = new AnswerOptionModel();
                this.answerOptionMapper.Map(answerOptionModelWithId, answerOptionModel);
                answerOptions.Add(answerOptionModel);
            }

            return answerOptions;
        }

        public SessionConfiguration GetSessionConfiguration(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
                {
                    hashtag
                });

            SessionConfigurationReturn jsonConvert;

            try
            {
                var request = this.CreateWebRequest("sessionConfiguration", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                var responseString = this.GetResponseString(request, HttpStatusCode.OK);

                jsonConvert = JsonConvert.DeserializeObject<SessionConfigurationReturn>(responseString);
            }
            catch (JsonReaderException exception)
            {
                // not convertable -> not the object we expected. Possible reasons: arsnova.click api changed or hashtag not active
                throw new CommunicationException("Json-Object not mappable", exception);
            }
            
            var sessionConfiguration = new SessionConfiguration();

            this.sessionConfigurationMapper.Map(jsonConvert.sessionConfiguration.FirstOrDefault(), sessionConfiguration);

            return sessionConfiguration;
        }

        private HttpWebRequest AddContentToRequest(HttpWebRequest request, string data)
        {
            var encoding = new ASCIIEncoding();
            var dataBytes = encoding.GetBytes(data);

            request.ContentType = "application/json";
            request.ContentLength = dataBytes.Length;
            try
            {
                var requestStream = request.GetRequestStream();

                requestStream.Write(dataBytes, 0, dataBytes.Length);
            }
            catch (Exception exception)
            {
                throw new CommunicationException("The connection to the remote server could not be established.", exception);
            }

            return request;
        }

        private HttpWebRequest CreateWebRequest(string apiMethod, HttpMethod httpMethod)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(this.apiUrl + apiMethod);
            webRequest.Accept = "*/*";

            switch (httpMethod)
            {
                case HttpMethod.Get:
                    webRequest.Method = "GET";
                    break;
                case HttpMethod.Post:
                    webRequest.Method = "POST";
                    break;
                case HttpMethod.Put:
                    webRequest.Method = "PUT";
                    break;
                case HttpMethod.Delete:
                    webRequest.Method = "DELETE";
                    break;
                case HttpMethod.Patch:
                    webRequest.Method = "PATCH";
                    break;
            }

            return webRequest;
        }

        private string GetResponseString(HttpWebRequest httpWebRequest, HttpStatusCode expectedHttpStatusCode)
        {
            try
            {
                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                var responseStream = response.GetResponseStream();

                if (responseStream == null)
                {
                    throw new CommunicationException("Expected response stream is null", response.StatusCode);
                }

                var responseString = new StreamReader(responseStream).ReadToEnd();

                if (response.StatusCode != expectedHttpStatusCode)
                {
                    throw new CommunicationException("Unexpected response from server", response.StatusCode, responseString);
                }

                return responseString;
            }
            catch (WebException webException)
            {
                throw new CommunicationException("Web Exception while requesting response", webException);
            }
            catch (Exception exception)
            {
                throw new CommunicationException("General Exception while requesting response", exception);
            }
        }
    }
}
