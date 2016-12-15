using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Communication.CastHelpers.Models;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

using HttpMethod = ARSnovaPPIntegration.Common.Enum.HttpMethod;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaClickApi
    {
        private readonly string apiUrl;

        public ArsnovaClickApi(
            string apiUrl)
        {
            this.apiUrl = apiUrl;
        }

        public List<HashtagInfo> GetAllHashtagInfos()
        {
            var request = this.CreateWebRequest("hashtags", HttpMethod.Get);

            var responseString = this.GetResponseString(request);

            return JsonConvert.DeserializeObject<HashtagInfos>(responseString).hashtags;
        }

        public AnswerOptionsReturn GetAnswerOptions(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                hashtag = Uri.EscapeDataString(hashtag)
            });

            try
            {
                var request = this.CreateWebRequest("answerOptions", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                var responseString = this.GetResponseString(request);

                return JsonConvert.DeserializeObject<AnswerOptionsReturn>(responseString);
            }
            catch (JsonReaderException exception)
            {
                // not convertable -> not the object we expected. Possible reasons: arsnova.click api changed or hashtag not active
                throw new CommunicationException("Json-Object not mappable", exception);
            }
        }

        public ValidationResult AddHashtag(string hashtag, string privateKey)
        {
            // test only start
            var jsonBody2 = JsonConvert.SerializeObject(new
            {
                hashtag = Uri.EscapeDataString("Demo Quiz 87")
            });


            var request2 = this.CreateWebRequest("getQuestionGroup", HttpMethod.Post);

            request2 = this.AddContentToRequest(request2, jsonBody2);

            var response = this.GetResponseString(request2);
            

            // test only end



            var validationResult = new ValidationResult();

            var createHashtagConfig = new 
                                      {
                                          hashtag = Uri.EscapeDataString(hashtag),
                                          privateKey
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = createHashtagConfig
            });

            try
            {
                var request = this.CreateWebRequest("addHashtag", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                this.SendRequest(request);
            }
            catch (CommunicationException comException)
            {
                validationResult = this.CommunicationExceptionToValidationResult(comException);
            }

            return validationResult;
        }

        public ValidationResult DeleteQuestionGroup(string hashtag, string privateKey)
        {
            var validationResult = new ValidationResult();

            var deleteQuestionGroupConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = deleteQuestionGroupConfig
            });

            try
            {
                var request = this.CreateWebRequest("removeQuestionGroup", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                this.SendRequest(request);
            }
            catch (CommunicationException comException)
            {
                validationResult = this.CommunicationExceptionToValidationResult(comException);
            }

            return validationResult;
        }
        
        /*public ValidationResult AddQuestionGroup(string hashtag, string privateKey, QuestionGroupReturn questionGroup)
        {
            // TODO
            return new ValidationResult();
        }*/


        public SessionConfigurationReturn GetSessionConfiguration(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                hashtag = Uri.EscapeDataString(hashtag)
            });

            try
            {
                var request = this.CreateWebRequest("sessionConfiguration", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                var responseString = this.GetResponseString(request);

                return JsonConvert.DeserializeObject<SessionConfigurationReturn>(responseString);
            }
            catch (JsonReaderException exception)
            {
                // not convertable -> not the object we expected. Possible reasons: arsnova.click api changed or hashtag not active
                throw new CommunicationException("Json-Object not mappable", exception);
            }
        }

        public string NewPrivateKey()
        {
            var request = this.CreateWebRequest("createPrivateKey", HttpMethod.Get);
            return this.GetResponseString(request);
        }

        // Request-Helpers

        private ValidationResult CommunicationExceptionToValidationResult(CommunicationException comException)
        {
            var validationResult = new ValidationResult();

            validationResult.FailureTitel = "Communication Error";
            validationResult.FailureMessage = comException.WithHttpStatusCode
                                                  ? comException.HttpStatusCode
                                                    + comException.ServerResponseString
                                                  : comException.ServerResponseString;
            return validationResult;
        }

        private HttpWebRequest AddContentToRequest(HttpWebRequest request, string data)
        {
            var encoding = new ASCIIEncoding();
            var dataBytes = encoding.GetBytes(data);

            //request.ContentType = "application/json";
            request.ContentType = "application/json; charset=utf-8";
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
            var webRequest = (HttpWebRequest)WebRequest.Create(this.apiUrl + apiMethod);
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

        private void SendRequest(HttpWebRequest httpWebRequest, HttpStatusCode expectedHttpStatusCode = HttpStatusCode.OK)
        {
            try
            {
                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                if (response.StatusCode != expectedHttpStatusCode)
                {
                    throw new CommunicationException("Unexpected response from server", response.StatusCode);
                }
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

        private string GetResponseString(HttpWebRequest httpWebRequest, HttpStatusCode expectedHttpStatusCode = HttpStatusCode.OK)
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
