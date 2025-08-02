using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Api.Filters
{
    public class EncryptionExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<EncryptionExceptionFilter> _logger;

        public EncryptionExceptionFilter(ILogger<EncryptionExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // 記錄加密相關錯誤
            _logger.LogError(exception, "Encryption operation failed in {ControllerName}.{ActionName}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);

            var response = new ApiResponse<object>
            {
                Success = false,
                Errors = new List<string> { exception.Message }
            };

            switch (exception)
            {
                case NullReferenceException:
                    context.Result = new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Null reference error in encryption operation",
                        Errors = new List<string> { "A null reference was encountered during encryption operation. This could be a simulated error for testing." }
                    });
                    break;

                case ArgumentNullException:
                    context.Result = new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Required parameter is missing",
                        Errors = new List<string> { "Please provide all required fields for encryption/decryption" }
                    });
                    break;

                case ArgumentException argEx when argEx.Message.Contains("key"):
                    context.Result = new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid encryption key",
                        Errors = new List<string> { "The provided encryption key is invalid or has incorrect format" }
                    });
                    break;

                case FormatException:
                    context.Result = new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid data format",
                        Errors = new List<string> { "The provided data format is incorrect for the requested operation" }
                    });
                    break;

                case CryptographicException cryptoEx:
                    context.Result = new BadRequestObjectResult(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cryptographic operation failed",
                        Errors = new List<string> { "The encryption/decryption operation could not be completed. Please check your input data." }
                    });
                    break;

                case OutOfMemoryException:
                    context.Result = new StatusCodeResult(507); // Insufficient Storage
                    break;

                default:
                    // 讓其他例外由 GlobalExceptionMiddleware 處理
                    return;
            }

            // 標記例外已被處理
            context.ExceptionHandled = true;
        }
    }
}
