using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spyglass.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Spyglass
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<AuthenticationService>();

            builder.Services.AddScoped<ReactionService>();

            builder.Services.AddScoped<SettingsService>();

            WebAssemblyHost host = builder.Build();

            AuthenticationService authenticationService = host.Services.GetRequiredService<AuthenticationService>();
            authenticationService.Initialize();

            await builder.Build().RunAsync();
        }
    }
}
