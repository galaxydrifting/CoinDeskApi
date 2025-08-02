using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using CoinDeskApi.Infrastructure.Data;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Infrastructure.Repositories;
using CoinDeskApi.Infrastructure.Services;
using CoinDeskApi.Infrastructure.Mapping;
using CoinDeskApi.Api.Middleware;
using CoinDeskApi.Core.Entities;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/coindesk-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// 添加多語系支援
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("zh-TW")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // 設定語言提供者的優先順序
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(), // ?culture=zh-TW
        new CookieRequestCultureProvider(),      // Cookie
        new AcceptLanguageHeaderRequestCultureProvider() // Accept-Language header
    };
});

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 共用 HttpClient
builder.Services.AddHttpClient();

// Repository and Service dependencies
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICoinDeskService, CoinDeskService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoinDesk API",
        Version = "v1",
        Description = "A Web API for CoinDesk Bitcoin Price Index with Currency Management",
        Contact = new OpenApiContact
        {
            Name = "Developer",
            Email = "developer@example.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS, demo 用先全開，正常僅允許前端或者 reverse proxy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// 使用多語系中介軟體
app.UseRequestLocalization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoinDesk API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at apps root
    });
}

// Only use HTTPS redirection in production or when HTTPS is available
if (!app.Environment.IsDevelopment() ||
    app.Configuration.GetValue<string>("ASPNETCORE_URLS")?.Contains("https") == true)
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Auto migrate database on startup and seed initial data
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    Log.Information("Ensuring database is created...");
    context.Database.EnsureCreated();

    Log.Information("Seeding initial data if needed...");
    if (!context.Currencies.Any())
    {
        var currencies = new[]
        {
            new Currency { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$" },
            new Currency { Id = "EUR", ChineseName = "歐元", EnglishName = "Euro", Symbol = "€" },
            new Currency { Id = "GBP", ChineseName = "英鎊", EnglishName = "British Pound Sterling", Symbol = "£" }
        };

        context.Currencies.AddRange(currencies);
        context.SaveChanges();
        Log.Information("Initial currency data seeded successfully");
    }
    else
    {
        Log.Information("Currency data already exists, skipping seed");
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Error during database initialization");
}

try
{
    Log.Information("Starting CoinDesk API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
