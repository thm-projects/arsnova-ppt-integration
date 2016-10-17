using ARSnovaPPIntegration.Communication.Contract;
using ARSnovaPPIntegration.Presentation.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARSnovaPPIntegration.Test
{
    [TestClass]
    public class ClickCommunicationTests
    {
        private readonly IArsnovaClickService arsnovaClickService;

        public ClickCommunicationTests()
        {
            var unityContainer = Bootstrapper.GetRegisteredUnityContainer();
            this.arsnovaClickService = unityContainer.Resolve<IArsnovaClickService>();
        }

        [TestMethod]
        public void ApiPostTest()
        {
            var answerOptions = this.arsnovaClickService.GetAnswerOptionsForHashtag("TestHashtag");
            foreach (var answerOption in answerOptions)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(answerOption.hashtag));
            }
        }

        [TestMethod]
        public void GetHashtagsTest()
        {
            var allHashtags = this.arsnovaClickService.FindAllHashtags();
            Assert.IsFalse(string.IsNullOrWhiteSpace(allHashtags));
        }

        // Tests for existing sessions (which are currently not online)

        [TestMethod]
        public void GetSessionConfiguration()
        {
            var sessionConfiguration = this.arsnovaClickService.GetSessionConfiguration("TestHashtag");
            Assert.IsFalse(string.IsNullOrWhiteSpace(sessionConfiguration.theme));
        }
    }
}
