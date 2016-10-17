using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Model.ArsnovaClick;
using Newtonsoft.Json;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaClickService : IArsnovaClickService
    {
        private readonly string apiUrl;
        
        public ArsnovaClickService()
        {
            #if DEBUG
                this.apiUrl = "http://localhost:3000/api/";
            #else
                this.apiUrl = "https://arsnova.click/api/";
            #endif
        }

        public string FindAllHashtags()
        {
            var request = this.CreateWebRequest("hashtags", HttpMethod.Get);

            var responseString = this.GetResponseString(request, HttpStatusCode.OK);

            return responseString;
        }

        public List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
                {
                    hashtag
                });

            var request = this.CreateWebRequest("answerOptions", HttpMethod.Post);

            request = this.AddContentToRequest(request, jsonBody);

            var responseString = this.GetResponseString(request, HttpStatusCode.OK);

            var jsonConvert = JsonConvert.DeserializeObject< AnswerOptionsReturn>(responseString);

            return jsonConvert.answeroptions;
        }

        public SessionConfiguration GetSessionConfiguration(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
                {
                    hashtag
                });

            var request = this.CreateWebRequest("sessionConfiguration", HttpMethod.Post);

            request = this.AddContentToRequest(request, jsonBody);

            var responseString = this.GetResponseString(request, HttpStatusCode.OK);

            var jsonConvert = JsonConvert.DeserializeObject<SessionConfigurationReturn>(responseString);

            return jsonConvert.sessionConfiguration.FirstOrDefault();
        }

        private HttpWebRequest AddContentToRequest(HttpWebRequest request, string data)
        {
            var encoding = new ASCIIEncoding();
            var dataBytes = encoding.GetBytes(data);

            request.ContentType = "application/json";
            request.ContentLength = dataBytes.Length;

            var requestStream = request.GetRequestStream();

            requestStream.Write(dataBytes, 0, dataBytes.Length);

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

    // Json Result Casting classes
    internal class AnswerOptionsReturn
    {
        public List<AnswerOptionModel> answeroptions { get; set; }
    }

    internal class SessionConfigurationReturn
    {
        public List<SessionConfiguration> sessionConfiguration { get; set; }
    }
}
