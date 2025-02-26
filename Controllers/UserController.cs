using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Security.Claims;

namespace WebAzureEntraID.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;

    public UserController(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            // Obtém o nome e ID do usuário autenticado
            var userName = User.Identity.Name;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Obtém informações do tenant
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Obtém tentativas de login recentes
            var signIns = await _graphServiceClient.AuditLogs.SignIns.GetAsync();

            // Obtém usuários do tenant
            var users = await _graphServiceClient.Users.GetAsync();

            // Obtém grupos do tenant
            var groups = await _graphServiceClient.Groups.GetAsync();

            return Ok(new
            {
                UserName = userName,
                UserId = userId,
                TenantId = tenantId,
                RecentSignIns = signIns,
                Users = users,
                Groups = groups
            });
        }
        catch (ServiceException ex)
        {
            return StatusCode(500, $"Erro ao acessar Microsoft Graph: {ex.Message}");
        }
    }
}