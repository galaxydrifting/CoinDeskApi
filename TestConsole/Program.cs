using Microsoft.EntityFrameworkCore;
using CoinDeskApi.Infrastructure.Data;
using CoinDeskApi.Core.Entities;

Console.WriteLine("開始測試資料庫連線...");

var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=CoinDeskApiDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true";

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(connectionString)
    .ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
    .Options;

try
{
    using var context = new ApplicationDbContext(options);

    Console.WriteLine("正在建立資料庫...");
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("資料庫建立成功！");

    Console.WriteLine("檢查資料庫是否可以連線...");
    var canConnect = await context.Database.CanConnectAsync();
    Console.WriteLine($"資料庫連線狀態: {canConnect}");

    Console.WriteLine("檢查 Currency 資料表是否存在...");
    var currencyCount = await context.Currencies.CountAsync();
    Console.WriteLine($"Currency 資料表中的記錄數: {currencyCount}");

    if (currencyCount == 0)
    {
        Console.WriteLine("新增初始資料...");
        var currencies = new[]
        {
            new Currency { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$" },
            new Currency { Id = "EUR", ChineseName = "歐元", EnglishName = "Euro", Symbol = "€" },
            new Currency { Id = "GBP", ChineseName = "英鎊", EnglishName = "British Pound Sterling", Symbol = "£" }
        };

        context.Currencies.AddRange(currencies);
        await context.SaveChangesAsync();
        Console.WriteLine("初始資料新增完成！");
    }

    Console.WriteLine("列出所有幣別:");
    var allCurrencies = await context.Currencies.OrderBy(c => c.Id).ToListAsync();
    foreach (var currency in allCurrencies)
    {
        Console.WriteLine($"  {currency.Id}: {currency.ChineseName} ({currency.EnglishName}) {currency.Symbol}");
    }

    Console.WriteLine("\n測試完成！資料庫運作正常。");
}
catch (Exception ex)
{
    Console.WriteLine($"發生錯誤: {ex.Message}");
    Console.WriteLine($"錯誤詳情: {ex.InnerException?.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
