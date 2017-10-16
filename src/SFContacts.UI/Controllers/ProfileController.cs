using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Fabric.Query;
using SFContacts.SessionKeys.Domain;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Threading;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Client;
using SFContacts.UI.Security;

namespace SFContacts.UI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        static FabricClient fc = new FabricClient();

        public ProfileController(ISessionService sessionService) {
            SessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public async Task<IActionResult> Index()
        {
            ViewData["currentUser"] = HttpContext.User?.Identity?.Name;
            ViewData["currentClaim"] = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            ServiceUriBuilder builder = new ServiceUriBuilder("SFContacts.SessionKeys");
            var serviceName = builder.ToUri();
            ServicePartitionList partitions = await fc.QueryManager.GetPartitionListAsync(serviceName);

            List<SessionKeyItem> keys = new List<SessionKeyItem>();

            try {
                foreach (var partition in partitions) {
                    long minKey = (partition.PartitionInformation as Int64RangePartitionInformation).LowKey;
                    var keysService = ServiceProxy.Create<ISessionKeysService>(serviceName, new ServicePartitionKey(minKey));
                    keys.AddRange(await keysService.GetKeys(CancellationToken.None));
                }
            }
            catch (Exception ex) {
                ServiceEventSource.Current.Message($"{nameof(ProfileController)}->Index() failed to obtain Protection Keys with error: {ex.ToString()}");
            }
            ViewData["ProtectionKeys"] = keys;

            var cookies = new Dictionary<string, string>();
            foreach (var cookie in Request.Cookies) {
                cookies.Add(cookie.Key, cookie.Value);
            }
            ViewData["Cookies"] = cookies;

            if(string.IsNullOrEmpty(await SessionService.GetSessionItem<string>("Username"))) {
                await SessionService.AddSessionItem("Username", HttpContext.User.Identity.Name);
            }

            return View();
        }

        ISessionService SessionService { get; }
    }
}