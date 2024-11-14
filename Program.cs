using SeminarIntegration.Data;
using SeminarIntegration.Middleware;
using SeminarIntegration.Services;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;

namespace SeminarIntegration
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

            // Register the database settings
            builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
            builder.Services.AddSingleton<UserDbContext>();
            builder.Services.AddScoped<IUserService, UserService>();
            // Register the AutoMapper profile
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Global Error Handling
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            // Adding of login 
            builder.Services.AddLogging();

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

            {
                // Create a scope to resolve the DbContext
                using var scope = app.Services.CreateScope(); // Add this line
                var context = scope.ServiceProvider; // Add this line
            }

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
