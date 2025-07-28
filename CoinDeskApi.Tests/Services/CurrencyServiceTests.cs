using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Infrastructure.Services;
using CoinDeskApi.Core.Entities;
using CoinDeskApi.Core.DTOs;
using AutoMapper;

namespace CoinDeskApi.Tests.Services
{
    public class CurrencyServiceTests
    {
        private readonly Mock<ICurrencyRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CurrencyService>> _mockLogger;
        private readonly CurrencyService _service;

        public CurrencyServiceTests()
        {
            _mockRepository = new Mock<ICurrencyRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CurrencyService>>();
            _service = new CurrencyService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllCurrenciesAsync_ShouldReturnSuccessResponse()
        {
            // Arrange
            var currencies = new List<Currency>
            {
                new Currency { Id = "USD", ChineseName = "美元" },
                new Currency { Id = "EUR", ChineseName = "歐元" }
            };
            var currencyDtos = new List<CurrencyDto>
            {
                new CurrencyDto { Id = "USD", ChineseName = "美元" },
                new CurrencyDto { Id = "EUR", ChineseName = "歐元" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(currencies);
            _mockMapper.Setup(m => m.Map<IEnumerable<CurrencyDto>>(currencies)).Returns(currencyDtos);

            // Act
            var result = await _service.GetAllCurrenciesAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetCurrencyByIdAsync_WithValidId_ShouldReturnCurrency()
        {
            // Arrange
            var currency = new Currency { Id = "USD", ChineseName = "美元" };
            var currencyDto = new CurrencyDto { Id = "USD", ChineseName = "美元" };

            _mockRepository.Setup(r => r.GetByIdAsync("USD")).ReturnsAsync(currency);
            _mockMapper.Setup(m => m.Map<CurrencyDto>(currency)).Returns(currencyDto);

            // Act
            var result = await _service.GetCurrencyByIdAsync("USD");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("USD", result.Data.Id);
        }

        [Fact]
        public async Task GetCurrencyByIdAsync_WithInvalidId_ShouldReturnErrorResponse()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync("INVALID")).ReturnsAsync((Currency)null);

            // Act
            var result = await _service.GetCurrencyByIdAsync("INVALID");

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not found", result.Message);
        }

        [Fact]
        public async Task CreateCurrencyAsync_WithValidData_ShouldReturnSuccessResponse()
        {
            // Arrange
            var createDto = new CreateCurrencyDto { Id = "JPY", ChineseName = "日圓" };
            var currency = new Currency { Id = "JPY", ChineseName = "日圓" };
            var currencyDto = new CurrencyDto { Id = "JPY", ChineseName = "日圓" };

            _mockRepository.Setup(r => r.ExistsAsync("JPY")).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Currency>(createDto)).Returns(currency);
            _mockRepository.Setup(r => r.CreateAsync(currency)).ReturnsAsync(currency);
            _mockMapper.Setup(m => m.Map<CurrencyDto>(currency)).Returns(currencyDto);

            // Act
            var result = await _service.CreateCurrencyAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("JPY", result.Data.Id);
        }

        [Fact]
        public async Task CreateCurrencyAsync_WithExistingId_ShouldReturnErrorResponse()
        {
            // Arrange
            var createDto = new CreateCurrencyDto { Id = "USD", ChineseName = "美元" };
            _mockRepository.Setup(r => r.ExistsAsync("USD")).ReturnsAsync(true);

            // Act
            var result = await _service.CreateCurrencyAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("already exists", result.Message);
        }
    }
}
