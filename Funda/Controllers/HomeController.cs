using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Funda.Api;

namespace Funda.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> TopAgents()
        {
            try
            {
                AdvertisementProcessor adPro = new AdvertisementProcessor();

                List<AgentModel> agents = await adPro.GetTopAgentsAsync("koop", "amsterdam");
                return View(agents);
            }
            catch (Exception e) {
                return Content("Error: " + e.Message);
            }
        }

        public async Task<ActionResult> TopAgentsWithTuin()
        {
            try
            {
                AdvertisementProcessor adPro = new AdvertisementProcessor();

                List<AgentModel> agents = await adPro.GetTopAgentsAsync("koop", "amsterdam/tuin");

                return View(agents);
            }
            catch (Exception e) {
                return Content("Error: " + e.Message);
            }
        }
    }
}