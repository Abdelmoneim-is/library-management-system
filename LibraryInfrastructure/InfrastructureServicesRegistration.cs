using LibraryApplication.Contracts.BooksContracts;
using LibraryDomain.Contracts;
using LibraryInfrastructure.Data;
using LibraryInfrastructure.DataSeeder;
using LibraryInfrastructure.Identity.Data;
using LibraryInfrastructure.Identity.Entities;
using LibraryInfrastructure.Identity.Services;
using LibraryInfrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibraryInfrastructure.Identity.Services.TokenService;

namespace LibraryInfrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureService (this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbContext<StoreIdentityDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            });
            services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("Catalog");
            services.AddKeyedScoped<IDataSeeder, IdentityDataSeeder>("Identity");

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
             .AddRoles<IdentityRole>() 
             .AddRoleManager<RoleManager<IdentityRole>>() 
             .AddEntityFrameworkStores<StoreIdentityDbContext>();

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();

            var jwtSetting = configuration.GetSection("JWT").Get<JWTSettings>()
               ?? throw new InvalidOperationException("JWT Setting Is Missing");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSetting.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSetting.Audience,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),
                    ClockSkew = TimeSpan.Zero,
                };

                opt.Events = new JwtBearerEvents()
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Status = 401,
                            Title = "Auth.Unauthorized",
                            Detail = "You must be logged in to access this resource."
                        });
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Status = 403,
                            Title = "Auth.Forbidden",
                            Detail = "You do not have permission to perform this action."
                        });
                    }


                };
            });

            return services;
        }
    }
}
