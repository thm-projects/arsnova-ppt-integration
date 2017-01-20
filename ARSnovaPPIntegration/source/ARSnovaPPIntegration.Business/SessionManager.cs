using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Communication.Model.ArsnovaClick;

namespace ARSnovaPPIntegration.Business
{
    public class SessionManager : ISessionManager
    {
        private readonly ISlideManipulator slideManipulator;

        private readonly IArsnovaClickService arsnovaClickService;

        private readonly IArsnovaEuService arsnovaVotingService;

        private readonly ILocalizationService localizationService;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private int countdown = 0;

        private SlideQuestionModel currentQuestionModel;

        private SlideSessionModel currentSlideSessionModel;

        private Slide questionSlide;

        private Slide resultsSlide;

        private Timer timer;

        public event EventHandler ShowNextSlideEventHandler;

        public SessionManager(
            ISlideManipulator slideManipulator,
            IArsnovaClickService arsnovaClickService,
            IArsnovaEuService arsnovaEuService,
            ILocalizationService localizationService,
            ISessionInformationProvider sessionInformationProvider)
        {
            this.slideManipulator = slideManipulator;
            this.arsnovaClickService = arsnovaClickService;
            this.arsnovaVotingService = arsnovaEuService;
            this.localizationService = localizationService;
            this.sessionInformationProvider = sessionInformationProvider;
        }

        public ValidationResult SetSession(SlideSessionModel slideSessionModel)
        {
            var validationResult = new ValidationResult();
            if (slideSessionModel.SessionType == SessionType.ArsnovaClick)
            {
                validationResult = this.SetArsnovaClickOnlineSession(slideSessionModel);

                if (!validationResult.Success)
                {
                    return validationResult;
                }
            }

            if (slideSessionModel.SessionType == SessionType.ArsnovaVoting)
            {
                // TODO voting
            }

            return validationResult;
        }

        public ValidationResult ActivateClickSession(SlideSessionModel slideSessionModel)
        {
            // push data to server
            var validationResult = this.SetSession(slideSessionModel);

            if (!validationResult.Success)
            {
                return validationResult;
            }

            // set question as active
            return this.arsnovaClickService.MakeSessionAvailable(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);
        }

        public ValidationResult SetHashtag(SlideSessionModel slideSessionModel)
        {
            var tupleResult = this.arsnovaClickService.CreateHashtag(slideSessionModel.Hashtag);
            slideSessionModel.PrivateKey = tupleResult.Item2;

            return tupleResult.Item1;
        }

        private ValidationResult SetArsnovaClickOnlineSession(SlideSessionModel slideSessionModel)
        {
            var validationResult = new ValidationResult();

            var allHashtagInfos = this.arsnovaClickService.GetAllHashtagInfos();

            if (string.IsNullOrEmpty(slideSessionModel.Hashtag))
            {
                validationResult.FailureTitel = this.localizationService.Translate("Error during session setup");
                validationResult.FailureMessage = this.localizationService.Translate("Hashtag is needed.");

                return validationResult;
            }

            var alreadyCreatedHashtag = false;

            if (allHashtagInfos.Any(hi => hi.hashtag.ToLower() == slideSessionModel.Hashtag.ToLower()))
            {
                if (!this.arsnovaClickService.IsThisMineHashtag(slideSessionModel.Hashtag, slideSessionModel.PrivateKey))
                {
                    validationResult.FailureTitel = this.localizationService.Translate("Error during session setup");
                    validationResult.FailureMessage =
                        this.localizationService.Translate("Can't create session, hashtag is already taken.");

                    return validationResult;
                }
                else
                {
                    alreadyCreatedHashtag = true;
                }
            }

            if (!alreadyCreatedHashtag)
            {
                validationResult = this.SetHashtag(slideSessionModel);

                if (!validationResult.Success)
                {
                    return validationResult;
                }
            }

            validationResult = this.arsnovaClickService.UpdateQuestionGroup(slideSessionModel);

            return validationResult;
        }

        public void RemoveClickQuizDataFromServer(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.arsnovaClickService.RemoveQuizData(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                throw new CommunicationException(validationResult.FailureMessage);
            }
        }

