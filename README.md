# WebAzureEntraID

Este projeto utiliza WEB API, MVC, Blazor e autenticação com Azure Entra ID.

## Funcionalidades

- Autenticação com Azure Entra ID
- Exibição de informações do usuário autenticado:
  - Nome
  - ID do Tenant
- Exibição de informações adicionais relacionadas ao Tenant:
  - Tentativas de login recentes
  - Usuários do Tenant
  - Grupos do Tenant
  - Outras informações relevantes

## Adicionando Blazor ao Projeto

1. Instale o SDK do .NET 7.0 (ou superior).
2. Crie um novo projeto Blazor:
   ```bash
   dotnet new blazorserver -o BlazorApp
   ```
3. Integre o projeto Blazor ao projeto existente.
4. Adicione as seguintes referências ao arquivo `.csproj`:
   ```xml
   <ItemGroup>
     <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="7.0.0" />
     <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="7.0.0" PrivateAssets="all" />
     <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="7.0.0" />
   </ItemGroup>
   ```
5. Configure o Blazor no arquivo `Program.cs`:
   ```csharp
   using Microsoft.AspNetCore.Components;
   using Microsoft.AspNetCore.Components.Web;

   var builder = WebApplication.CreateBuilder(args);

   // Adicione serviços Blazor
   builder.Services.AddRazorComponents();
   builder.Services.AddServerSideBlazor();

   var app = builder.Build();

   // Configure o middleware Blazor
   if (!app.Environment.IsDevelopment())
   {
       app.UseExceptionHandler("/Error");
       app.UseHsts();
   }

   app.UseHttpsRedirection();
   app.UseStaticFiles();
   app.UseRouting();

   app.MapBlazorHub();
   app.MapFallbackToPage("/_Host");

   app.Run();
   ```

6. Crie um novo componente Blazor, por exemplo, `Pages/FetchData.razor`:
   ```razor
   @page "/fetchdata"
   @using System.Net.Http.Json
   @inject HttpClient Http

   <h3>Weather forecast</h3>

   @if (forecasts == null)
   {
       <p><em>Loading...</em></p>
   }
   else
   {
       <table class="table">
           <thead>
               <tr>
                   <th>Date</th>
                   <th>Temp. (C)</th>
                   <th>Temp. (F)</th>
                   <th>Summary</th>
               </tr>
           </thead>
           <tbody>
               @foreach (var forecast in forecasts)
               {
                   <tr>
                       <td>@forecast.Date.ToShortDateString()</td>
                       <td>@forecast.TemperatureC</td>
                       <td>@forecast.TemperatureF</td>
                       <td>@forecast.Summary</td>
                   </tr>
               }
           </tbody>
       </table>
   }

   @code {
       private WeatherForecast[]? forecasts;

       protected override async Task OnInitializedAsync()
       {
           forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
       }

       public class WeatherForecast
       {
           public DateTime Date { get; set; }
           public int TemperatureC { get; set; }
           public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
           public string? Summary { get; set; }
       }
   }
   ```

7. Configure a autenticação do Azure Entra ID conforme a [documentação oficial](https://learn.microsoft.com/azure/active-directory/develop/tutorial-v2-blazor-server).
