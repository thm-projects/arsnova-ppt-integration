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

        public ArsnovaClickApi()
        {
            #if DEBUG
                this.apiUrl = "http://localhost:3000/api/";
            #else
                this.apiUrl = "https://arsnova.click/api/";
            #endif
        }

        public bool IsThisMineHashtag(string hashtag, string privateKey)
        {
            var createHashtagConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey = Uri.EscapeDataString(privateKey)
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = createHashtagConfig
            });

            var request = this.CreateWebRequest("isThisMineHashtag", HttpMethod.Post);

            request = this.AddContentToRequest(request, jsonBody);

            var response = (HttpWebResponse)request.GetResponse();

            return response.StatusCode == HttpStatusCode.OK;
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

        public AnswerOptionsReturn GetResultsForHashtag(string hashtag)
        {
            var jsonBody = JsonConvert.SerializeObject(new
            {
                hashtag = Uri.EscapeDataString(hashtag)
            });

            try
            {
                var request = this.CreateWebRequest("getResultsFromHashtag", HttpMethod.Post);

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

        public ValidationResult ResetSession(string hashtag, string privateKey)
        {
            var createHashtagConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey = Uri.EscapeDataString(privateKey)
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = createHashtagConfig
            });

            return this.DefaultPostRequestWithHashtagAndPrivateKey("resetSession", jsonBody);
        }

        public ValidationResult AddHashtag(string hashtag, string privateKey)
        {
            var createHashtagConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey = Uri.EscapeDataString(privateKey)
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = createHashtagConfig
            });

            return this.DefaultPostRequestWithHashtagAndPrivateKey("addHashtag", jsonBody);
        }

        public ValidationResult ShowNextReadingConfirmation(string hashtag, string privateKey)
        {
            var createHashtagConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey = Uri.EscapeDataString(privateKey)
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = createHashtagConfig
            });

            return this.DefaultPostRequestWithHashtagAndPrivateKey("showReadingConfirmation", jsonBody);
        }

        public ValidationResult OpenSession(string hashtag , string privateKey)
        {
            var updateSessionStatusConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey = Uri.EscapeDataString(privateKey)
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = updateSessionStatusConfig
            });

            return this.DefaultPostRequestWithHashtagAndPrivateKey("openSession", jsonBody);
        }

        public ValidationResult StartNextQuestion(string hashtag, string privateKey, int questionIndex)
        {
            var createHashtagConfig = new
            {
                hashtag = Uri.EscapeDataString(hashtag),
                privateKey = Uri.EscapeDataString(privateKey),
                questionIndex = questionIndex
            };

            var jsonBody = JsonConvert.SerializeObject(new
            {
                sessionConfiguration = createHashtagConfig
            });

            return this.DefaultPostRequestWithHashtagAndPrivateKey("startNextQuestion", jsonBody);
        }

        public ValidationResult UpdateQuestionGroup(QuestionGroupModel questionGroupModel, string privateKey)
        {
            var validationResult = new ValidationResult();

            var jsonBody = JsonConvert.SerializeObject(new
            {
                privateKey,
                questionGroupModel
            });

            try
            {
                var request = this.CreateWebRequest("updateQuestionGroup", HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                this.SendRequest(request);
            }
            catch (CommunicationException comException)
            {
                validationResult = this.CommunicationExceptionToValidationResult(comException);
            }

            return validationResult;
        }

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

        private ValidationResult DefaultPostRequestWithHashtagAndPrivateKey(string methodName, string jsonBody)
        {
            var validationResult = new ValidationResult();

            try
            {
                var request = this.CreateWebRequest(methodName, HttpMethod.Post);

                request = this.AddContentToRequest(request, jsonBody);

                this.SendRequest(request);
            }
            catch (CommunicationException comException)
            {
                validationResult = this.CommunicationExceptionToValidationResult(comException);
            }

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
