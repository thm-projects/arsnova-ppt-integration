using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Common.Enum;
using ARSnovaPPIntegration.Communication;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARSnovaPPIntegration.Test.Communication
{
    [TestClass]
    public class ArsnovaCommunicationTests
    {
        private readonly IArsnovaEuService arsnovaEuService;

        public ArsnovaCommunicationTests()
        {
            this.arsnovaEuService = new ArsnovaEuService();
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
