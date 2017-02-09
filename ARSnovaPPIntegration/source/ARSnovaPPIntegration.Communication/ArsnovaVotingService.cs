using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Newtonsoft.Json;

using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Model.ArsnovaEu;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaVotingService : IArsnovaVotingService
    {
        private readonly List<Tuple<string, string>> arsnovaEuHeaders = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("Origin", "https://arsnova.eu"),
            new Tuple<string, string>("X-Requested-With", "XMLHttpRequest"),
            new Tuple<string, string>("Accept-Encoding", "gzip, deflate, br"),
            new Tuple<string, string>("Accept-Language", "de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4")
        };

        private readonly string apiUrl;

        private List<Cookie> arsnovaEuCookies;

        private bool isAuthentificated = false;

        public ArsnovaVotingService()
        {
            this.apiUrl = "https://arsnova.eu/api/";
        }

        public SessionModel CreateNewSession(SlideSessionModel slideSessionModel)
        {
            this.CheckAuthentification(slideSessionModel);
            var urlSuffix = "session/?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);
            var requestBody = "{" +
                               "\"courseId\": null," +
                               "\"courseType\": null," +
                               "\"creationTime\": \"" + this.ConvertToUnixTimestampString(DateTime.Now) + "\"," +
                               "\"name\": \"testQuizOffice\"," +
                               "\"ppAuthorMail\": null," +
                               "\"ppAuthorName\": null," +
                               "\"ppDescription\": null," +
                               "\"ppFaculty\": null," +
                               "\"ppLevel\": null," +
                               "\"ppLicense\": null," +
                               "\"ppLogo\": null," +
                               "\"ppSubject\": null," +
                               "\"ppUniversity\": null," +
                               "\"sessionType\": null," +
                               "\"shortName\": \"tqo\"" +
                           "}";

            try
            {
                var request = this.CreateWebRequest(urlSuffix, HttpMethod.Post);

                request = this.AddContentToRequest(request, requestBody);

                var responseString = this.GetResponseString(request, HttpStatusCode.Created);

                return JsonConvert.DeserializeObject<SessionModel>(responseString);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Json-Object not mappable", exception);
            }
        }

        public SessionModel GetSessionInformation(SlideSessionModel slideSessionModel)
        {
            // TODO
            throw new NotImplementedException();
        }

        public LectureQuestionModel GetLectureQuestion(SlideSessionModel slideSessionModel, string questionId)
        {
            this.CheckAuthentification(slideSessionModel);
            var urlSuffix = "lecturerquestion/" + questionId;

            try
            {
                var request = this.CreateWebRequest(urlSuffix, HttpMethod.Get);

                var responseString = this.GetResponseString(request);

                return JsonConvert.DeserializeObject<LectureQuestionModel>(responseString);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Json-Object not mappable", exception);
            }
        }

        public List<LectureQuestionModel> GetLectureQuestionInfos(SlideSessionModel slideSessionModel)
        {
            // TODO
            throw new NotImplementedException();
        }

        private void CheckAuthentification(SlideSessionModel slideSessionModel)
        {
            if (!this.isAuthentificated)
            {
                if (string.IsNullOrEmpty(slideSessionModel.ArsnovaEuConfig.GuestUserName)
                    && slideSessionModel.ArsnovaEuConfig.LoginMethod == LoginMethod.Guest)
                {
                    slideSessionModel.ArsnovaEuConfig.GuestUserName = this.GenerateGuestName();
                }

                // TODO implement other authentification methods
                switch (slideSessionModel.ArsnovaEuConfig.LoginMethod)
                {
                    case LoginMethod.Guest:
                        this.Login(slideSessionModel.ArsnovaEuConfig.GuestUserName, slideSessionModel.ArsnovaEuConfig.LoginMethod);
                        break;
                }
            }
        }

        // should be called with alternative login methods only (auto guest login)
        private void Login(string userName, LoginMethod loginMethod)
        {
            // TODO how long does the cookie remain? check it everytime before a HTTP-Request is fired!
            // TODO loginMethod
            var urlSuffix = "auth/login?type=guest&user=" + userName + "&_dc=" +
                      this.ConvertToUnixTimestampString(DateTime.Now);

            try
            {
                var request = this.CreateWebRequest(urlSuffix, HttpMethod.Get);

                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // login success
                    foreach (Cookie cookie in response.Cookies)
                    {
                        this.arsnovaEuCookies.Add(cookie);
                    }

                    this.isAuthentificated = true;
                }
            }
            catch (WebException webException)
            {
                throw new CommunicationException("Authentification Error", webException);
            }
        }

        private HttpWebRequest CreateWebRequest(string apiUrlSuffix, HttpMethod httpMethod)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(this.apiUrl + apiUrlSuffix);
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

            webRequest.Host = "arsnova.eu";
            webRequest.KeepAlive = true;
            webRequest.ContentType = "application/json";
            webRequest.Accept = "*/*";
            webRequest.Referer = "https://arsnova.eu/mobile/";
            webRequest.CookieContainer = new CookieContainer();

            foreach (var arsnovaEuHeader in this.arsnovaEuHeaders)
            {
                webRequest.Headers.Set(arsnovaEuHeader.Item1, arsnovaEuHeader.Item2);
            }

            foreach (var cookie in this.arsnovaEuCookies)
            {
                webRequest.CookieContainer.Add(cookie);
            }

            return webRequest;
        }

        private HttpWebRequest AddContentToRequest(HttpWebRequest request, string data)
        {
            var encoding = new ASCIIEncoding();
            var dataBytes = encoding.GetBytes(data);

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

        private string ConvertToUnixTimestampString(DateTime dateTime)
        {
            var unixDateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var dateTimeSpan = dateTime.ToUniversalTime() - unixDateTimeStart;
            return Math.Floor(dateTimeSpan.TotalSeconds).ToString(CultureInfo.CurrentCulture);
        }

        private string GenerateGuestName()
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
            const int stringLength = 5;
            const string randomstring = "Guest";
            var random = new Random();

            return randomstring + new string(Enumerable.Repeat(chars, stringLength)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
