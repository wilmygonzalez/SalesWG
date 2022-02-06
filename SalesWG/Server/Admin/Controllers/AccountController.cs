using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesWG.Server.Admin.Repositories.Interfaces;
using SalesWG.Shared.Models.Identity;

namespace SalesWG.Server.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(TokenRequest request)
        {
            var response = await _accountRepository.LoginAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<ActionResult> Refresh(RefreshTokenRequest request)
        {
            var response = await _accountRepository.GetRefreshTokenAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _accountRepository.RegisterAsync(request, origin));
        }
    }
}