        public void StartSession(SlideSessionModel slideSessionModel, int questionIndex, Slide questionSlide, Slide resultsSlide)
        {
            var validationResult = new ValidationResult();

            var slideQuestionModel = slideSessionModel.Questions.First(q => q.Index == questionIndex);

            this.questionSlide = questionSlide;
            this.resultsSlide = resultsSlide;
            this.currentSlideSessionModel = slideSessionModel;
            this.currentQuestionModel = slideQuestionModel;

            var clickQuesitonTypes = this.sessionInformationProvider.GetAvailableQuestionsClick();

            if (clickQuesitonTypes.Any(qt => qt.QuestionTypeEnum == slideQuestionModel.QuestionType))
            {
                // start click question
                validationResult = this.arsnovaClickService.StartNextQuestion(slideSessionModel, questionIndex);

                // set current question model
                this.currentQuestionModel = slideQuestionModel;

                // add timer to slide
                this.countdown = slideQuestionModel.Countdown;
                this.slideManipulator.InitTimerOnSlide(this.questionSlide, this.countdown);

                this.timer = new Timer(1000);
                this.timer.Elapsed += this.HandleTimerEvent;
                this.timer.Start();
            }
            else
            {
                // TODO start voting question
            }

            if (!validationResult.Success)
            {
                throw new CommunicationException(validationResult.FailureMessage);
            }
        }

        private void HandleTimerEvent(object source, ElapsedEventArgs e)
        {
            this.countdown--;
            if(this.countdown >= 0)
                this.slideManipulator.SetTimerOnSlide(this.resultsSlide, this.countdown);

            if (this.countdown == 0)
            {
                this.timer.Stop();

                var responses = this.arsnovaClickService.GetResultsForHashtag(this.currentSlideSessionModel.Hashtag, this.currentQuestionModel.Index);
                this.PublishCurrentResultsClick(responses);

                // move to next slide
                // this.ShowNextSlideEventHandler?.Invoke(this, EventArgs.Empty);

                // clean up
                this.timer = null;
                this.currentQuestionModel = null;
                this.currentSlideSessionModel = null;
                this.questionSlide = null;
                this.resultsSlide = null;
            }
        }

        private void PublishCurrentResultsClick(List<ResultModel> responses)
        {
            responses = this.FilterForCorrectResponsesClick(responses);

            var best10Responses = new List<ResultModel>();

            for(var i = 0; i < 10; i++)
            {
                if (responses.Count == 0)
                    break;

                var minResponse = responses.First(r => r.responseTime == responses.Min(r2 => r2.responseTime));
                best10Responses.Add(minResponse);
                responses.Remove(minResponse);
            }

            this.slideManipulator.SetResultsOnSlide(this.resultsSlide, best10Responses);
        }

        private List<ResultModel> FilterForCorrectResponsesClick(List<ResultModel> responses)
        {
            var correctResponses = new List<ResultModel>();

            var correctAnswerOptionPositions = this.currentQuestionModel.AnswerOptions.Where(ao => ao.IsTrue).Select(ao => ao.Position).Select(correctAnswerOptionPosition => correctAnswerOptionPosition - 1).ToList();
            var correctAnswerOptionPositionsCount = correctAnswerOptionPositions.Count();

            switch (this.currentQuestionModel.QuestionType)
            {
                case QuestionTypeEnum.SingleChoiceClick:
                case QuestionTypeEnum.YesNoClick:
                case QuestionTypeEnum.TrueFalseClick:
                    var correctAnswerOptionPosition = correctAnswerOptionPositions.First();
                    foreach (var response in responses)
                    {
                        if (response.answerOptionNumber.First() == correctAnswerOptionPosition)
                            correctResponses.Add(response);
                    }
                    break;
                case QuestionTypeEnum.MultipleChoiceClick:
                    foreach (var response in responses)
                    {
                        if (correctAnswerOptionPositionsCount == response.answerOptionNumber.Count)
                        {
                            var allCorrect = true;

                            foreach (var answerOption in response.answerOptionNumber)
                            {
                                if (correctAnswerOptionPositions.All(ca => ca != answerOption))
                                {
                                    allCorrect = false;
                                }
                            }

                            if (allCorrect)
                                correctResponses.Add(response);
                        }
                    }
                    break;
                case QuestionTypeEnum.RangedQuestionClick:
                    foreach (var response in responses)
                    {
                        // TODO
                    }
                    break;
                case QuestionTypeEnum.FreeTextClick:
                    // TODO
                    break;
                case QuestionTypeEnum.SurveyClick:
                    correctResponses = responses;
                    break;
            }

            return correctResponses;
        }
    }
}
