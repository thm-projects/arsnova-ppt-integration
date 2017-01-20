using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using ARSnovaPPIntegration.Business;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Common;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Common.Contract.Translators;
using ARSnovaPPIntegration.Common.Translators;
using ARSnovaPPIntegration.Communication;
using ARSnovaPPIntegration.Communication.Contract;

namespace ARSnovaPPIntegration.Presentation.Configuration
{
    public class Bootstrapper
    {
        public IUnityContainer UnityContainer { get; private set; }

        public void SetupUnityServiceLocator()
        {
            this.UnityContainer = this.GetRegisteredUnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(this.UnityContainer));
        }

        private IUnityContainer GetRegisteredUnityContainer()
        {
            var unityContainer = new UnityContainer();

            // Type registration
            unityContainer
                .RegisterType<ILocalizationService, LocalizationService>(new ContainerControlledLifetimeManager())
                .RegisterType<IArsnovaEuService, ArsnovaEuService>(new ContainerControlledLifetimeManager())
                .RegisterType<ISlideManipulator, SlideManipulator>(new ContainerControlledLifetimeManager())
                .RegisterType<IArsnovaClickService, ArsnovaClickService>(new ContainerControlledLifetimeManager())
                .RegisterType<IQuestionTypeTranslator, QuestionTypeTranslator>(new ContainerControlledLifetimeManager())
                .RegisterType<ISessionInformationProvider, SessionInformationProvider>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<ISessionManager, SessionManager>(new ContainerControlledLifetimeManager());

            return unityContainer;
        }
    }
}
