using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Presentation.Configuration;

using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
