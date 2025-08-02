using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoinDeskApi.Api.Controllers;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Tests.Controllers
{
    public class CurrenciesControllerTests
    {
        private readonly Mock<ICurrencyService> _mockCurrencyService;
        private readonly Mock<ILogger<CurrenciesController>> _mockLogger;
        private readonly Mock<ILocalizationService> _mockLocalizationService;
        private readonly CurrenciesController _controller;

        public CurrenciesControllerTests()
        {
            _mockCurrencyService = new Mock<ICurrencyService>();
            _mockLogger = new Mock<ILogger<CurrenciesController>>();
            _mockLocalizationService = new Mock<ILocalizationService>();

            _controller = new CurrenciesController(
                _mockCurrencyService.Object,
                _mockLogger.Object,
                _mockLocalizationService.Object);
        }

        [Fact]
        public async Task GetAllCurrencies_WhenServiceReturnsSuccess_ShouldReturnOkResult()
        {
            // Arrange
            var currencies = new List<CurrencyDto>
            {
                new CurrencyDto { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$" },
                new CurrencyDto { Id = "EUR", ChineseName = "歐元", EnglishName = "Euro", Symbol = "€" }
            };
            var successResponse = ApiResponse<IEnumerable<CurrencyDto>>.SuccessResult(currencies, "Success");

            _mockCurrencyService
                .Setup(x => x.GetAllCurrenciesAsync())
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.GetAllCurrencies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CurrencyDto>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, response.Data?.Count());
        }

        [Fact]
        public async Task GetAllCurrencies_WhenServiceReturnsError_ShouldReturnBadRequest()
        {
            // Arrange
            var errorResponse = ApiResponse<IEnumerable<CurrencyDto>>.ErrorResult("Service error");

            _mockCurrencyService
                .Setup(x => x.GetAllCurrenciesAsync())
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.GetAllCurrencies();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<CurrencyDto>>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Service error", response.Message);
        }

        [Fact]
        public async Task GetCurrency_WhenServiceReturnsSuccess_ShouldReturnOkResult()
        {
            // Arrange
            var currencyId = "USD";
            var currency = new CurrencyDto { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$" };
            var successResponse = ApiResponse<CurrencyDto>.SuccessResult(currency, "Success");

            _mockCurrencyService
                .Setup(x => x.GetCurrencyByIdAsync(currencyId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.GetCurrency(currencyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("USD", response.Data?.Id);
        }

        [Fact]
        public async Task GetCurrency_WhenServiceReturnsError_ShouldReturnNotFound()
        {
            // Arrange
            var currencyId = "INVALID";
            var errorResponse = ApiResponse<CurrencyDto>.ErrorResult("Currency not found");

            _mockCurrencyService
                .Setup(x => x.GetCurrencyByIdAsync(currencyId))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.GetCurrency(currencyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Currency not found", response.Message);
        }

        [Fact]
        public async Task CreateCurrency_WhenServiceReturnsSuccess_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createRequest = new CreateCurrencyDto { Id = "JPY", ChineseName = "日圓", EnglishName = "Japanese Yen", Symbol = "¥" };
            var createdCurrency = new CurrencyDto { Id = "JPY", ChineseName = "日圓", EnglishName = "Japanese Yen", Symbol = "¥" };
            var successResponse = ApiResponse<CurrencyDto>.SuccessResult(createdCurrency, "Created successfully");

            _mockCurrencyService
                .Setup(x => x.CreateCurrencyAsync(createRequest))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.CreateCurrency(createRequest);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(createdResult.Value);
            Assert.True(response.Success);
            Assert.Equal("JPY", response.Data?.Id);
        }

        [Fact]
        public async Task CreateCurrency_WhenServiceReturnsError_ShouldReturnBadRequest()
        {
            // Arrange
            var createRequest = new CreateCurrencyDto { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$" };
            var errorResponse = ApiResponse<CurrencyDto>.ErrorResult("Currency already exists");

            _mockCurrencyService
                .Setup(x => x.CreateCurrencyAsync(createRequest))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.CreateCurrency(createRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(badRequestResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Currency already exists", response.Message);
        }

        [Fact]
        public async Task UpdateCurrency_WhenServiceReturnsSuccess_ShouldReturnOkResult()
        {
            // Arrange
            var currencyId = "USD";
            var updateRequest = new UpdateCurrencyDto { ChineseName = "美金", EnglishName = "US Dollar", Symbol = "$" };
            var updatedCurrency = new CurrencyDto { Id = "USD", ChineseName = "美金", EnglishName = "US Dollar", Symbol = "$" };
            var successResponse = ApiResponse<CurrencyDto>.SuccessResult(updatedCurrency, "Updated successfully");

            _mockCurrencyService
                .Setup(x => x.UpdateCurrencyAsync(currencyId, updateRequest))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.UpdateCurrency(currencyId, updateRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("美金", response.Data?.ChineseName);
        }

        [Fact]
        public async Task UpdateCurrency_WhenServiceReturnsError_ShouldReturnNotFound()
        {
            // Arrange
            var currencyId = "INVALID";
            var updateRequest = new UpdateCurrencyDto { ChineseName = "無效", EnglishName = "Invalid", Symbol = "X" };
            var errorResponse = ApiResponse<CurrencyDto>.ErrorResult("Currency not found");

            _mockCurrencyService
                .Setup(x => x.UpdateCurrencyAsync(currencyId, updateRequest))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.UpdateCurrency(currencyId, updateRequest);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CurrencyDto>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Currency not found", response.Message);
        }

        [Fact]
        public async Task DeleteCurrency_WhenServiceReturnsSuccess_ShouldReturnOkResult()
        {
            // Arrange
            var currencyId = "USD";
            var successResponse = ApiResponse<bool>.SuccessResult(true, "Deleted successfully");

            _mockCurrencyService
                .Setup(x => x.DeleteCurrencyAsync(currencyId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _controller.DeleteCurrency(currencyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(response.Success);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task DeleteCurrency_WhenServiceReturnsError_ShouldReturnNotFound()
        {
            // Arrange
            var currencyId = "INVALID";
            var errorResponse = ApiResponse<bool>.ErrorResult("Currency not found");

            _mockCurrencyService
                .Setup(x => x.DeleteCurrencyAsync(currencyId))
                .ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.DeleteCurrency(currencyId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Currency not found", response.Message);
        }
    }
}
