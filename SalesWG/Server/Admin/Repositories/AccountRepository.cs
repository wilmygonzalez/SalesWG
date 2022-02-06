using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SalesWG.Server.Admin.Repositories.Interfaces;
using SalesWG.Server.Configurations;
using SalesWG.Server.Data.Identity;
using SalesWG.Shared.Constants;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Models;
using SalesWG.Shared.Models.Identity;

namespace SalesWG.Server.Admin.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppSetting _appSetting;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountRepository(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<AppSetting> appSetting,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appSetting = appSetting.Value;
            _signInManager = signInManager;
        }

        #region Utilities
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateJwtAsync(AppUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appSetting.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddDays(_appSetting.TokenExpiresInDays),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSetting.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        #endregion

        public async Task<AppResponse<TokenResponse>> LoginAsync(TokenRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return AppResponse<TokenResponse>.Invalid("User Not Found.");
            }
            if (!user.IsActive)
            {
                return AppResponse<TokenResponse>.Invalid("User Not Active. Please contact the administrator.");
            }
            if (!user.EmailConfirmed)
            {
                return AppResponse<TokenResponse>.Invalid("E-Mail not confirmed.");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return AppResponse<TokenResponse>.Invalid("Invalid Credentials.");
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_appSetting.TokenExpiresInDays);
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtAsync(user);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            
            return AppResponse<TokenResponse>.Valid(response, $"Login successfully {DateTime.UtcNow}");
        }

        public async Task<AppResponse<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                return AppResponse<TokenResponse>.Invalid("Invalid Client Token.");
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return AppResponse<TokenResponse>.Invalid("User Not Found.");
            }

            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return AppResponse<TokenResponse>.Invalid("Invalid Client Token.");
            }
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return AppResponse<TokenResponse>.Valid(response, "Token generated.");
        }

        public async Task<AppResponse> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                return AppResponse.Invalid($"Username {request.UserName} is already taken.");
            }
            var user = new AppUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                IsActive = true,
                EmailConfirmed = true
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, AppRoleConstants.BasicRole);
                    return AppResponse.Valid("User created successfully.");
                }
                else
                {
                    return AppResponse.Invalid(string.Join("", result.Errors.Select(a => a.Description.ToString()).ToList()));
                }
            }
            else
            {
                return AppResponse.Invalid($"Email {request.Email} is already registered.");
            }
        }

        //public async Task<AppResponse> RegisterAsync(RegisterRequest request, string origin)
        //{
        //    var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        //    if (userWithSameUserName != null)
        //    {
        //        return new AppResponse { Message = $"Username {request.UserName} is already taken." };
        //    }
        //    var user = new AppUser
        //    {
        //        Email = request.Email,
        //        FirstName = request.FirstName,
        //        LastName = request.LastName,
        //        UserName = request.UserName,
        //        PhoneNumber = request.PhoneNumber,
        //        IsActive = request.ActivateUser,
        //        EmailConfirmed = request.AutoConfirmEmail
        //    };

        //    if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        //    {
        //        var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
        //        if (userWithSamePhoneNumber != null)
        //        {
        //            return new AppResponse { Message = $"Phone number {request.PhoneNumber} is already registered." };
        //        }
        //    }

        //    var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        //    if (userWithSameEmail == null)
        //    {
        //        var result = await _userManager.CreateAsync(user, request.Password);
        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
        //            if (!request.AutoConfirmEmail)
        //            {
        //                var verificationUri = await SendVerificationEmail(user, origin);
        //                var mailRequest = new MailRequest
        //                {
        //                    From = "mail@codewithmukesh.com",
        //                    To = user.Email,
        //                    Body = string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], verificationUri),
        //                    Subject = _localizer["Confirm Registration"]
        //                };
        //                BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        //                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"], user.UserName));
        //            }
        //            return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered."], user.UserName));
        //        }
        //        else
        //        {
        //            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        //        }
        //    }
        //    else
        //    {
        //        return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
        //    }
        //}
    }
}
