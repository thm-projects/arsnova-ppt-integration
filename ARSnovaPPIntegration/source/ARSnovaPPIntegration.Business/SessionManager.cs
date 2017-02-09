using System;
using System.Linq;
using System.Timers;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Communication.Contract;

namespace ARSnovaPPIntegration.Business
{
    public class SessionManager : ISessionManager
    {
        private readonly ISlideManipulator slideManipulator;

        private readonly IArsnovaClickService arsnovaClickService;

        private readonly IArsnovaVotingService arsnovaVotingService;

        private readonly ILocalizationService localizationService;

        private readonly ISessionInformationProvider sessionInformationProvider;

        private int countdown = 0;

        private SlideQuestionModel currentQuestionModel;

        private SlideSessionModel currentSlideSessionModel;

        private Slide resultsSlide;

        private Slide questionTimerSlide;

        private Timer timer;

        public event EventHandler ShowNextSlideEventHandler;

        public SessionManager(
            ISlideManipulator slideManipulator,
            IArsnovaClickService arsnovaClickService,
            IArsnovaVotingService arsnovaVotingService,
            ILocalizationService localizationService,
            ISessionInformationProvider sessionInformationProvider)
        {
            this.slideManipulator = slideManipulator;
            this.arsnovaClickService = arsnovaClickService;
            this.arsnovaVotingService = arsnovaVotingService;
            this.localizationService = localizationService;
            this.sessionInformationProvider = sessionInformationProvider;
        }

        public ValidationResult SetClickSession(SlideSessionModel slideSessionModel)
        {
            // arsnova sessions are updated while they are changed -> should not called by arsnova sessions, nothing to do here
            return this.SetArsnovaClickOnlineSession(slideSessionModel);
        }

        public ValidationResult ActivateClickSession(SlideSessionModel slideSessionModel)
        {
            // push data to server
            var validationResult = this.SetClickSession(slideSessionModel);

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

        public void KeepAlive(SlideSessionModel slideSessionModel)
        {
            var validationResult = this.arsnovaClickService.KeepAlive(slideSessionModel.Hashtag, slideSessionModel.PrivateKey);

            if (!validationResult.Success)
            {
                throw new CommunicationException(validationResult.FailureMessage);
            }
        }

        public void StartSession(SlideSessionModel slideSessionModel, int questionIndex, Slide questionTimerSlideParam, Slide resultsSlideParam)
        {
            var validationResult = new ValidationResult();

            this.currentQuestionModel = slideSessionModel.Questions.First(q => q.Index == questionIndex);

            this.questionTimerSlide = questionTimerSlideParam;
            this.resultsSlide = resultsSlideParam;
            this.currentSlideSessionModel = slideSessionModel;

            var clickQuesitonTypes = this.sessionInformationProvider.GetAvailableQuestionsClick();

            if (clickQuesitonTypes.Any(qt => qt.QuestionTypeEnum == this.currentQuestionModel.QuestionType))
            {
                // start click question
                validationResult = this.arsnovaClickService.StartNextQuestion(slideSessionModel, this.currentQuestionModel.RecalculatedOnlineIndex);

                this.countdown = this.currentQuestionModel.Countdown;
                this.timer = new Timer(1000);
                this.timer.Elapsed += this.HandleTimerEvent;
                this.timer.Start();
            }
            else
            {
                // TODO start voting question
                //validationResult = this.arsnovaClickService.StartNextQuestion(slideSessionModel, questionIndex);
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
                this.slideManipulator.SetTimerOnSlide(this.currentQuestionModel, this.questionTimerSlide, this.countdown);

            if (this.countdown == 0)
            {
                this.timer.Stop();

                var responses = this.arsnovaClickService.GetResultsForHashtag(this.currentSlideSessionModel.Hashtag, this.currentQuestionModel.RecalculatedOnlineIndex);
                if (responses != null)
                {
                    this.slideManipulator.SetResults(this.currentQuestionModel, this.resultsSlide, responses);
                }


                // move to next slide (results)
                this.ShowNextSlideEventHandler?.Invoke(this, EventArgs.Empty);

                // clean up
                this.timer = null;
                this.currentQuestionModel = null;
                this.currentSlideSessionModel = null;
                this.questionTimerSlide = null;
                this.resultsSlide = null;
            }
        }        
    }
}
