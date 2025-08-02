using System.Text.Json;
using CoinDeskApi.Core.DTOs;
using CoinDeskApi.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoinDeskApi.Infrastructure.Services
{
    public class CoinDeskService : ICoinDeskService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoinDeskService> _logger;
        private readonly ICurrencyRepository _currencyRepository;
        private const string COINDESK_API_URL = "https://api.coindesk.com/v1/bpi/currentprice.json";

        public CoinDeskService(HttpClient httpClient, ILogger<CoinDeskService> logger, ICurrencyRepository currencyRepository)
        {
            _httpClient = httpClient;
            _logger = logger;
            _currencyRepository = currencyRepository;
        }

        public async Task<CoinDeskApiResponse> GetCurrentPriceAsync()
        {
            try
            {
                _logger.LogInformation("Calling CoinDesk API: {Url}", COINDESK_API_URL);

                var response = await _httpClient.GetAsync(COINDESK_API_URL);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("CoinDesk API Response: {Response}", jsonContent);

                    var coinDeskResponse = JsonSerializer.Deserialize<CoinDeskApiResponse>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return coinDeskResponse ?? GetMockingData();
                }
                else
                {
                    _logger.LogWarning("CoinDesk API call failed with status: {StatusCode}", response.StatusCode);
                    return GetMockingData();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling CoinDesk API, using mocking data");
                return GetMockingData();
            }
        }

        public async Task<TransformedCoinDeskResponse> GetTransformedDataAsync()
        {
            var originalData = await GetCurrentPriceAsync();
            var currencies = await _currencyRepository.GetAllAsync();
            var currencyDict = currencies.ToDictionary(c => c.Id, c => c.ChineseName);

            var transformedResponse = new TransformedCoinDeskResponse
            {
                UpdateTime = FormatIsoDateTimeString(originalData.Time.UpdatedISO),
                Currencies = originalData.Bpi
                    .Select(bpi => new CurrencyInfo
                    {
                        Code = bpi.Key,
                        ChineseName = currencyDict.GetValueOrDefault(bpi.Key, bpi.Value.Description),
                        Rate = bpi.Value.Rate
                    })
                    .OrderBy(c => c.Code)
                    .ToList()
            };

            return transformedResponse;
        }

        private static CoinDeskApiResponse GetMockingData()
        {
            return new CoinDeskApiResponse
            {
                Time = new Time
                {
                    Updated = "Aug 3, 2022 20:25:00 UTC",
                    UpdatedISO = "2022-08-03T20:25:00+00:00",
                    Updateduk = "Aug 3, 2022 at 21:25 BST"
                },
                Disclaimer = "This data was produced from the CoinDesk Bitcoin Price Index (USD). Non-USD currency data converted using hourly conversion rate from openexchangerates.org",
                ChartName = "Bitcoin",
                Bpi = new Dictionary<string, BpiInfo>
                {
                    ["USD"] = new BpiInfo
                    {
                        Code = "USD",
                        Symbol = "$",
                        Rate = "23,342.0112",
                        Description = "US Dollar",
                        Rate_float = 23342.0112m
                    },
                    ["GBP"] = new BpiInfo
                    {
                        Code = "GBP",
                        Symbol = "£",
                        Rate = "19,504.3978",
                        Description = "British Pound Sterling",
                        Rate_float = 19504.3978m
                    },
                    ["EUR"] = new BpiInfo
                    {
                        Code = "EUR",
                        Symbol = "€",
                        Rate = "22,738.5269",
                        Description = "Euro",
                        Rate_float = 22738.5269m
                    }
                }
            };
        }

        private static string FormatIsoDateTimeString(string isoDateTime)
        {
            if (DateTimeOffset.TryParse(isoDateTime, out var dateTimeOffset))
            {
                // 只格式化，不轉時區
                return dateTimeOffset.UtcDateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
            return DateTimeOffset.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
