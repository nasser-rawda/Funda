using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Funda;
using Funda.Controllers;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Funda.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        private async Task<List<Api.AgentModel>> GetTopAgentsAmsteramTest() {
            Api.AdvertisementProcessor adPro = new Api.AdvertisementProcessor();
            return await adPro.GetTopAgentsAsync("koop", "amsterdam");
        }

        private async Task<List<Api.AgentModel>> GetTopAgentsAmsteramTuinTest()
        {
            Api.AdvertisementProcessor adPro = new Api.AdvertisementProcessor();
            return await adPro.GetTopAgentsAsync("koop", "amsterdam/tuin");
        }

        [TestMethod]
        public void TopAgents()
        {
            var agents = AsyncContext.Run(GetTopAgentsAmsteramTest);
            // Assert
            Assert.AreEqual(10, agents.Count());
        }

        [TestMethod]
        public void TopAgentsWithTuin()
        {
            var agents = AsyncContext.Run(GetTopAgentsAmsteramTuinTest);
            // Assert
            Assert.AreEqual(10, agents.Count());
        }

    }
}
