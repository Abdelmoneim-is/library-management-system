using LibraryDomain.Common;
using LibraryDomain.Contracts;
using LibraryDomain.Entities;
using LibraryInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LibraryInfrastructure.DataSeeder
{
    internal class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                var rootPath = Path.Combine(AppContext.BaseDirectory, "DataSeeder");
                logger.LogInformation("Seeder root path: {RootPath}", rootPath);

                var totalRows = 0;

                var seededAuthors = await SeedIfEmptyAsync<BookAuthor, int>(rootPath, "Authors.json", ct);
                var seededCategories = await SeedIfEmptyAsync<BookCategory, int>(rootPath, "Categories.json", ct);
                totalRows += await dbContext.SaveChangesAsync(ct);

                if (await dbContext.Set<Books>().AnyAsync(ct))
                {
                    logger.LogInformation("Books table already has data, skipping.");
                }
                else
                {
                    var books = await ReadJsonAsync<Books>(rootPath, "Books.json", ct);

                    if (books is { Count: > 0 })
                    {
                        var authors = seededAuthors
                            ?? await dbContext.Set<BookAuthor>().OrderBy(a => a.Id).ToListAsync(ct);
                        var categories = seededCategories
                            ?? await dbContext.Set<BookCategory>().OrderBy(c => c.Id).ToListAsync(ct);

                        if (seededAuthors is null || seededCategories is null)
                            logger.LogWarning(
                                "Authors/Categories already existed; Books.json ids are mapped by position among existing rows.");

                        foreach (var book in books)
                        {
                            // AuthorId و CategoryId في Books.json هي ترتيب العنصر (1 = أول مؤلف في Authors.json)
                            var authorPosition = book.AuthorId;
                            var categoryPosition = book.CategoryId;

                            if (authorPosition < 1 || authorPosition > authors.Count)
                                throw new InvalidOperationException(
                                    $"Invalid AuthorId {authorPosition} in Books.json for '{book.Title}'.");

                            if (categoryPosition < 1 || categoryPosition > categories.Count)
                                throw new InvalidOperationException(
                                    $"Invalid CategoryId {categoryPosition} in Books.json for '{book.Title}'.");

                            book.AuthorId = authors[authorPosition - 1].Id;
                            book.CategoryId = categories[categoryPosition - 1].Id;
                        }

                        await dbContext.Set<Books>().AddRangeAsync(books, ct);
                        totalRows += await dbContext.SaveChangesAsync(ct);
                    }
                }

                if (totalRows > 0)
                    logger.LogInformation("{Rows} rows added.", totalRows);
                else
                    logger.LogInformation("Database already seeded.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Catalog data seeding failed.");
                throw;
            }
        }

        private async Task<List<T>?> SeedIfEmptyAsync<T, TKey>(string rootPath, string fileName, CancellationToken ct)
            where T : BaseEntity<TKey>
        {
            if (await dbContext.Set<T>().AnyAsync(ct))
            {
                logger.LogInformation("{Table} already has data, skipping.", typeof(T).Name);
                return null;
            }

            var items = await ReadJsonAsync<T>(rootPath, fileName, ct);
            if (items is null || items.Count == 0)
                return null;

            await dbContext.Set<T>().AddRangeAsync(items, ct);
            return items;
        }

        private async Task<List<T>?> ReadJsonAsync<T>(string rootPath, string fileName, CancellationToken ct)
        {
            var filePath = Path.Combine(rootPath, fileName);

            if (!File.Exists(filePath))
            {
                logger.LogWarning("Seed file {FileName} was not found at {FilePath}.", fileName, filePath);
                return null;
            }

            await using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions, ct);
        }
    }
}