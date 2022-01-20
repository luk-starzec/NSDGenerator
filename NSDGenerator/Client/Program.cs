using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NSDGenerator.Client.Helpers;
using NSDGenerator.Client.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSDGenerator.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddHttpClient("NSDGenerator.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NSDGenerator.Server"));

        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<TokenAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());

        builder.Services.AddScoped<IThemeService, ThemeService>();
        builder.Services.AddSingleton<IModelConverterService, ModelConverterService>();
        builder.Services.AddScoped<IDiagramService, DiagramService>();
        builder.Services.AddSingleton<IColumnsHelper, ColumnsHelper>();
        builder.Services.AddSingleton<AppState>();

        await builder.Build().RunAsync();
    }
}
