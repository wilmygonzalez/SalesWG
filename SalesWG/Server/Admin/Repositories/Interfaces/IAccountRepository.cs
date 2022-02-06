using SalesWG.Shared.Helpers;
using SalesWG.Shared.Models;
using SalesWG.Shared.Models.Identity;

namespace SalesWG.Server.Admin.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<AppResponse<TokenResponse>> LoginAsync(TokenRequest model);
        Task<AppResponse<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
        Task<AppResponse> RegisterAsync(RegisterRequest request, string origin);
        //Task<AppResponse> RegisterAsync(RegisterRequest request, string origin);
    }
}
