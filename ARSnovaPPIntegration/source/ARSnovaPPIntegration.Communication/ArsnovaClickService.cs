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
        private ArsnovaClickApi arsnovaClickApi;

        private readonly ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel> answerOptionMapper;

        private readonly ObjectMapper<SessionConfigurationWithId, SessionConfiguration> sessionConfigurationMapper;

        public ArsnovaClickService()
        {
            this.answerOptionMapper = new ObjectMapper<AnswerOptionModelWithId, AnswerOptionModel>();

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

        public SessionConfiguration GetSessionConfiguration(string hashtag)
        {
            var sessionConfigurationReturnModel = this.arsnovaClickApi.GetSessionConfiguration(hashtag);

            var sessionConfiguration = new SessionConfiguration();

            this.sessionConfigurationMapper.Map(sessionConfigurationReturnModel.sessionConfiguration.FirstOrDefault(), sessionConfiguration);

            return sessionConfiguration;
        }

        public Tuple<ValidationResult, string> CreateHashtag(string hashtag)
        {
            // Temporary: One private key per question
            var privateKey = string.Empty;

            try
            {
                privateKey = this.arsnovaClickApi.NewPrivateKey();
            }
            catch (CommunicationException comException)
            {
                return new Tuple<ValidationResult, string>(
                    new ValidationResult
                    {
                        FailureTitel = "Error - create hashtag",
                        FailureMessage = comException.Message
                    },
                    privateKey);
            }

            var validationResult = this.arsnovaClickApi.AddHashtag(hashtag, privateKey);

            return new Tuple<ValidationResult, string>(validationResult, privateKey);
        }

        public ValidationResult ResetSession(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.ResetSession(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);
        }

        public ValidationResult UpdateQuestionGroup(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            validationResult = this.ValidateValidQuestionGroup(slideSessionModel);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.UpdateQuestionGroup(this.SlideSessionModelToQuestionGroupModel(slideSessionModel), slideSessionModel.PrivateKey);
        }

        public ValidationResult ShowNextReadingConfirmation(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.ShowNextReadingConfirmation(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);
        }

        public ValidationResult StartNextQuestion(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.CheckForHashtagAndPrivateKey(slideSessionModel);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            return this.arsnovaClickApi.StartNextQuestion(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);
        }

        private ValidationResult CheckForHashtagAndPrivateKey(SlideSessionModel slideSessionModel)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(slideSessionModel.Hashtag))
            {
                validationResult.FailureTitel = "Error -  hashtag";
                validationResult.FailureMessage = "No hashtag provided";
                return validationResult;
            }

            if (string.IsNullOrEmpty(slideSessionModel.PrivateKey))
            {
                validationResult.FailureTitel = "Error -  private key";
                validationResult.FailureMessage = "No private key provided";
                return validationResult;
            }

            return validationResult;
        }

        private ValidationResult ValidateValidQuestionGroup(SlideSessionModel slideSessionModel)
        {
            var validationResult = new ValidationResult();

            // TODO

            return validationResult;
        }

        private QuestionGroupModel SlideSessionModelToQuestionGroupModel(SlideSessionModel slideSessionModel)
        {
            // TODO  many default values

            var questionModelList =
                slideSessionModel.Questions.Select(q => this.SlideQuestionModelToQuestionModel(q, slideSessionModel.Hashtag)).ToList();

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
                                                       isEnabled = true,
                                                       volume = 90,
                                                       title = "Song3",
                                                       isLobbyEnabled = true,
                                                       lobbyTitle = "LobbySong1",
                                                       finishSoundTitle = "LobbySong1"
                                                   },
                                           nicks = new NicksModel
                                                   {
                                                       hashtag = Uri.EscapeDataString(slideSessionModel.Hashtag),
                                                       selectedValues = new List<string>(),
                                                       blockIllegal = true,
                                                       restrictToCASLogin = false
                                                   },
                                           theme = "theme-arsnova-dot-click-contrast",
                                           readingConfirmationEnabled = false
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
                       timer = 60,
                       startTime = 0,
                       questionIndex = slideQuestionModel.Index,
                       displayAnswerText = false,
                       type = this.QuestionTypeToClickQuestionType(slideQuestionModel.QuestionType)
            };

            if (slideQuestionModel.QuestionType == QuestionTypeEnum.RangedQuestionClick)
            {
                var rangedAnswerOption = slideQuestionModel.AnswerOptions.First() as RangedAnswerOption;

                if (rangedAnswerOption == null)
                {
                    throw new ArgumentException("no answer options provided");
                }

                questionModel.rangeMin = rangedAnswerOption.LowerLimit;
                questionModel.rangeMax = rangedAnswerOption.HigherLimit;
                questionModel.correctValue = rangedAnswerOption.Correct;
            }
            else
            {
                var isFreetextAnswerOption = slideQuestionModel.QuestionType == QuestionTypeEnum.FreeTextClick;

                var answerOptionModelList =
                    slideQuestionModel.AnswerOptions.Select(a => this.SlideAnswerOptionModelToAnswerOptionModel(a, hashtag, slideQuestionModel.Index, isFreetextAnswerOption))
                                      .ToList();

                questionModel.answerOptionList = answerOptionModelList;
            }

            return questionModel;
        }

        private AnswerOptionModel SlideAnswerOptionModelToAnswerOptionModel(object answerOption, string hashtag, int questionIndex, bool isFreetextAnswerOption)
        {
            if (answerOption.GetType() == typeof(GeneralAnswerOption))
            {
                var castedAnswerOption = answerOption as GeneralAnswerOption;

                return new AnswerOptionModel

                {
                    hashtag = Uri.EscapeDataString(hashtag),
                    questionIndex = questionIndex,
                    answerText = Uri.EscapeDataString(castedAnswerOption.Text),
                    answerOptionNumber = castedAnswerOption.Position,
                    isCorrect = castedAnswerOption.IsTrue,
                    type = isFreetextAnswerOption ? "FreeTextAnswerOption" : "DefaultAnswerOption"

                };
            }

            if (answerOption.GetType() == typeof(RangedAnswerOption))
            {
                return null;
            }

            throw new ArgumentException($"Unknow answer option type {answerOption.GetType()}");
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
                default: return String.Empty;
            }
        }
    }
}
