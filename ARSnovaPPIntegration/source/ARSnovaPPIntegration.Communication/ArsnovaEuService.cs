using System;
using System.Collections.Generic;
using System.IO;
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
        public SessionModel CreateNewSession()
        {
            return this.CreateNewSessionTask();
        }

        private SessionModel CreateNewSessionTask()
        {
            var url = "https://arsnova.thm.de/api/session/";

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            var requestBody = "{" +
                               "\"name\":\"testQuizOffice\"" +
                               "\"shortName\":\"tqo\"" +
                               "\"courseId\":\"null\"" +
                               "\"courseType\":\"null\"" +
                               "\"creationTime\":\"" + DateTime.Now.ToString() + "\"" +
                               "\"ppAuthorName\":\"Tjark Wilhelm Hoeck\"" +
                               "\"ppAuthorMail\":\"tjark.wilhelm.hoeck@mni.thm.de\"" +
                               "\"ppLogo\":\"null\"" +
                               "\"ppSubject\":\"null\"" +
                               "\"ppLicense\":\"null\"" +
                               "\"ppDescription\":\"null\"" +
                               "\"ppFaculty\":\"null\"" +
                               "\"ppLevel\":\"null\"" +
                               "\"sessionType\":\"null\"" +
                           "}";

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
    }
}
