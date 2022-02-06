using System.Reflection;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SalesWG.Client.Services.Authentication;
using SalesWG.Shared.Constants;

namespace SalesWG.Client.Extensions
{
    public static class AppClientExtensions
    {
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddAuthorizationCore(options =>
            {
                RegisterPermissionClaims(options);
            });

            builder.Services.AddOptions();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<AppStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider> (s => s.GetRequiredService<AppStateProvider>());
            builder.Services.AddTransient<AppAuthenticationHeaderHandler>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            return builder;
        }

        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim("Permission", propertyValue.ToString()));
                }
            }
        }
    }
}
