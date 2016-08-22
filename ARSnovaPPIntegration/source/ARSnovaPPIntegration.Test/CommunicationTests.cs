using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARSnovaPPIntegration.Presentation.Configuration;
using ARSnovaPPIntegration.Communication.Contract;
using Microsoft.Practices.Unity;

namespace ARSnovaPPIntegration.Test
{
    [TestClass]
    public class CommunicationTests
    {
        private readonly IArsnovaEuService arsnovaEuService;

        public CommunicationTests()
        {
            var unityContainer = Bootstrapper.GetRegisteredUnityContainer();
            this.arsnovaEuService = unityContainer.Resolve<IArsnovaEuService>();
        }

        [TestMethod]
        public void ArsnovaEuCreateSession()
        {
            var sessionData = this.arsnovaEuService.CreateNewSession();
            // TODO check sessionData, which values are expected?
        }
    }
}
