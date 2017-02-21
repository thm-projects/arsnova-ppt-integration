using System;
using System.Collections.Generic;
using System.Linq;

using ARSnovaPPIntegration.Communication.CastHelpers.Converters;
using ARSnovaPPIntegration.Communication.CastHelpers.Models;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Communication
{
    public class ArsnovaClickService : IArsnovaClickService
    {
        private readonly ArsnovaClickApi arsnovaClickApi;

        private readonly ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel> answerOptionMapper;

        private readonly ObjectMapper<ResultModelWithId, ResultModel> responseMapper;

        private readonly ObjectMapper<SessionConfigurationWithId, SessionConfiguration> sessionConfigurationMapper;

        public ArsnovaClickService()
        {
            this.answerOptionMapper = new ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel>();

            this.responseMapper = new ObjectMapper<ResultModelWithId, ResultModel>();

            this.sessionConfigurationMapper = new ObjectMapper<SessionConfigurationWithId, SessionConfiguration>();

            this.arsnovaClickApi = new ArsnovaClickApi();
        }

        public List<HashtagInfo> GetAllHashtagInfos()
        {
            return this.arsnovaClickApi.GetAllHashtagInfos();
        }

        public List<AnswerOptionModel> GetAnswerOptionsForHashtag(string hashtag)
        {
            var answerOptionsReturnModel = this.arsnovaClickApi.GetAnswerOptions(hashtag);

            var answerOptions = new List<AnswerOptionModel>();

            foreach (var answerOptionModelWithId in answerOptionsReturnModel.answeroptions)
            {
                var answerOptionModel = new AnswerOptionModel();
                this.answerOptionMapper.Map(answerOptionModelWithId, answerOptionModel);
                answerOptions.Add(answerOptionModel);
            }

            return answerOptions;
        }

        public List<ResultModel> GetResultsForHashtag(string hashtag, int questionIndex)
        {
            var resultsReturnModel = this.arsnovaClickApi.GetResultsForHashtag(hashtag);

            if (resultsReturnModel.responses != null
                && resultsReturnModel.responses.Count > 0)
            {
                var responses = new List<ResultModel>();

                foreach (var resultModelWithId in resultsReturnModel.responses)
                {
                    var resultModel = new ResultModel();
                    this.responseMapper.Map(resultModelWithId, resultModel);
                    responses.Add(resultModel);
                }

                return responses.Where(r => r.questionIndex == questionIndex).ToList();
            }

            return null;
        }

        public SessionConfiguration GetSessionConfiguration(string hashtag)
        {
            var sessionConfigurationReturnModel = this.arsnovaClickApi.GetSessionConfiguration(hashtag);

            var sessionConfiguration = new SessionConfiguration();

            this.sessionConfigurationMapper.Map(sessionConfigurationReturnModel.sessionConfiguration.FirstOrDefault(), sessionConfiguration);

            return sessionConfiguration;
        }

        public string CreateHashtag(string hashtag)
        {
            try
            {
                var privateKey = this.arsnovaClickApi.NewPrivateKey();
                this.arsnovaClickApi.AddHashtag(hashtag, privateKey);

                return privateKey;
            }
            catch (CommunicationException comException)
            {
                throw new Exception("Error while creating hashtag: arsnova.click server down? Statuscode: " + comException.HttpStatusCode);
            }
        }

        public ValidationResult UpdateQuestionGroup(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.UpdateQuestionGroup(this.SlideSessionModelToQuestionGroupModel(slideSessionModel), slideSessionModel.PrivateKey);
        }

        public ValidationResult ShowNextReadingConfirmation(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.ShowNextReadingConfirmation(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);
        }

        public ValidationResult MakeSessionAvailable(string hashtag, string privateKey)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(hashtag, privateKey);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.OpenSession(hashtag, privateKey);
        }

        public ValidationResult RemoveQuizData(string hashtag, string privateKey)
        {
            return this.arsnovaClickApi.RemoveQuizData(hashtag, privateKey);
        }

        public ValidationResult KeepAlive(string hashtag, string privateKey)
        {
            return this.arsnovaClickApi.KeepAlive(hashtag, privateKey);
        }

        public bool IsThisMineHashtag(string hashtag, string privateKey)
        {
            return this.arsnovaClickApi.IsThisMineHashtag(hashtag, privateKey);
        }

        public ValidationResult StartNextQuestion(SlideSessionModel slideSessionModel, int questionIndex)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.StartNextQuestion(slideSessionModel.Hashtag, slideSessionModel.PrivateKey, questionIndex);
        }

        private ValidationResult CheckForHashtagAndPrivateKey(string hashtag, string privateKey)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(hashtag))
            {
                validationResult.FailureTitel = "Error -  hashtag";
                validationResult.FailureMessage = "No hashtag provided";
                return validationResult;
            }

            if (string.IsNullOrEmpty(privateKey))
            {
                validationResult.FailureTitel = "Error -  private key";
                validationResult.FailureMessage = "No private key provided";
                return validationResult;
            }

            return validationResult;
        }

        private QuestionGroupModel SlideSessionModelToQuestionGroupModel(SlideSessionModel slideSessionModel)
        {
            var questionModelList =
                slideSessionModel.Questions.Where(q => !q.Hidden).Select(q => this.SlideQuestionModelToQuestionModel(q, slideSessionModel.Hashtag)).ToList();

            // a valid questionGroupModel requires a hashtag only, the rest can be null or missing (not defined) -> change values to null if not needed (from pre-config)
            return new QuestionGroupModel
                   {
                       hashtag = Uri.EscapeDataString(slideSessionModel.Hashtag),
                       questionList = questionModelList,
                       configuration = new ConfigurationModel
                                       {
                                            hashtag = Uri.EscapeDataString(slideSessionModel.Hashtag),
                                            music = new MusicModel
                                            {
                                                hashtag = Uri.EscapeDataString(slideSessionModel.Hashtag),
                                                isUsingGlobalVolume = true,
                                                lobbyEnabled = true,
                                                lobbyTitle = "Song3",
                                                lobbyVolume = 90,
                                                countdownRunningEnabled = true,
                                                countdownRunningTitle = "Song1",
                                                countdownRunningVolume = 90,
                                                countdownEndEnabled = true,
                                                countdownEndTitle = "LobbySong1",
                                                countdownEndVolume = 90
                                           },
                                           nicks = new NicksModel
                                                   {
                                                       hashtag = Uri.EscapeDataString(slideSessionModel.Hashtag),
                                                       selectedValues = new List<string>(),
                                                       blockIllegal = true,
                                                       restrictToCASLogin = false
                                                   },
                                           theme = "material",
                                           readingConfirmationEnabled = false,
                                           showResponseProgress = false
                       },
                       type = "DefaultQuestionGroup"
            };
        }

        private QuestionModel SlideQuestionModelToQuestionModel(SlideQuestionModel slideQuestionModel, string hashtag)
        {
            var questionModel =  new QuestionModel
                   {
                       hashtag = Uri.EscapeDataString(hashtag),
                       questionText = Uri.EscapeDataString(slideQuestionModel.QuestionText),
                       timer = slideQuestionModel.Countdown,
                       startTime = 0,
                       questionIndex = slideQuestionModel.RecalculatedOnlineIndex,
                       displayAnswerText = false,
                       type = this.QuestionTypeToClickQuestionType(slideQuestionModel.QuestionType)
            };

            if (slideQuestionModel.QuestionType == QuestionTypeEnum.RangedQuestionClick)
            {
                var rangedAnswerOption = slideQuestionModel.AnswerOptions.First();

                if (rangedAnswerOption == null)
                {
                    throw new ArgumentException("no answer options provided");
                }

                questionModel.rangeMin = rangedAnswerOption.RangedLowerLimit;
                questionModel.rangeMax = rangedAnswerOption.RangedHigherLimit;
                questionModel.correctValue = rangedAnswerOption.RangedCorrectValue;

                questionModel.answerOptionList = new List<AnswerOptionModel>();
            }
            else
            {
                var isFreetextAnswerOption = slideQuestionModel.QuestionType == QuestionTypeEnum.FreeTextClick;

                var answerOptionModelList =
                    slideQuestionModel.AnswerOptions.Select(a => this.SlideAnswerOptionModelToAnswerOptionModel(a, hashtag, slideQuestionModel.RecalculatedOnlineIndex, isFreetextAnswerOption))
                                      .ToList();

                questionModel.answerOptionList = answerOptionModelList;
            }

            return questionModel;
        }

        private AnswerOptionModel SlideAnswerOptionModelToAnswerOptionModel(GeneralAnswerOption answerOption, string hashtag, int questionIndex, bool isFreetextAnswerOption)
        {
            var answerOptionModel = new AnswerOptionModel
            {
                hashtag = Uri.EscapeDataString(hashtag),
                questionIndex = questionIndex,
                answerText = Uri.EscapeDataString(answerOption.Text),
                answerOptionNumber = answerOption.Position - 1,
                isCorrect = answerOption.IsTrue,
                type = isFreetextAnswerOption ? "FreeTextAnswerOption" : "DefaultAnswerOption"
            };

            if (isFreetextAnswerOption)
            {
                answerOptionModel.configCaseSensitive = answerOption.ConfigCaseSensitive;
                answerOptionModel.configTrimWhitespaces = answerOption.ConfigTrimWhitespaces;
                answerOptionModel.configUseKeywords = answerOption.ConfigUseKeywords;
                answerOptionModel.configUsePunctuation = answerOption.ConfigUsePunctuation;
            }

            return answerOptionModel;
        }

        private string QuestionTypeToClickQuestionType(QuestionTypeEnum questionType)
        {
            switch (questionType)
            {
                case QuestionTypeEnum.SingleChoiceClick:
                    return "SingleChoiceQuestion";
                case QuestionTypeEnum.MultipleChoiceClick:
                    return "MultipleChoiceQuestion";
                case QuestionTypeEnum.YesNoClick:
                    return "YesNoSingleChoiceQuestion";
                case QuestionTypeEnum.TrueFalseClick:
                    return "TrueFalseSingleChoiceQuestion";
                case QuestionTypeEnum.RangedQuestionClick:
                    return "RangedQuestion";
                case QuestionTypeEnum.FreeTextClick:
                    return "FreeTextQuestion";
                case QuestionTypeEnum.SurveyClick:
                    return "SurveyQuestion";
                default: return string.Empty;
            }
        }
    }
}
