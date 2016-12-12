using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Business.Model;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication.Contract;

namespace ARSnovaPPIntegration.Business
{
    public class SessionManager : ISessionManager
    {
        private readonly ISlideManipulator slideManipulator;

        private readonly IArsnovaClickService arsnovaClickService;

        private readonly IArsnovaEuService arsnovaVotingService;

        private readonly ILocalizationService localizationService;

        public SessionManager()
        {
            this.slideManipulator = ServiceLocator.Current.GetInstance<ISlideManipulator>();
            this.arsnovaClickService = ServiceLocator.Current.GetInstance<IArsnovaClickService>();
            this.arsnovaVotingService = ServiceLocator.Current.GetInstance<IArsnovaEuService>();
            this.localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
        }

        public ValidationResult CreateSession(SlideSessionModel slideSessionModel)
        {
            var validationResult = new ValidationResult();
            if (slideSessionModel.SessionType == SessionType.ArsnovaClick)
            {
                validationResult = this.SetupArsnovaClickOnlineSession(slideSessionModel);

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

        public ValidationResult EditSession(SlideSessionModel slideSessionModel)
        {
            throw new NotImplementedException();
        }

        private ValidationResult SetupArsnovaClickOnlineSession(SlideSessionModel slideSessionModel)
        {
            var validationResult = new ValidationResult();

            var allHashtagInfos = this.arsnovaClickService.GetAllHashtagInfos();

            if (string.IsNullOrEmpty(slideSessionModel.Hashtag))
            {
                validationResult.FailureTitel = this.localizationService.Translate("Error during session setup");
                validationResult.FailureMessage = this.localizationService.Translate("Hashtag is needed.");

                return validationResult;
            }

            if (allHashtagInfos.Any(hi => hi.hashtag.ToLower() == slideSessionModel.Hashtag.ToLower()))
            {
                validationResult.FailureTitel = this.localizationService.Translate("Error during session setup");
                validationResult.FailureMessage = this.localizationService.Translate("Can't create session, hashtag is already taken.");

                return validationResult;
            }

            // TODO setup

            return validationResult;
        }
    }
}
