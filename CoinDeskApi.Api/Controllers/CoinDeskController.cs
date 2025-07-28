using Microsoft.AspNetCore.Mvc;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoinDeskController : ControllerBase
    {
        private readonly ICoinDeskService _coinDeskService;
        private readonly ILogger<CoinDeskController> _logger;

        public CoinDeskController(ICoinDeskService coinDeskService, ILogger<CoinDeskController> logger)
        {
            _coinDeskService = coinDeskService;
            _logger = logger;
        }

        /// <summary>
        /// 取得 CoinDesk API 原始資料
        /// </summary>
        [HttpGet("original")]
        public async Task<ActionResult<ApiResponse<CoinDeskApiResponse>>> GetOriginalData()
        {
            _logger.LogInformation("API called: GET /api/coindesk/original");
            
            try
            {
                var result = await _coinDeskService.GetCurrentPriceAsync();
                var response = ApiResponse<CoinDeskApiResponse>.SuccessResult(result, "Original CoinDesk data retrieved successfully");
                
                _logger.LogInformation("API response: {@Response}", response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting original CoinDesk data");
                var errorResponse = ApiResponse<CoinDeskApiResponse>.ErrorResult("Failed to retrieve CoinDesk data");
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// 取得轉換後的 CoinDesk 資料
        /// </summary>
        [HttpGet("transformed")]
        public async Task<ActionResult<ApiResponse<TransformedCoinDeskResponse>>> GetTransformedData()
        {
            _logger.LogInformation("API called: GET /api/coindesk/transformed");
            
            try
            {
                var result = await _coinDeskService.GetTransformedDataAsync();
                var response = ApiResponse<TransformedCoinDeskResponse>.SuccessResult(result, "Transformed CoinDesk data retrieved successfully");
                
                _logger.LogInformation("API response: {@Response}", response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transformed CoinDesk data");
                var errorResponse = ApiResponse<TransformedCoinDeskResponse>.ErrorResult("Failed to retrieve transformed CoinDesk data");
                return StatusCode(500, errorResponse);
            }
        }
    }
}
