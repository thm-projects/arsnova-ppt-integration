using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Model;
using ARSnovaPPIntegration.Common.Contract.Exceptions;

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

        public ArsnovaEuService()
        {
            // Login as guest by default
            // TODO how long does the cookie remain? check it everytime before a HTTP-Request is fired!
            this.Login();
            this.Authentification();
        }

        public SessionModel CreateNewSession()
        {
            return this.CreateNewSessionTask();
        }

        private void Login()
        {
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

                /*using (var isf = IsolatedStorageFile.GetUserStoreForSite())
                {
                    using (var isfs = isf.OpenFile("CookieExCookies", FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            this.arsnovaEuCookies = new List<string>();

                            foreach (var cookieValue in response.Cookies)
                            {
                                this.arsnovaEuCookies.Add(cookieValue.ToString());
                            }
                        }
                    }
                }*/


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
                throw new ArsnovaCommunicationException("Error while creating new session", webException);
            }
        }

        private void Authentification()
        {
            var url = "https://arsnova.eu/api/auth/";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            // TODO sum the next part up! bad code quality in here!
            request.Method = "GET";
            request.Host = "arsnova.eu";
            request.KeepAlive = true;
            request.ContentType = "application/json";
            request.Accept = "*/*";
            request.Referer = "https://arsnova.eu/mobile/";

            foreach (var cookie in this.arsnovaEuCookies)
            {
                request.CookieContainer.Add(cookie);
            }

            var response = (HttpWebResponse)request.GetResponse();

            /*this.arsnovaEuCookies = new List<Cookie>();
            foreach (Cookie cookie in response.Cookies)
            {
                this.arsnovaEuCookies.Add(cookie);
            }*/

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

        private SessionModel CreateNewSessionTask()
        {
            var url = "https://arsnova.eu/api/session/?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);

            var request = (HttpWebRequest) WebRequest.Create(url);

            // The Headers with Properties should be setted with them
            request.Method = "POST";
            request.Host = "arsnova.eu";
            request.KeepAlive = true;
            //request.Connection = "keep-alive";
            request.ServicePoint.Expect100Continue = false;
            request.ContentType = "application/json";
            request.Accept = "*/*";
            request.Referer = "https://arsnova.eu/mobile/";
            request.CookieContainer = new CookieContainer();

            foreach (var arsnovaEuHeader in this.arsnovaEuHeaders)
            {
                request.Headers.Set(arsnovaEuHeader.Item1, arsnovaEuHeader.Item2);
            }

            var requestBody = "{" +
                               "\"courseId\": \"null\"," +
                               "\"courseType\": \"null\"" +
                               "\"creationTime\": \"" + this.ConvertToUnixTimestampString(DateTime.Now) + "\"," +
                               "\"name\": \"testQuizOffice\"" +
                               "\"ppAuthorMail\": \"null\"," +
                               "\"ppAuthorName\": \"null\"," +
                               "\"ppDescription\": \"null\"," +
                               "\"ppFaculty\": \"null\"," +
                               "\"ppLevel\": \"null\"," +
                               "\"ppLicense\": \"null\"," +
                               "\"ppLogo\": \"null\"," +
                               "\"ppSubject\": \"null\"," +
                               "\"ppUniversity\": \"null\"," +
                               "\"sessionType\": \"null\"," +
                               "\"shortName\": \"tqo\"" +
                           "}";

            //request.ContentLength = requestBody.Length;

            foreach (var cookie in this.arsnovaEuCookies)
            {
                request.CookieContainer.Add(cookie);
            }

            using (Stream stream = this.GenerateStreamFromString(requestBody))
            {
                var dataStream = request.GetRequestStream();
                stream.CopyTo(dataStream);

                try
                {
                    var response = (HttpWebResponse)request.GetResponse();

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        return (SessionModel)js.Deserialize(objText, typeof(SessionModel));
                    }
                }
                catch (WebException webException)
                {
                    throw new ArsnovaCommunicationException("Error while creating new session", webException);
                }
            }

            // old try

            /*using (var client = this.CreateArsnovaHttpClient())
            {
                var content = new HttpContent

                HttpResponseMessage response = await client.PostAsync("/session", ); // TODO content

                if (response.IsSuccessStatusCode)
                {
                    var session = await response.Content.ReadAs
                }
            }*/
        }

        private HttpClient CreateArsnovaHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://arsnova.thm.de/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private string ConvertToUnixTimestampString(DateTime dateTime)
        {
            var unixDateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var dateTimeSpan = dateTime.ToUniversalTime() - unixDateTimeStart;
            return Math.Floor(dateTimeSpan.TotalSeconds).ToString(CultureInfo.CurrentCulture);
        }
    }
}
