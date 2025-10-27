using BLCTWeb.Client;
using BLCTWeb.Client.Pages;
using BLCTWeb.Components;
using Microsoft.AspNetCore.Components.Server;
using MudBlazor.Services;
using System.Globalization;

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
#if DEBUG
            builder.Services.Configure<CircuitOptions>(options =>
            {
                options.DetailedErrors = true;
            });
#endif

            var app = builder.Build();
            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

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
