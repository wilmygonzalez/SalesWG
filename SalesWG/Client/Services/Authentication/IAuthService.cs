using System.Security.Claims;
using SalesWG.Shared.Models;
using SalesWG.Shared.Models.Identity;

namespace SalesWG.Client.Services.Authentication
{
    public interface IAuthService
    {
        Task<AppResponse> Login(TokenRequest model);

        Task Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
        Task<bool> IsUserAuthenticated();
    }
}
