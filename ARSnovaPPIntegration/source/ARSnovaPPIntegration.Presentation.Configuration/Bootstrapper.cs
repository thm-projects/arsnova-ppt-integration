using System;
using System.Reflection.Emit;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using ARSnovaPPIntegration.Business;
using ARSnovaPPIntegration.Business.Contract;
using ARSnovaPPIntegration.Common;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Communication;
using ARSnovaPPIntegration.Communication.Contract;

namespace ARSnovaPPIntegration.Presentation.Configuration
{
    public class Bootstrapper
    {
        public static IServiceLocator GetUnityServiceLocator()
        {
            var unityContainer = GetRegisteredUnityContainer();

            return new UnityServiceLocator(unityContainer);
        }

        private static IUnityContainer GetRegisteredUnityContainer()
        {
            var unityContainer = new UnityContainer();

            // Type registration
            unityContainer
                .RegisterType<ILocalizationService, LocalizationService>()
                    //new PerResolveLifetimeManager(),
                    //new InjectionConstructor())
                /*.RegisterType<IExceptionHandler, UiExceptionHandler>(
                new PerResolveLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<ILocalizationService>()))*/
                .RegisterType<IArsnovaEuService, ArsnovaEuService>()
                .RegisterType<ISlideManipulator, SlideManipulator>()
                .RegisterType<IArsnovaClickService, ArsnovaClickService>();

            // Factory registration
            unityContainer.RegisterType<Func<ILocalizationService>>();
            unityContainer.RegisterType<Func<IArsnovaEuService>>();
            unityContainer.RegisterType<Func<IArsnovaClickService>>();
            unityContainer.RegisterType<Func<ISlideManipulator>>();

            return unityContainer;
        }
    }
}
