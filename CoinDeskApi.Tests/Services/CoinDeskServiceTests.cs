using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;
using CoinDeskApi.Infrastructure.Services;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Core.DTOs;
using System.Net;
using System.Text.Json;

namespace CoinDeskApi.Tests.Services
{
    public class CoinDeskServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<CoinDeskService>> _mockLogger;
        private readonly Mock<ICurrencyRepository> _mockCurrencyRepository;
        private readonly HttpClient _httpClient;
        private readonly CoinDeskService _service;

        public CoinDeskServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<CoinDeskService>>();
            _mockCurrencyRepository = new Mock<ICurrencyRepository>();

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _service = new CoinDeskService(_httpClient, _mockLogger.Object, _mockCurrencyRepository.Object);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_WhenApiCallSucceeds_ShouldReturnCoinDeskResponse()
        {
            // Arrange
            var mockApiResponse = new CoinDeskApiResponse
            {
                Time = new Time
                {
                    Updated = "Mar 22, 2018 23:30:00 UTC",
                    UpdatedISO = "2018-03-22T23:30:00+00:00",
                    Updateduk = "Mar 22, 2018 at 23:30 GMT"
                },
                Disclaimer = "Test disclaimer",
                ChartName = "Bitcoin",
                Bpi = new Dictionary<string, BpiInfo>
                {
                    {"USD", new BpiInfo { Code = "USD", Symbol = "&#36;", Rate = "10,000.00", Description = "United States Dollar", Rate_float = 10000.00m }}
                }
            };

            var jsonResponse = JsonSerializer.Serialize(mockApiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetCurrentPriceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bitcoin", result.ChartName);
            Assert.Equal("Test disclaimer", result.Disclaimer);
            Assert.Single(result.Bpi);
            Assert.True(result.Bpi.ContainsKey("USD"));
        }

        [Fact]
        public async Task GetCurrentPriceAsync_WhenApiCallFails_ShouldReturnMockingData()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetCurrentPriceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bitcoin", result.ChartName);
            Assert.Contains("USD", result.Bpi.Keys);
            Assert.Contains("GBP", result.Bpi.Keys);
            Assert.Contains("EUR", result.Bpi.Keys);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_WhenExceptionThrown_ShouldReturnMockingData()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _service.GetCurrentPriceAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bitcoin", result.ChartName);
            Assert.Contains("USD", result.Bpi.Keys);
        }

        [Fact]
        public async Task GetTransformedDataAsync_ShouldReturnTransformedResponse()
        {
            // Arrange
            var mockCurrencies = new List<CoinDeskApi.Core.Entities.Currency>
            {
                new CoinDeskApi.Core.Entities.Currency { Id = "USD", ChineseName = "美元" },
                new CoinDeskApi.Core.Entities.Currency { Id = "GBP", ChineseName = "英鎊" },
                new CoinDeskApi.Core.Entities.Currency { Id = "EUR", ChineseName = "歐元" }
            };

            _mockCurrencyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(mockCurrencies);

            var mockApiResponse = new CoinDeskApiResponse
            {
                Time = new Time
                {
                    Updated = "Mar 22, 2018 23:30:00 UTC",
                    UpdatedISO = "2018-03-22T23:30:00+00:00", // 正確的 ISO 格式
                    Updateduk = "Mar 22, 2018 at 23:30 GMT"
                },
                Bpi = new Dictionary<string, BpiInfo>
                {
                    {"USD", new BpiInfo { Code = "USD", Rate = "10,000.00" }},
                    {"GBP", new BpiInfo { Code = "GBP", Rate = "8,000.00" }},
                    {"EUR", new BpiInfo { Code = "EUR", Rate = "9,000.00" }}
                }
            };

            var jsonResponse = JsonSerializer.Serialize(mockApiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetTransformedDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2018/03/22 23:30:00", result.UpdateTime);
            Assert.Equal(3, result.Currencies.Count);

            var usdCurrency = result.Currencies.First(c => c.Code == "USD");
            Assert.Equal("美元", usdCurrency.ChineseName);
            Assert.Equal("10,000.00", usdCurrency.Rate);
        }

        [Fact]
        public async Task GetTransformedDataAsync_WhenApiCallFails_ShouldStillReturnTransformedMockingData()
        {
            // Arrange
            var mockCurrencies = new List<CoinDeskApi.Core.Entities.Currency>
            {
                new CoinDeskApi.Core.Entities.Currency { Id = "USD", ChineseName = "美元" },
                new CoinDeskApi.Core.Entities.Currency { Id = "GBP", ChineseName = "英鎊" },
                new CoinDeskApi.Core.Entities.Currency { Id = "EUR", ChineseName = "歐元" }
            };

            _mockCurrencyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(mockCurrencies);

            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _service.GetTransformedDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.UpdateTime);
            Assert.Equal(3, result.Currencies.Count);

            var usdCurrency = result.Currencies.First(c => c.Code == "USD");
            Assert.Equal("美元", usdCurrency.ChineseName);
        }

        [Fact]
        public async Task GetTransformedDataAsync_VerifyRepositoryMethodCalled()
        {
            // Arrange
            var mockCurrencies = new List<CoinDeskApi.Core.Entities.Currency>();
            _mockCurrencyRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(mockCurrencies);

            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            await _service.GetTransformedDataAsync();

            // Assert
            _mockCurrencyRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCurrentPriceAsync_VerifyHttpClientCalled()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"chartName\":\"Bitcoin\",\"bpi\":{}}")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            await _service.GetCurrentPriceAsync();

            // Assert
            _mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains("coindesk.com")),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}
