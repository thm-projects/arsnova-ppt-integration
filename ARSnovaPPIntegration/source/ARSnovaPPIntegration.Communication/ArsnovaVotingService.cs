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

        public ArsnovaVotingService()
        {
            this.apiUrl = "https://arsnova.eu/api/";
        }

        public void CreateNewSession(SlideSessionModel slideSessionModel)
        {
            this.CheckAuthentification(slideSessionModel);
            
            var urlSuffix = "session/?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);
            var requestBody = "{" +
                               "\"courseId\": null," +
                               "\"courseType\": null," +
                               "\"creationTime\": \"" + this.ConvertToUnixTimestampString(DateTime.Now) + "\"," +
                               "\"name\": \"" + slideSessionModel.ArsnovaVotingSessionName + "\"," +
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
                               "\"shortName\": \"" + slideSessionModel.ArsnovaVotingSessionShortName + "\"" +
                           "}";

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, urlSuffix, HttpMethod.Post);

                request = this.AddContentToRequest(request, requestBody);

                var responseString = this.GetResponseString(request, HttpStatusCode.Created);

                var sessionModel = JsonConvert.DeserializeObject<SessionModel>(responseString);

                slideSessionModel.Hashtag = sessionModel.keyword;
                slideSessionModel.ArsnovaEuConfig.SessionId = sessionModel._id;

                this.SetFeatures(slideSessionModel);

            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Json-Object not mappable", exception);
            }
        }

        public void CreateOrUpdateQuestion(SlideSessionModel slideSessionModel, int questionIndex)
        {
            var question = slideSessionModel.Questions.FirstOrDefault(q => q.Index == questionIndex);

            if (question != null)
            {
                this.CheckAuthentification(slideSessionModel);
                var arsnovaQuestion = this.SlideQuestionModelToLectureQuestionModel(question, slideSessionModel.Hashtag);

                if (string.IsNullOrEmpty(question.ArsnovaVotingId))
                {
                    question.ArsnovaVotingId = this.CreateQuestion(slideSessionModel, arsnovaQuestion, slideSessionModel.Hashtag);
                }
                else
                {
                    this.UpdateQuestion(slideSessionModel, question, question.ArsnovaVotingId);
                }
            }
        }

        public void SetSessionAsActive(SlideSessionModel slideSessionModel)
        {
            this.CheckAuthentification(slideSessionModel);

            var apiPath = "lecturerquestion/disablevote?sessionkey=" + slideSessionModel.Hashtag + "&disable=false&lecturequestionsonly=false&preparationquestionsonly=false&_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Post);

                this.SendRequest(request);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Activate session failed", exception);
            }
        }

        public void StartQuestion(SlideSessionModel slideSessionModel, SlideQuestionModel slideQuestionModel)
        {
            this.SetQuestionState(slideSessionModel, slideQuestionModel, true);
        }

        public List<ArsnovaVotingResultReturnElement> GetResults(SlideSessionModel slideSessionModel, SlideQuestionModel slideQuestionModel)
        {
            // broken backend v2, need to call this before....
            /*var preApiPath = "lecturerquestion/" + slideSessionModel.ArsnovaEuConfig.SessionId + "/answer/?all=true&_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);

            try
            {
                var preRequest = this.CreateWebRequest(slideSessionModel, preApiPath, HttpMethod.Get);
                this.SendRequest(preRequest);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Get results or json deserialization failed", exception);
            }*/


            // this.SetQuestionState(slideSessionModel, slideQuestionModel, false);

            var apiPath = "lecturerquestion/" + slideQuestionModel.ArsnovaVotingId + "/answer/?piround=1&_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Get, false);
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, " sdch");

                var responseString = this.GetResponseString(request);

                return JsonConvert.DeserializeObject<List<ArsnovaVotingResultReturnElement>>(responseString);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Get results or json deserialization failed", exception);
            }
        }

        private void SetQuestionState(SlideSessionModel slideSessionModel, SlideQuestionModel slideQuestionModel,
           bool sessionStatus)
        {
            this.CheckAuthentification(slideSessionModel);

            var lectureObject = this.GetLectureQuestion(slideSessionModel, slideQuestionModel.ArsnovaVotingId);
            lectureObject.active = sessionStatus;

            var apiPath = "lecturerquestion/" + slideQuestionModel.ArsnovaVotingId + "/publish?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);
            var requestBody = JsonConvert.SerializeObject(lectureObject);

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Post);
                request = this.AddContentToRequest(request, requestBody);

                this.SendRequest(request);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Start question failed", exception);
            }
        }

        private void SetFeatures(SlideSessionModel slideSessionModel)
        {
            var sessionId = slideSessionModel.Hashtag;
            // already authentificated
            // activate questions-feature only

            var urlSuffix = "session/" + sessionId + "/features?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);
            var requestBody = "{" +
                               "\"feedback\": null," +
                               "\"flashcardFeature\": null," +
                               "\"interposed\": null," +
                               "\"jitt\": true," +
                               "\"liveFeedback\": null," +
                               "\"learningProgress\": null," +
                               "\"lecture\": null," +
                               "\"pi\": null," +
                               "\"slides\": null" +
                              "}";

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, urlSuffix, HttpMethod.Put);

                request = this.AddContentToRequest(request, requestBody);

                this.SendRequest(request);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Error while setting custom features", exception);
            }
        }

        private string CreateQuestion(SlideSessionModel slideSessionModel, LectureQuestionModel question, string sessionId)
        {
            var apiPath = "session/" + sessionId + "/question?_dc=" + this.ConvertToUnixTimestampString(DateTime.Now);
            var requestBody = JsonConvert.SerializeObject(question);

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Post);
                request = this.AddContentToRequest(request, requestBody);

                var responseString = this.GetResponseString(request, HttpStatusCode.Created);

                var questionModel = JsonConvert.DeserializeObject<LectureQuestionModelWithId>(responseString);

                return questionModel._id;
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Create question failed (maybe Json-Object not mappable)", exception);
            }
        }

        private void UpdateQuestion(SlideSessionModel slideSessionModel, SlideQuestionModel questionModel, string arsnovaVotingQuestionId)
        {
            var oldQuestion = this.GetLectureQuestion(slideSessionModel, arsnovaVotingQuestionId);

            // update entries
            oldQuestion.questionType = this.QuestionTypeToVotingQuestionType(questionModel.QuestionType);
            oldQuestion.text = questionModel.QuestionText;

            if (questionModel.QuestionType == QuestionTypeEnum.FreeTextVoting)
            {
                var answerOption = questionModel.AnswerOptions.First();

                oldQuestion.strictMode = true;
                oldQuestion.textAnswerEnabled = true;
                oldQuestion.correctAnswer = answerOption.Text;
                oldQuestion.fixedAnswer = true;
                oldQuestion.ignoreCaseSensitive = answerOption.ConfigCaseSensitive;
                oldQuestion.ignorePunctuation = answerOption.ConfigUsePunctuation;
                oldQuestion.ignoreWhitespaces = answerOption.ConfigTrimWhitespaces;
            }
            else
            {
                var answerOptionList = new List<PossibleAnswerObject>();

                foreach (var answerOption in questionModel.AnswerOptions)
                {
                    answerOptionList.Add(new PossibleAnswerObject
                    {
                        correct = answerOption.IsTrue,
                        text = answerOption.Text,
                        value = answerOption.IsTrue ? 10 : -10
                    });
                }

                oldQuestion.possibleAnswers = answerOptionList;
            }

            var apiPath = "lecturerquestion/" + arsnovaVotingQuestionId;
            var requestBody = JsonConvert.SerializeObject(oldQuestion);

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Put);
                request = this.AddContentToRequest(request, requestBody);

                this.SendRequest(request);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Update question failed", exception);
            }
        }

        private LectureQuestionModel SlideQuestionModelToLectureQuestionModel(SlideQuestionModel questionModel, string sessionKey)
        {
            var leqtureQuestion = new LectureQuestionModel
            {
                abstention = false, // TODO should abstention be allowed?
                active = false,
                imageQuestion = false,
                number = questionModel.Index - 1,
                questionType = this.QuestionTypeToVotingQuestionType(questionModel.QuestionType),
                questionVariant = "preparation",
                releasedFor = "all",
                sessionKeyword = sessionKey,
                showStatistic = true,
                subject = "powerpoint generated questions", // TODO
                text = questionModel.QuestionText,
                type = "skill_question",
                votingDisabled = true // Disabled by default!
            };

            if (questionModel.QuestionType != QuestionTypeEnum.FreeTextVoting)
            {
                var answerOptionList = new List<PossibleAnswerObject>();

                foreach (var answerOption in questionModel.AnswerOptions)
                {
                    answerOptionList.Add(new PossibleAnswerObject
                    {
                        correct = answerOption.IsTrue,
                        text = answerOption.Text,
                        value = answerOption.IsTrue ? 10 : -10
                    });
                }

                leqtureQuestion.possibleAnswers = answerOptionList;
            }
            else
            {
                // freetext
                var answerOption = questionModel.AnswerOptions.First();

                leqtureQuestion.strictMode = true;
                leqtureQuestion.textAnswerEnabled = true;
                leqtureQuestion.correctAnswer = answerOption.Text;
                leqtureQuestion.fixedAnswer = true;
                leqtureQuestion.ignoreCaseSensitive = answerOption.ConfigCaseSensitive;
                leqtureQuestion.ignorePunctuation = answerOption.ConfigUsePunctuation;
                leqtureQuestion.ignoreWhitespaces = answerOption.ConfigTrimWhitespaces;
            }

            return leqtureQuestion;
        }

        private string QuestionTypeToVotingQuestionType(QuestionTypeEnum questionType)
        {
            switch (questionType)
            {
                case QuestionTypeEnum.SingleChoiceVoting:
                    return "sc";
                case QuestionTypeEnum.MultipleChoiceVoting:
                    return "mc";
                case QuestionTypeEnum.YesNoVoting:
                    return "yesno";
                case QuestionTypeEnum.FreeTextVoting:
                    return "freetext";
                case QuestionTypeEnum.EvaluationVoting:
                    return "vote";
                case QuestionTypeEnum.GradsVoting:
                    return "school";
                default: return string.Empty;
            }
        }

        private LectureQuestionModel GetLectureQuestion(SlideSessionModel slideSessionModel, string questionId)
        {
            /*var allQuestions = this.GetAllQuestionsOfSession(slideSessionModel);

            return allQuestions.FirstOrDefault(q => q._id == questionId);*/

            this.CheckAuthentification(slideSessionModel);
            var apiPath = "lecturerquestion/" + questionId;

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Get);

                var responseString = this.GetResponseString(request);

                return JsonConvert.DeserializeObject<LectureQuestionModelWithId>(responseString);
            }
            catch (JsonReaderException exception)
            {
                throw new CommunicationException("Json-Object not mappable", exception);
            }
        }

        private void CheckAuthentification(SlideSessionModel slideSessionModel)
        {
            if (!slideSessionModel.ArsnovaEuConfig.IsAuthenticated)
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
                        this.FirstLogin(slideSessionModel);
                        break;
                }
            }
        }

        // should be called with alternative login methods only (auto guest login)
        private void FirstLogin(SlideSessionModel slideSessionModel)
        {
            var userName = slideSessionModel.ArsnovaEuConfig.GuestUserName;
            var loginMethod = slideSessionModel.ArsnovaEuConfig.LoginMethod;

            var apiPath = "auth/login?type=guest&user=" + userName + "&_dc=" +
                      this.ConvertToUnixTimestampString(DateTime.Now);

            try
            {
                var request = this.CreateWebRequest(slideSessionModel, apiPath, HttpMethod.Get);

                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    slideSessionModel.ArsnovaEuConfig.Cookies = new List<Cookie>();
                    // login success
                    foreach (Cookie cookie in response.Cookies)
                    {
                        slideSessionModel.ArsnovaEuConfig.Cookies.Add(cookie);
                    }

                    slideSessionModel.ArsnovaEuConfig.IsAuthenticated = true;
                }
            }
            catch (WebException webException)
            {
                throw new CommunicationException("Authentification Error", webException);
            }
        }

        private HttpWebRequest CreateWebRequest(SlideSessionModel slideSessionModel, string apiUrlSuffix, HttpMethod httpMethod, bool withContentType = true)
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

            if (withContentType)
            {
                webRequest.ContentType = "application/json";
            }
            
            webRequest.Accept = "*/*";
            webRequest.Referer = "https://arsnova.eu/mobile/";
            webRequest.CookieContainer = new CookieContainer();

            foreach (var arsnovaEuHeader in this.arsnovaEuHeaders)
            {
                webRequest.Headers.Set(arsnovaEuHeader.Item1, arsnovaEuHeader.Item2);
            }

            if (slideSessionModel.ArsnovaEuConfig.Cookies != null)
            {
                foreach (var cookie in slideSessionModel.ArsnovaEuConfig.Cookies)
                {
                    webRequest.CookieContainer.Add(new Uri("https://arsnova.eu"), new Cookie("JSESSIONID", cookie.Value));
                    //webRequest.CookieContainer.Add(cookie);
                }
                
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
