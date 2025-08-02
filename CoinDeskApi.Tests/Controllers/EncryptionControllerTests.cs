using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoinDeskApi.Api.Controllers;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Tests.Controllers
{
    public class EncryptionControllerTests
    {
        private readonly Mock<IEncryptionService> _mockEncryptionService;
        private readonly Mock<ILogger<EncryptionController>> _mockLogger;
        private readonly EncryptionController _controller;

        public EncryptionControllerTests()
        {
            _mockEncryptionService = new Mock<IEncryptionService>();
            _mockLogger = new Mock<ILogger<EncryptionController>>();
            _controller = new EncryptionController(_mockEncryptionService.Object, _mockLogger.Object);
        }

        [Fact]
        public void EncryptAES_ShouldReturnOkResultWithEncryptedText()
        {
            // Arrange
            var request = new CoinDeskApi.Api.Controllers.EncryptRequest { PlainText = "Hello World", Key = "TestKey123456789" };
            var encryptedText = "EncryptedTextExample";

            _mockEncryptionService
                .Setup(x => x.EncryptAES(request.PlainText, request.Key))
                .Returns(encryptedText);

            // Act
            var result = _controller.EncryptAES(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Text encrypted successfully", response.Message);
            Assert.Equal(encryptedText, response.Data);
        }

        [Fact]
        public void DecryptAES_ShouldReturnOkResultWithDecryptedText()
        {
            // Arrange
            var request = new CoinDeskApi.Api.Controllers.DecryptRequest { CipherText = "EncryptedTextExample", Key = "TestKey123456789" };
            var decryptedText = "Hello World";

            _mockEncryptionService
                .Setup(x => x.DecryptAES(request.CipherText, request.Key))
                .Returns(decryptedText);

            // Act
            var result = _controller.DecryptAES(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Text decrypted successfully", response.Message);
            Assert.Equal(decryptedText, response.Data);
        }

        [Fact]
        public void GenerateRSAKeys_ShouldReturnOkResultWithKeyPair()
        {
            // Arrange
            var publicKey = "PublicKeyExample";
            var privateKey = "PrivateKeyExample";

            _mockEncryptionService
                .Setup(x => x.GenerateRSAKeyPair())
                .Returns((publicKey, privateKey));

            // Act
            var result = _controller.GenerateRSAKeys();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<CoinDeskApi.Api.Controllers.RSAKeyPair>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("RSA key pair generated successfully", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(publicKey, response.Data.PublicKey);
            Assert.Equal(privateKey, response.Data.PrivateKey);
        }

        [Fact]
        public void EncryptRSA_ShouldReturnOkResultWithEncryptedText()
        {
            // Arrange
            var request = new CoinDeskApi.Api.Controllers.RSAEncryptRequest { PlainText = "Hello RSA", PublicKey = "PublicKeyExample" };
            var encryptedText = "RSAEncryptedTextExample";

            _mockEncryptionService
                .Setup(x => x.EncryptRSA(request.PlainText, request.PublicKey))
                .Returns(encryptedText);

            // Act
            var result = _controller.EncryptRSA(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Text encrypted successfully", response.Message);
            Assert.Equal(encryptedText, response.Data);
        }

        [Fact]
        public void DecryptRSA_ShouldReturnOkResultWithDecryptedText()
        {
            // Arrange
            var request = new CoinDeskApi.Api.Controllers.RSADecryptRequest { CipherText = "RSAEncryptedTextExample", PrivateKey = "PrivateKeyExample" };
            var decryptedText = "Hello RSA";

            _mockEncryptionService
                .Setup(x => x.DecryptRSA(request.CipherText, request.PrivateKey))
                .Returns(decryptedText);

            // Act
            var result = _controller.DecryptRSA(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Text decrypted successfully", response.Message);
            Assert.Equal(decryptedText, response.Data);
        }

        [Fact]
        public void EncryptAES_VerifyServiceMethodCalled()
        {
            // Arrange
            var request = new CoinDeskApi.Api.Controllers.EncryptRequest { PlainText = "Test", Key = "TestKey" };

            _mockEncryptionService
                .Setup(x => x.EncryptAES(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("encrypted");

            // Act
            _controller.EncryptAES(request);

            // Assert
            _mockEncryptionService.Verify(x => x.EncryptAES(request.PlainText, request.Key), Times.Once);
        }

        [Fact]
        public void DecryptAES_VerifyServiceMethodCalled()
        {
            // Arrange
            var request = new CoinDeskApi.Api.Controllers.DecryptRequest { CipherText = "encrypted", Key = "TestKey" };

            _mockEncryptionService
                .Setup(x => x.DecryptAES(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("decrypted");

            // Act
            _controller.DecryptAES(request);

            // Assert
            _mockEncryptionService.Verify(x => x.DecryptAES(request.CipherText, request.Key), Times.Once);
        }

        [Fact]
        public void GenerateRSAKeys_VerifyServiceMethodCalled()
        {
            // Arrange
            _mockEncryptionService
                .Setup(x => x.GenerateRSAKeyPair())
                .Returns(("public", "private"));

            // Act
            _controller.GenerateRSAKeys();

            // Assert
            _mockEncryptionService.Verify(x => x.GenerateRSAKeyPair(), Times.Once);
        }
    }
}
