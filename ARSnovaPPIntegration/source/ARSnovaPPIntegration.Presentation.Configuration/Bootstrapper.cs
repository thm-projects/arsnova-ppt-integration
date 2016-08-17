using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARSnovaPPIntegration.Common;
using ARSnovaPPIntegration.Common.Contract;
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
                .RegisterType<ILocalizationService, LocalizationService>();

            // Factory registration

            unityContainer.RegisterType<Func<ILocalizationService>>();



            return unityContainer;
        }
    }
}
