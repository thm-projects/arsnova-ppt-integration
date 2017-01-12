using System;
using System.Linq;
using System.Timers;
using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Exceptions;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;

using Microsoft.Office.Interop.PowerPoint;

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

        public SessionManager()
        {
            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();
            this.arsnovaClickService = ServiceLocator.Current.GetInstance<IArsnovaClickService>();
            this.arsnovaVotingService = ServiceLocator.Current.GetInstance<IArsnovaEuService>();
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
            this.sessionInformationProvider = ServiceLocator.Current.GetInstance<ISessionInformationProvider>();
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
                this.slideManipulator.InitTimerOnSlide(this.resultsSlide, this.countdown);

                timer = new Timer(1000);
                timer.Elapsed += this.HandleTimerEvent;
                timer.Start();

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
            if (this.countdown <= 0)
            {
                var questionResults = this.arsnovaClickService.GetResultsForHashtag(this.currentSlideSessionModel.Hashtag);
                // TODO
                // add results to next slide (on end)

                // move to next slide
                this.ShowNextSlideEventHandler?.Invoke(this, EventArgs.Empty);

                // clean up
                this.timer.Stop();
                this.timer = null;
                this.currentQuestionModel = null;
                this.currentSlideSessionModel = null;
                this.questionSlide = null;
                this.resultsSlide = null;
            }
            else
            {
                this.slideManipulator.SetTimerOnSlide(this.resultsSlide, this.countdown);
            }
        }
    }
}
