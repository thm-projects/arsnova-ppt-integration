using Microsoft.VisualStudio.TestTools.UnitTesting;

using ARSnovaPPIntegration.Communication;
using ARSnovaPPIntegration.Communication.Contract;

namespace ARSnovaPPIntegration.Test.Communication
{
    [TestClass]
    public class ClickCommunicationTests
    {
        private readonly IArsnovaClickService arsnovaClickService;

        // TODO Add encoding: hashtag = Uri.EscapeDataString(hashtag)
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
        public void TestAddSession()
        {
            // TODO
            // test addHashtag
            /*var newRandomHashtag = Guid.NewGuid();
            var allHashtagsCount = this.arsnovaClickService.GetAllHashtagInfos().Count;
            this.arsnovaClickService.*/

            // test question

            // test questions

            //test if sessions is online (user can join)
        }

        [TestMethod]
        public void GetQuestionGroup()
        {
            // TODO
        }

        [TestMethod]
        public void GetHashtagsTest()
        {
            var allHashtagInfos = this.arsnovaClickService.GetAllHashtagInfos();
            foreach (var allHashtagInfo in allHashtagInfos)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(allHashtagInfo.hashtag));
            }
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
