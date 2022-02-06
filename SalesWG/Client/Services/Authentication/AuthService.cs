using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using SalesWG.Shared.Constants;
using SalesWG.Shared.Models;
using SalesWG.Shared.Models.Identity;

namespace SalesWG.Client.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AppStateProvider _appStateProvider;

        public AuthService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AppStateProvider appStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _appStateProvider = appStateProvider;
        }

        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await _appStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

        public async Task<bool> IsUserAuthenticated()
        {
            return (await _appStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task<AppResponse> Login(TokenRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/account/Login", model);
            var result = await response.Content.ReadFromJsonAsync<AppResponse<TokenResponse>>();
            if (result != null && result.Success)
            {
                var token = result.Data.Token;
                var refreshToken = result.Data.RefreshToken;
                var userImageURL = result.Data.UserImageURL;
                await _localStorage.SetItemAsync(AppStorageConstants.Local.AuthToken, token);
                await _localStorage.SetItemAsync(AppStorageConstants.Local.RefreshToken, refreshToken);
                if (!string.IsNullOrEmpty(userImageURL))
                {
                    await _localStorage.SetItemAsync(AppStorageConstants.Local.UserImageURL, userImageURL);
                }

                await ((AppStateProvider)this._appStateProvider).StateChangedAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return AppResponse.Valid(result.Message);
            }
            else
            {
                return AppResponse.Invalid(result.Message);
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(AppStorageConstants.Local.AuthToken);
            await _localStorage.RemoveItemAsync(AppStorageConstants.Local.RefreshToken);
            await _localStorage.RemoveItemAsync(AppStorageConstants.Local.UserImageURL);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await ((AppStateProvider)this._appStateProvider).StateChangedAsync();
        }

        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(AppStorageConstants.Local.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(AppStorageConstants.Local.RefreshToken);
            var refreshTokenRequest = new RefreshTokenRequest { Token = token, RefreshToken = refreshToken };

            var response = await _httpClient.PostAsJsonAsync("api/admin/account/Refresh", refreshTokenRequest);

            var result = await response.Content.ReadFromJsonAsync<AppResponse<TokenResponse>>();

            if (result !=null && !result.Success)
            {
                throw new ApplicationException("Something went wrong during the refresh token action");
            }

            token = result.Data.Token;
            refreshToken = result.Data.RefreshToken;
            await _localStorage.SetItemAsync(AppStorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(AppStorageConstants.Local.RefreshToken, refreshToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return token;
        }

        public async Task<string> TryRefreshToken()
        {
            //check if token exists
            var availableToken = await _localStorage.GetItemAsync<string>(AppStorageConstants.Local.RefreshToken);
            if (string.IsNullOrEmpty(availableToken)) return string.Empty;
            var authState = await _appStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 1)
                return await RefreshToken();
            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken()
        {
            return await RefreshToken();
        }
    }
}
