using System;
using ARSnovaPPIntegration.Common;
using ARSnovaPPIntegration.Common.Contract;
using ARSnovaPPIntegration.Communication;
using ARSnovaPPIntegration.Communication.Contract;
using Microsoft.Practices.Unity;

namespace ARSnovaPPIntegration.Presentation.Configuration
{
    public class Bootstrapper
    {
        public static IUnityContainer GetRegisteredUnityContainer()
        {
            var unityContainer = new UnityContainer();

            // Type registration

            unityContainer
                .RegisterType<ILocalizationService, LocalizationService>()
                .RegisterType<IArsnovaEuService, ArsnovaEuService>()
                .RegisterType<IArsnovaClickService, ArsnovaClickService>();

            // Factory registration

            unityContainer.RegisterType<Func<ILocalizationService>>();
            unityContainer.RegisterType<Func<IArsnovaEuService>>();
            unityContainer.RegisterType<Func<IArsnovaClickService>>();

            return unityContainer;
        }
    }
}
