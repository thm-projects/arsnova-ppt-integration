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
        public void FirstTest()
        {
            var allRestMethods = this.arsnovaClickService.GetAllRestMethods();
            Assert.IsTrue(string.IsNullOrEmpty(allRestMethods));

            var allHashtags = this.arsnovaClickService.FindAllHashtags();
            Assert.IsTrue(string.IsNullOrEmpty(allHashtags));
        }
    }
}
