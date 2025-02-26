using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMvcAzureEntraID.Models;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace WebMvcAzureEntraID.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient; ;
        }

        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public async Task<IActionResult> Index()
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();
            ViewData["GraphApiResult"] = user.DisplayName;

            // Obtém informações do tenant
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            try
            {

                // Obtém tentativas de login recentes
                var signIns = await _graphServiceClient.AuditLogs.SignIns.Request().GetAsync();
                ViewData["RecentSignIns"] = signIns;
            }
            catch (Exception)
            {

                ViewData["RecentSignIns"] = "Tenant does not have a premium license.";
            }

            // Obtém usuários do tenant
            var users = await _graphServiceClient.Users.Request().GetAsync();
            ViewData["Users"] = users;

            // Obtém grupos do tenant
            var groups = await _graphServiceClient.Groups.Request().GetAsync();
            ViewData["Groups"] = groups;

            // Adiciona TenantId ao ViewData
            ViewData["TenantId"] = tenantId;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
