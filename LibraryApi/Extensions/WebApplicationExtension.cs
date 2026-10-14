using LibraryDomain.Contracts;

namespace LibraryApi.Extensions
{
    public static class WebApplicationExtension
    {
        public static async Task<WebApplication> SendAndMigrationDataSeedingAsync (this WebApplication app)
        {
            using var scope = app.Services.CreateScope ();
            var dataSeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Catalog");
            var IdentityDataSeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("Identity");

            await IdentityDataSeeder.SeedDataAsync();
            await dataSeeder.SeedDataAsync();
            return app;
        }
    } 
}
