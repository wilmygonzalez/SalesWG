using Microsoft.AspNetCore.Identity;
using SalesWG.Server.Data.Identity;
using SalesWG.Server.Helpers;
using SalesWG.Shared.Constants;

namespace SalesWG.Server.Data
{
    public class AppDbSeeder
    {
        private readonly ILogger<AppDbSeeder> _logger;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AppDbSeeder(
            ILogger<AppDbSeeder> logger,
            AppDbContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var adminRole = new AppRole(AppRoleConstants.AdministratorRole, "Administrator role with full permissions");
                var adminRoleInDb = await _roleManager.FindByNameAsync(AppRoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(AppRoleConstants.AdministratorRole);
                    _logger.LogInformation("Seeded Administrator Role.");
                }

                var basicUser = await _userManager.FindByEmailAsync("wilmygnz1@gmail.com");
                if (basicUser == null)
                {
                    basicUser = new AppUser
                    {
                        FirstName = "Wilmy",
                        LastName = "Gonzalez",
                        Email = "wilmygnz1@gmail.com",
                        UserName = "wilmygnz",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _userManager.CreateAsync(basicUser, "SalesWG2021.");
                    var result1 = await _userManager.AddToRoleAsync(basicUser, AppRoleConstants.AdministratorRole);

                }

                var superUser = await _userManager.FindByEmailAsync("wilmygnz@gmail.com");
                if (superUser == null)
                {
                    superUser = new AppUser
                    {
                        FirstName = "Wilmy",
                        LastName = "Gonzalez",
                        Email = "wilmygnz@gmail.com",
                        UserName = "wilmygnz",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _userManager.CreateAsync(superUser, "SalesWG2021.");
                    var result = await _userManager.AddToRoleAsync(superUser, AppRoleConstants.AdministratorRole);

                    

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Seeded Default SuperAdmin User.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }

                foreach (var permission in AppPermissions.GetRegisteredPermissions())
                {
                    await _userManager.AddPermissionClaim(superUser, permission);
                    await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                }
            }).GetAwaiter().GetResult();
        }
    }
}
