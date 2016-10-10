using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;

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

        public List<string> GetAnswerOptionsForHashtag(string hashtag)
        {
            var jsonBody = this.ObjectToJSON(hashtag);

            var request = this.CreateWebRequest("answerOptions", HttpMethod.Post);

            request = this.AddContentToRequest(request, "hashtag", hashtag);

            var responseString = this.GetResponseString(request, HttpStatusCode.OK);

            // test only
            var result = new List<string> {responseString};

            return result;
        }

        private string ObjectToJSON(object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        private object JSONToObject(string json)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.DeserializeObject(json);
        }

        private HttpWebRequest AddContentToRequest(HttpWebRequest request, string varName, string content)
        {
            var data = varName + "=" + content;
            var encoding = new ASCIIEncoding();
            var dataBytes = encoding.GetBytes(data);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = dataBytes.Length;

            var requestStream = request.GetRequestStream();

            requestStream.Write(dataBytes, 0, dataBytes.Length);

            /*   if (content.Length > 0)
               {
                   var data = Encoding.ASCII.GetBytes(content);

                   request.ContentType = "application/json";
                   request.ContentLength = data.Length;

                   using (var stream = request.GetRequestStream())
                   {
                       stream.Write(data, 0 , data.Length);
                   }
               }*/

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
