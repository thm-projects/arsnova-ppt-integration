using System;
using ARSnovaPPIntegration.Common.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARSnovaPPIntegration.Presentation.Configuration;
using ARSnovaPPIntegration.Communication.Contract;
using Microsoft.Practices.Unity;

namespace ARSnovaPPIntegration.Test
{
    [TestClass]
    public class ArsnovaCommunicationTests
    {
        private readonly IArsnovaEuService arsnovaEuService;

        public ArsnovaCommunicationTests()
        {
            var unityContainer = Bootstrapper.GetRegisteredUnityContainer();
            this.arsnovaEuService = unityContainer.Resolve<IArsnovaEuService>();
        }

        [TestMethod]
        public void Login()
        {
            this.arsnovaEuService.Login(LoginMethod.Guest);
        }

        [TestMethod]
        public void ArsnovaEuCreateSession()
        {
            var sessionData = this.arsnovaEuService.CreateNewSession();

            Assert.IsFalse(string.IsNullOrEmpty(sessionData.keyword));
            Assert.IsTrue(sessionData.keyword.Length == 8);
        }
    }
}
