using Microsoft.Extensions.Logging.Console;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotApiGw
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                //.SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
                //.AddEnvironmentVariables(); 
                //.AddJsonFile($"ocelot.Local.json", true, true);

            //Configure Logging
            //builder.Host.ConfigureLogging((hostingContext, loggingBuilder) =>
            //{
            //    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
            //    loggingBuilder.AddConsole();
            //    loggingBuilder.AddDebug();
            //});
            builder.Logging.ClearProviders();
            builder.Logging
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug();

            builder.Services.AddOcelot(builder.Configuration);

            var app = builder.Build();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.UseOcelot().Wait();

            app.Run();
        }

    }
}