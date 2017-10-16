using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using SFContacts.UI.Security;

namespace SFContacts.UI.Controllers {
    public class HomeController : Controller
    {

        public HomeController(ISessionService sessionService) {
            SessionService = sessionService ?? throw new System.ArgumentNullException(nameof(sessionService));
        }
        public async Task<IActionResult> Index()
        {
            ViewData["CurrentUsername"] = await SessionService.GetSessionItem<string>("Username");
            return View();
        }

        public async Task<IActionResult> About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        ISessionService SessionService { get; }

    }
}
