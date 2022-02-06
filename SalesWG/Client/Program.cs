using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SalesWG.Client;
using MudBlazor.Services;
using SalesWG.Client.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using SalesWG.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.AddClientServices();

await builder.Build().RunAsync();
