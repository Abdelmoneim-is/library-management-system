
using LibraryApi.Extensions;
using LibraryApi.Handlers;
using LibraryApplication;
using LibraryApplication.Profiles;
using LibraryDomain.Contracts;
using LibraryInfrastructure;
using LibraryInfrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using static LibraryInfrastructure.Identity.Services.TokenService;

namespace LibraryApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<StoreDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("auth", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.OnRejected = async (context, ct) =>
                {
                    await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = 429,
                        Title = "Too.ManyRequests",
                        Detail = "Too many attempts. Please try again later."
                    }, ct);
                };
            });

            builder.Services.AddInfrastructureService(builder.Configuration);
            builder.Services.ApplicationService();

            builder.Services.Configure<URLSettings>(builder.Configuration.GetSection("URLSettings"));
            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));

            var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTSettings>(); // أو الاسم الصحيح للكلاس عندك

            if (jwtSettings is null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT:SecretKey is missing or shorter than 32 characters. Set it via user-secrets or environment variables.");
            }

            builder.Services.AddCors(options =>
            {
                var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

                options.AddPolicy("Frontend", policy =>
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseExceptionHandler();

            try
            {
                await app.SendAndMigrationDataSeedingAsync();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(ex, "An error occurred during migration or seeding.");
                throw;   
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("Frontend");

            app.UseRateLimiter();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Folder")),
                RequestPath = "/Folder"
            });

            app.UseAuthentication();           
            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
