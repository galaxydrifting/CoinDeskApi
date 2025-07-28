using Xunit;
using CoinDeskApi.Infrastructure.Services;

namespace CoinDeskApi.Tests.Services
{
    public class EncryptionServiceTests
    {
        private readonly EncryptionService _encryptionService;

        public EncryptionServiceTests()
        {
            _encryptionService = new EncryptionService();
        }

        [Fact]
        public void AES_EncryptDecrypt_ShouldReturnOriginalText()
        {
            // Arrange
            var plainText = "Hello World!";
            var key = "TestKey123456789";

            // Act
            var encrypted = _encryptionService.EncryptAES(plainText, key);
            var decrypted = _encryptionService.DecryptAES(encrypted, key);

            // Assert
            Assert.Equal(plainText, decrypted);
            Assert.NotEqual(plainText, encrypted);
        }

        [Fact]
        public void RSA_GenerateKeyPair_ShouldReturnValidKeys()
        {
            // Act
            var (publicKey, privateKey) = _encryptionService.GenerateRSAKeyPair();

            // Assert
            Assert.NotNull(publicKey);
            Assert.NotNull(privateKey);
            Assert.NotEmpty(publicKey);
            Assert.NotEmpty(privateKey);
            Assert.NotEqual(publicKey, privateKey);
        }

        [Fact]
        public void RSA_EncryptDecrypt_ShouldReturnOriginalText()
        {
            // Arrange
            var plainText = "Hello RSA!";
            var (publicKey, privateKey) = _encryptionService.GenerateRSAKeyPair();

            // Act
            var encrypted = _encryptionService.EncryptRSA(plainText, publicKey);
            var decrypted = _encryptionService.DecryptRSA(encrypted, privateKey);

            // Assert
            Assert.Equal(plainText, decrypted);
            Assert.NotEqual(plainText, encrypted);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Short")]
        [InlineData("This is a longer text for testing purposes")]
        public void AES_EncryptDecrypt_WithDifferentTextLengths_ShouldWork(string plainText)
        {
            // Arrange
            var key = "TestKey123456789";

            // Act
            var encrypted = _encryptionService.EncryptAES(plainText, key);
            var decrypted = _encryptionService.DecryptAES(encrypted, key);

            // Assert
            Assert.Equal(plainText, decrypted);
        }
    }
}
