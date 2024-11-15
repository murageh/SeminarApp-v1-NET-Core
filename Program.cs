using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SeminarIntegration.Data;
using SeminarIntegration.Middleware;
using SeminarIntegration.Services;
using SeminarIntegration.Interfaces;
using SeminarIntegration.Models;
using SeminarIntegration.Controllers;
using SeminarIntegration.Services.Auth;
using SeminarIntegration.Utils;

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
            builder.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "SeminarManagementAPI",
                        Version = "v1"
                    });

                    // Define security scheme
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        BearerFormat = "JWT"
                    });

                    // Apply security to endpoints
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });

            // Register the database settings
            builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
            builder.Services.AddSingleton<UserDbContext>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
            builder.Services.AddScoped<IAuthService, AuthService>();
            // builder.Services.AddSingleton<ControllerHelpers>();

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

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.RequireHttpsMetadata = false; // For development only
                    jwtBearerOptions.SaveToken = true;
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
                        ValidAudience = builder.Configuration["AuthSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                            builder.Configuration["AuthSettings:SecretKey"] ??
                            throw new InvalidOperationException("No secret configured."))),
                    };
                });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireRole("user"));
                options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
            });

            var app = builder.Build();

            {
                // Create a scope to resolve the DbContext
                using var scope = app.Services.CreateScope(); // Add this line
                var context = scope.ServiceProvider; // Add this line
            }

            // // Add custom request validation middleware for token authentication
            // app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/Customers"),
            //     appBuilder => { appBuilder.UseMiddleware<RequestValidationMiddleware>(); });


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // For jwt token authentication

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}