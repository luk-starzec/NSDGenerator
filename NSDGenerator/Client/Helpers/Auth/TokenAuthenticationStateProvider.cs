using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NSDGenerator.Client.Helpers.Auth;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public TokenAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("token");

        var isValid = !string.IsNullOrWhiteSpace(token) && CheckExpTime(token);

        var identity = isValid
            ? new ClaimsIdentity(token.ParseClaimsFromJwt(), "jwt")
            : new ClaimsIdentity();

        _httpClient.DefaultRequestHeaders.Authorization = isValid
            ? new AuthenticationHeaderValue("bearer", token)
            : null;

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private bool CheckExpTime(string token)
    {
        var claims = token.ParseClaimsFromJwt();
        var expiry = claims.Where(claim => claim.Type.Equals("exp")).FirstOrDefault();
        if (expiry == null)
            return false;

        var datetime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiry.Value));
        if (datetime.UtcDateTime <= DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task Login(string token)
    {
        await _localStorage.SetItemAsync("token", token);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("token");

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void RefreshAuthenticationState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
