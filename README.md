# WebAzureEntraID

Este projeto utiliza WEB API, MVC, Blazor e autenticação com Azure Entra ID para fornecer informações sobre tentativas de login recentes, usuários do tenant, grupos do tenant e outras informações relevantes do Entra ID.

## Funcionalidades

- **Autenticação com Azure Entra ID**: Integração com o Microsoft Identity para autenticar os usuários.
- **Exibição de Informações do Usuário**:
  - Nome do Usuário
  - ID do Tenant
- **Informações do Tenant**:
  - Tentativas de Login Recentes
  - Usuários do Tenant
  - Grupos do Tenant
  - Outras Informações Relevantes

## Tecnologias Utilizadas

- ASP.NET Core MVC
- Blazor
- Microsoft Graph API
- Microsoft Identity

## Configuração do Projeto

### Pré-requisitos

- .NET SDK 7.0 ou superior
- Conta no Azure com permissões para acessar o Microsoft Entra ID

### Instalação

1. Clone o repositório:
   ```bash
   git clone https://github.com/gabrieldassie/WebAzureEntraID.git
   cd WebAzureEntraID
   ```

2. Instale as dependências necessárias:
   ```bash
   dotnet restore
   ```

3. Configure os serviços no arquivo `appsettings.json`:
   ```json
   {
     "AzureAd": {
       "Instance": "https://login.microsoftonline.com/",
       "Domain": "yourdomain.onmicrosoft.com",
       "TenantId": "your-tenant-id",
       "ClientId": "your-client-id",
       "ClientSecret": "your-client-secret",
       "CallbackPath": "/signin-oidc"
     },
     "MicrosoftGraph": {
       "Scopes": "User.Read.All Group.Read.All AuditLog.Read.All"
     }
   }
   ```

4. Execute o projeto:
   ```bash
   dotnet run
   ```

### Estrutura do Projeto

- **Controllers**: Contém os controladores da API.
  - `UserController.cs`: Controlador responsável por obter informações do usuário e do tenant.
- **Views**: Contém as views do projeto.
  - `Index.cshtml`: View principal que exibe as informações do usuário e do tenant.
- **Properties**: Contém configurações do projeto.
  - `serviceDependencies.local.json`: Arquivo de dependências do serviço.

### Exemplo de Uso

#### Controlador `UserController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAzureEntraID.Controllers
{
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

        [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
        public async Task<IActionResult> Index()
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();
            ViewData["GraphApiResult"] = user.DisplayName;

            // Obtém informações do tenant
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Obtém tentativas de login recentes
            var signIns = await _graphServiceClient.AuditLogs.SignIns.Request().GetAsync();
            ViewData["RecentSignIns"] = signIns;

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
    }
}
```

#### View `Index.cshtml`

````markdown name=Views/User/Index.cshtml
@{
    ViewData["Title"] = "User Information";
}

<h2>User Information</h2>

<div>
    <h3>Graph API Result</h3>
    <p>@ViewData["GraphApiResult"]</p>
</div>

<div>
    <h3>Tenant ID</h3>
    <p>@ViewData["TenantId"]</p>
</div>

<div>
    <h3>Tentativas de Login Recentes</h3>
    <ul>
        @foreach (var signIn in ViewData["RecentSignIns"] as IEnumerable<Microsoft.Graph.SignIn>)
        {
            <li>@signIn.UserPrincipalName - @signIn.CreatedDateTime</li>
        }
    </ul>
</div>

<div>
    <h3>Usuários do Tenant</h3>
    <ul>
        @foreach (var user in ViewData["Users"] as IEnumerable<Microsoft.Graph.User>)
        {
            <li>@user.DisplayName - @user.UserPrincipalName</li>
        }
    </ul>
</div>

<div>
    <h3>Grupos do Tenant</h3>
    <ul>
        @foreach (var group in ViewData["Groups"] as IEnumerable<Microsoft.Graph.Group>)
        {
            <li>@group.DisplayName</li>
        }
    </ul>
</div>