using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Communication;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Test.Communication
{
    [TestClass]
    public class ArsnovaCommunicationTests
    {
        private readonly IArsnovaVotingService arsnovaVotingService;

        public ArsnovaCommunicationTests()
        {
            this.arsnovaVotingService = new ArsnovaVotingService();
        }

        [TestMethod]
        public void Login()
        {
            //var guestUserName = this.arsnovaVotingService.GenerateGuestName();
            //this.arsnovaVotingService.Login(guestUserName, LoginMethod.Guest);
        }

        [TestMethod]
        public void ArsnovaEuCreateSession()
        {
            var slideSessionModel = new SlideSessionModel();
            slideSessionModel.ArsnovaEuConfig = new ArsnovaEuConfig();
            this.arsnovaVotingService.CreateNewSession(slideSessionModel);

            Assert.IsFalse(string.IsNullOrEmpty(slideSessionModel.Hashtag));
            Assert.IsTrue(slideSessionModel.Hashtag.Length == 8);
        }
    }
}
