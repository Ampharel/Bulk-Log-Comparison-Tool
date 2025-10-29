using BLCTWeb.Client;
using BLCTWeb.Client.Pages;
using BLCTWeb.Components;
using Microsoft.AspNetCore.Components.Server;
using MudBlazor.Services;
using System.Globalization;
using Microsoft.AspNetCore.Components.Web; // AntiforgeryMessageHandler
using Microsoft.AspNetCore.Http;          // IHttpContextAccessor
using Microsoft.AspNetCore.HttpOverrides; // ForwardedHeaders

namespace BLCTWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var sw = new StreamWriter($"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\OutputLog.txt", true);
            sw.AutoFlush = true;
            Console.SetOut(sw);
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMudServices();
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents(options =>
                options.DetailedErrors = builder.Environment.IsDevelopment())
                .AddInteractiveWebAssemblyComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddScoped<ServerParser>();
            builder.Services.AddScoped<SpecFilterService>();

            // Register HttpClient for server-rendered components
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AntiforgeryMessageHandler>();

            // Do NOT set BaseAddress here (can be invalid behind proxies). Just attach antiforgery.
            builder.Services.AddHttpClient("Self")
                .AddHttpMessageHandler<AntiforgeryMessageHandler>();

            // Allow injecting HttpClient directly
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Self"));

            // Show detailed circuit errors to diagnose hydration failures
            builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(o => o.DetailedErrors = true);

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) app.UseWebAssemblyDebugging();
            else { app.UseExceptionHandler("/Error"); app.UseHsts(); }

            app.UseHttpsRedirection();

            // Forwarded headers (helps Scheme/Host when behind Cloudflare/NGINX)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            });

            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Counter).Assembly)
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
