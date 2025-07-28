using Microsoft.AspNetCore.Mvc;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncryptionController : ControllerBase
    {
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<EncryptionController> _logger;

        public EncryptionController(IEncryptionService encryptionService, ILogger<EncryptionController> logger)
        {
            _encryptionService = encryptionService;
            _logger = logger;
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        [HttpPost("aes/encrypt")]
        public ActionResult<ApiResponse<string>> EncryptAES([FromBody] EncryptRequest request)
        {
            _logger.LogInformation("API called: POST /api/encryption/aes/encrypt");

            try
            {
                var encrypted = _encryptionService.EncryptAES(request.PlainText, request.Key);
                var response = ApiResponse<string>.SuccessResult(encrypted, "Text encrypted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AES encryption");
                var errorResponse = ApiResponse<string>.ErrorResult("Encryption failed");
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        [HttpPost("aes/decrypt")]
        public ActionResult<ApiResponse<string>> DecryptAES([FromBody] DecryptRequest request)
        {
            _logger.LogInformation("API called: POST /api/encryption/aes/decrypt");

            try
            {
                var decrypted = _encryptionService.DecryptAES(request.CipherText, request.Key);
                var response = ApiResponse<string>.SuccessResult(decrypted, "Text decrypted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AES decryption");
                var errorResponse = ApiResponse<string>.ErrorResult("Decryption failed");
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// 產生 RSA 金鑰對
        /// </summary>
        [HttpPost("rsa/generate-keys")]
        public ActionResult<ApiResponse<RSAKeyPair>> GenerateRSAKeys()
        {
            _logger.LogInformation("API called: POST /api/encryption/rsa/generate-keys");

            try
            {
                var (publicKey, privateKey) = _encryptionService.GenerateRSAKeyPair();
                var keyPair = new RSAKeyPair { PublicKey = publicKey, PrivateKey = privateKey };
                var response = ApiResponse<RSAKeyPair>.SuccessResult(keyPair, "RSA key pair generated successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating RSA key pair");
                var errorResponse = ApiResponse<RSAKeyPair>.ErrorResult("Key generation failed");
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        [HttpPost("rsa/encrypt")]
        public ActionResult<ApiResponse<string>> EncryptRSA([FromBody] RSAEncryptRequest request)
        {
            _logger.LogInformation("API called: POST /api/encryption/rsa/encrypt");

            try
            {
                var encrypted = _encryptionService.EncryptRSA(request.PlainText, request.PublicKey);
                var response = ApiResponse<string>.SuccessResult(encrypted, "Text encrypted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RSA encryption");
                var errorResponse = ApiResponse<string>.ErrorResult("Encryption failed");
                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        [HttpPost("rsa/decrypt")]
        public ActionResult<ApiResponse<string>> DecryptRSA([FromBody] RSADecryptRequest request)
        {
            _logger.LogInformation("API called: POST /api/encryption/rsa/decrypt");

            try
            {
                var decrypted = _encryptionService.DecryptRSA(request.CipherText, request.PrivateKey);
                var response = ApiResponse<string>.SuccessResult(decrypted, "Text decrypted successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RSA decryption");
                var errorResponse = ApiResponse<string>.ErrorResult("Decryption failed");
                return BadRequest(errorResponse);
            }
        }
    }

    public class EncryptRequest
    {
        public string PlainText { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }

    public class DecryptRequest
    {
        public string CipherText { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
    }

    public class RSAKeyPair
    {
        public string PublicKey { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
    }

    public class RSAEncryptRequest
    {
        public string PlainText { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }

    public class RSADecryptRequest
    {
        public string CipherText { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
    }
}
