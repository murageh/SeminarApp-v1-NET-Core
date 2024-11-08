
using WeatherApp.DTOs;
using WeatherApp.Interfaces;
using WeatherApp.Middleware;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /*
             * CUSTOM OBJECTS
             * Register CustomerService with HttpClient and default credentials.
             */
            builder.Services.AddHttpClient<SeminarService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseDefaultCredentials = true,
                });
            builder.Services.AddScoped<ISeminar, SeminarService>();
            builder.Services.AddSingleton<Credentials>();

            var app = builder.Build();

            // Add custom request validation middleware for token authentication
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Customers"), appBuilder =>
            {
                appBuilder.UseMiddleware<RequestValidationMiddleware>();
            });


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
