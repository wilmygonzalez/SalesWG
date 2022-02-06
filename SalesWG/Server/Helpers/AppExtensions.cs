using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalesWG.Server.Data.Identity;
using SalesWG.Shared.Admin.Models;
using SalesWG.Shared.Constants;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Models;
using SalesWG.Shared.Models.Identity;

namespace SalesWG.Server.Helpers
{
    public static class AppExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (source == null)
                return new PagedList<T>(new List<T>(), pageIndex, pageSize);

            pageSize = Math.Max(pageSize, 1);

            var count = await source.CountAsync();

            var data = new List<T>();
            data.AddRange(await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync());

            return new PagedList<T>(data, pageIndex, pageSize, count);
        }

        public static PagedResponse<T> ToPagedResponse<T>(this IPagedList<T> source)
        {
            return new PagedResponse<T>
            {
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                TotalPages = source.TotalPages,
                HasPreviousPage = source.HasPreviousPage,
                HasNextPage = source.HasNextPage,
                Data = source.ToList()
            };
        }

        public static void GetAllPermissions(this List<RoleClaimResponse> allPermissions)
        {
            var modules = typeof(AppPermissions).GetNestedTypes();

            foreach (var module in modules)
            {
                var moduleName = string.Empty;
                var moduleDescription = string.Empty;

                if (module.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
                    moduleName = displayNameAttribute.DisplayName;

                if (module.GetCustomAttributes(typeof(DescriptionAttribute), true)
                    .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                    moduleDescription = descriptionAttribute.Description;

                var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (var fi in fields)
                {
                    var propertyValue = fi.GetValue(null);

                    if (propertyValue is not null)
                        allPermissions.Add(new RoleClaimResponse { Value = propertyValue.ToString(), Type = "Permission", Group = moduleName, Description = moduleDescription });
                }
            }

        }

        public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<AppRole> roleManager, AppRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                return await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return IdentityResult.Failed();
        }

        public static async Task<IdentityResult> AddPermissionClaim(this UserManager<AppUser> userManager, AppUser user, string permission)
        {
            var allClaims = await userManager.GetClaimsAsync(user);
            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                return await userManager.AddClaimAsync(user, new Claim("Permission", permission));
            }

            return IdentityResult.Failed();
        }
    }
}
