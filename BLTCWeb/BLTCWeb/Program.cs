using BLCTWeb.Client;
using BLCTWeb.Client.Pages;
using BLCTWeb.Components;
using MudBlazor.Services;

namespace BLCTWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMudServices();
            builder.Services.AddRazorComponents(options =>
                options.DetailedErrors = builder.Environment.IsDevelopment())
                .AddInteractiveWebAssemblyComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddScoped<ServerParser>();

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

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Counter).Assembly)
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
