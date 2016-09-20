using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Model;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaEuService : IArsnovaEuService
    {
        private readonly List<Tuple<string, string>> arsnovaEuHeaders = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("Origin", "https://arsnova.eu"),
            new Tuple<string, string>("X-Requested-With", "XMLHttpRequest"),
            new Tuple<string, string>("Accept-Encoding", "gzip, deflate, br"),
            new Tuple<string, string>("Accept-Language", "de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4")
        };

        private List<Cookie> arsnovaEuCookies;

        private bool isAuthentificated = false;

        public void Login(LoginMethod loginMethod = LoginMethod.Guest)
        {
            // TODO how long does the cookie remain? check it everytime before a HTTP-Request is fired!

            var url = "https://arsnova.eu/api/auth/login?type=guest&user=" + this.GenerateGuestName() + "&_dc=" +
                      this.ConvertToUnixTimestampString(DateTime.Now);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            // TODO sum the next part up! bad code quality in here!
            request.Method = "GET";
            request.Host = "arsnova.eu";
            request.KeepAlive = true;
            request.ContentType = "application/json";
            request.Accept = "*/*";
            request.Referer = "https://arsnova.eu/mobile/";

            // TODO swap this one, too! (differ from http-method)
            try
            {
                this.arsnovaEuCookies = new List<Cookie>();

                var response = (HttpWebResponse)request.GetResponse();

                foreach (Cookie cookie in response.Cookies)
                {
                    this.arsnovaEuCookies.Add(cookie);
                }

                // check for authentification!
                this.isAuthentificated = true;

                /*using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    if (response.IsMutuallyAuthenticated)
                    {
                        // test purpose only
                    }
                }*/
            }
            catch (WebException webException)
            {
                throw new ArsnovaCommunicationException("Authentification Error", webException);
            }
        }

        public SessionModel CreateNewSession()
        {
            this.CheckAuthentification();

            var url = "https://arsnova.eu/api/session/?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);

            var request = (HttpWebRequest)WebRequest.Create(url);

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

            var requestBodyData = Encoding.ASCII.GetBytes(requestBody);

            // The Headers with Properties should be setted with them
            request.Method = "POST";
            request.Host = "arsnova.eu";
            request.KeepAlive = true;
            request.ContentType = "application/json";
            request.Accept = "*/*";
            request.Referer = "https://arsnova.eu/mobile/";
            request.CookieContainer = new CookieContainer();
            request.ContentLength = requestBody.Length;

            foreach (var arsnovaEuHeader in this.arsnovaEuHeaders)
            {
                request.Headers.Set(arsnovaEuHeader.Item1, arsnovaEuHeader.Item2);
            }



            //request.ContentLength = requestBody.Length;

            foreach (var cookie in this.arsnovaEuCookies)
            {
                request.CookieContainer.Add(cookie);
            }

            using (var stream = request.GetRequestStream())
            {
                stream.Write(requestBodyData, 0, requestBodyData.Length);

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();

                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var js = new JavaScriptSerializer();

                    return (SessionModel)js.Deserialize(responseString, typeof(SessionModel));

                    /*using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        return (SessionModel)js.Deserialize(objText, typeof(SessionModel));
                    }*/
                }
                catch (WebException webException)
                {
                    throw new ArsnovaCommunicationException("Error while creating new session", webException);
                }
            }
        }

        private void CheckAuthentification()
        {
            if (!this.isAuthentificated)
            {
                this.Login();
            }
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

        private string ConvertToUnixTimestampString(DateTime dateTime)
        {
            var unixDateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var dateTimeSpan = dateTime.ToUniversalTime() - unixDateTimeStart;
            return Math.Floor(dateTimeSpan.TotalSeconds).ToString(CultureInfo.CurrentCulture);
        }
    }
}
