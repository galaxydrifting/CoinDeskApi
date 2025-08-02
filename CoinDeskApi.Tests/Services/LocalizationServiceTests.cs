using Moq;
using Microsoft.Extensions.Localization;
using CoinDeskApi.Infrastructure.Services;

namespace CoinDeskApi.Tests.Services
{
    public class LocalizationServiceTests
    {
        private readonly Mock<IStringLocalizerFactory> _mockLocalizerFactory;
        private readonly Mock<IStringLocalizer> _mockLocalizer;
        private readonly LocalizationService _service;

        public LocalizationServiceTests()
        {
            _mockLocalizerFactory = new Mock<IStringLocalizerFactory>();
            _mockLocalizer = new Mock<IStringLocalizer>();

            _mockLocalizerFactory
                .Setup(x => x.Create("Messages", "CoinDeskApi.Api"))
                .Returns(_mockLocalizer.Object);

            _service = new LocalizationService(_mockLocalizerFactory.Object);
        }

        [Fact]
        public void GetString_WithKey_ShouldReturnLocalizedString()
        {
            // Arrange
            var key = "CurrencyNotFound";
            var expectedValue = "Currency not found";

            var localizedString = new LocalizedString(key, expectedValue);
            _mockLocalizer
                .Setup(x => x[key])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetString_WithKeyAndArguments_ShouldReturnFormattedString()
        {
            // Arrange
            var key = "CurrencyCreatedWithId";
            var arg1 = "USD";
            var expectedValue = "Currency USD has been created";

            var localizedString = new LocalizedString(key, expectedValue);
            _mockLocalizer
                .Setup(x => x[key, arg1])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key, arg1);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetString_WithMultipleArguments_ShouldReturnFormattedString()
        {
            // Arrange
            var key = "UpdatedCurrencyFromTo";
            var arg1 = "USD";
            var arg2 = "美元";
            var arg3 = "美金";
            var expectedValue = "Updated USD from 美元 to 美金";

            var localizedString = new LocalizedString(key, expectedValue);
            _mockLocalizer
                .Setup(x => x[key, arg1, arg2, arg3])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key, arg1, arg2, arg3);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetString_WhenKeyNotFound_ShouldReturnKey()
        {
            // Arrange
            var key = "NonExistentKey";
            var value = "SomeValue";
            var localizedString = new LocalizedString(key, value, true); // resourceNotFound = true
            _mockLocalizer
                .Setup(x => x[key])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key);

            // Assert
            // 應該回傳 key 而不是 value
            Assert.Equal(key, result);
        }

        [Fact]
        public void GetString_VerifyLocalizerAccessedWithCorrectKey()
        {
            // Arrange
            var key = "TestKey";
            var localizedString = new LocalizedString(key, "Test Value");

            _mockLocalizer
                .Setup(x => x[key])
                .Returns(localizedString);

            // Act
            _service.GetString(key);

            // Assert
            _mockLocalizer.Verify(x => x[key], Times.Once);
        }

        [Fact]
        public void GetString_WithArguments_VerifyLocalizerAccessedWithCorrectKeyAndArguments()
        {
            // Arrange
            var key = "TestKey";
            var arg1 = "arg1";
            var arg2 = "arg2";
            var localizedString = new LocalizedString(key, "Test Value");

            _mockLocalizer
                .Setup(x => x[key, arg1, arg2])
                .Returns(localizedString);

            // Act
            _service.GetString(key, arg1, arg2);

            // Assert
            _mockLocalizer.Verify(x => x[key, arg1, arg2], Times.Once);
        }

        [Theory]
        [InlineData("CurrenciesRetrievedSuccessfully", "成功取得幣別列表")]
        [InlineData("CurrencyNotFound", "找不到指定的幣別")]
        [InlineData("CurrencyAlreadyExists", "幣別已存在")]
        public void GetString_WithCommonKeys_ShouldReturnExpectedValues(string key, string expectedValue)
        {
            // Arrange
            var localizedString = new LocalizedString(key, expectedValue);
            _mockLocalizer
                .Setup(x => x[key])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key);

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetString_WithEmptyKey_ShouldHandleGracefully()
        {
            // Arrange
            var key = "";
            var localizedString = new LocalizedString(key, key);
            _mockLocalizer
                .Setup(x => x[key])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key);

            // Assert
            Assert.Equal(key, result);
            _mockLocalizer.Verify(x => x[key], Times.Once);
        }

        [Fact]
        public void GetString_WithNullArguments_ShouldHandleGracefully()
        {
            // Arrange
            var key = "TestKey";
            object[] nullArgs = null!;
            var localizedString = new LocalizedString(key, "Test Value");

            _mockLocalizer
                .Setup(x => x[key, nullArgs])
                .Returns(localizedString);

            // Act
            var result = _service.GetString(key, nullArgs);

            // Assert
            Assert.Equal("Test Value", result);
        }
    }
}
