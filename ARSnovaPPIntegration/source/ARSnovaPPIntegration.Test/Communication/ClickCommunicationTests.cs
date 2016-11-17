using ARSnovaPPIntegration.Communication;
using ARSnovaPPIntegration.Communication.Contract;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ARSnovaPPIntegration.Test.Communication
{
    [TestClass]
    public class ClickCommunicationTests
    {
        private readonly IArsnovaClickService arsnovaClickService;

        public ClickCommunicationTests()
        {
            this.arsnovaClickService = new ArsnovaClickService();
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
