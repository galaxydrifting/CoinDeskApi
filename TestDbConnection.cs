using Microsoft.EntityFrameworkCore;
using CoinDeskApi.Infrastructure.Data;
using CoinDeskApi.Core.Entities;

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CoinDeskApiDb_Test;Trusted_Connection=true;MultipleActiveResultSets=true")
    .ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
    .Options;

using var context = new ApplicationDbContext(options);

try
{
    Console.WriteLine("Testing database creation...");
    await context.Database.EnsureCreatedAsync();

    Console.WriteLine("Testing seed data insertion...");
    if (!context.Currencies.Any())
    {
        var currencies = new[]
        {
            new Currency { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$" },
            new Currency { Id = "EUR", ChineseName = "歐元", EnglishName = "Euro", Symbol = "€" },
            new Currency { Id = "GBP", ChineseName = "英鎊", EnglishName = "British Pound Sterling", Symbol = "£" }
        };

        context.Currencies.AddRange(currencies);
        await context.SaveChangesAsync();
        Console.WriteLine("Seed data inserted successfully!");
    }
    else
    {
        Console.WriteLine("Currencies already exist in database.");
    }

    var currencyCount = await context.Currencies.CountAsync();
    Console.WriteLine($"Total currencies in database: {currencyCount}");

    Console.WriteLine("Database operations completed successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
finally
{
    await context.Database.EnsureDeletedAsync();
    Console.WriteLine("Test database cleaned up.");
}
