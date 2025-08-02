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
            var result = await _coinDeskService.GetCurrentPriceAsync();
            var response = ApiResponse<CoinDeskApiResponse>.SuccessResult(result, "Original CoinDesk data retrieved successfully");
            return Ok(response);
        }

        /// <summary>
        /// 取得轉換後的 CoinDesk 資料
        /// </summary>
        [HttpGet("transformed")]
        public async Task<ActionResult<ApiResponse<TransformedCoinDeskResponse>>> GetTransformedData()
        {
            var result = await _coinDeskService.GetTransformedDataAsync();
            var response = ApiResponse<TransformedCoinDeskResponse>.SuccessResult(result, "Transformed CoinDesk data retrieved successfully");
            return Ok(response);
        }
    }
}
