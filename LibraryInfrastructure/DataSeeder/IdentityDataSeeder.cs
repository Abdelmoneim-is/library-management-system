using LibraryDomain.Contracts;
using LibraryInfrastructure.Identity.Data;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LibraryInfrastructure.DataSeeder
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private static readonly string[] Roles = ["Admin", "Member"];

        private readonly StoreIdentityDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityDataSeeder> _logger;

        public IdentityDataSeeder(
            StoreIdentityDbContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<IdentityDataSeeder> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await _dbContext.Database.MigrateAsync(ct);

                await SeedRolesAsync();
                await SeedDefaultAdminAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity data seeding failed.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            foreach (var role in Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                    continue;

                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"Could not create role '{role}': {string.Join(" | ", result.Errors.Select(e => e.Description))}");

                _logger.LogInformation("Role seeded: {Role}", role);
            }
        }

        private async Task SeedDefaultAdminAsync()
        {
            var email = _configuration["SeedAdmin:Email"];
            var password = _configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "SeedAdmin:Email / SeedAdmin:Password are not configured. Default admin was NOT created.");
                return;
            }

            if (await _userManager.FindByEmailAsync(email) is not null)
            {
                _logger.LogInformation("Default admin already exists.");
                return;
            }

            var admin = new AppUser
            {
                DisplayName = _configuration["SeedAdmin:DisplayName"] ?? "Library Admin",
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(admin, password);
            if (!createResult.Succeeded)
                throw new InvalidOperationException(
                    $"Could not seed default admin: {string.Join(" | ", createResult.Errors.Select(e => e.Description))}");

            var roleResult = await _userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    $"Could not assign Admin role: {string.Join(" | ", roleResult.Errors.Select(e => e.Description))}");

            _logger.LogInformation("Default admin created.");
        }
    }
}