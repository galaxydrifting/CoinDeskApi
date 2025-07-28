using Microsoft.AspNetCore.Mvc;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrenciesController> _logger;

        public CurrenciesController(ICurrencyService currencyService, ILogger<CurrenciesController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        /// <summary>
        /// 取得所有幣別資料（依幣別代碼排序）
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CurrencyDto>>>> GetAllCurrencies()
        {
            _logger.LogInformation("API called: GET /api/currencies");
            var result = await _currencyService.GetAllCurrenciesAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// 取得特定幣別資料
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> GetCurrency(string id)
        {
            _logger.LogInformation("API called: GET /api/currencies/{Id}", id);
            var result = await _currencyService.GetCurrencyByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// 新增幣別資料
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> CreateCurrency([FromBody] CreateCurrencyDto createDto)
        {
            _logger.LogInformation("API called: POST /api/currencies with data: {@CreateDto}", createDto);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<CurrencyDto>.ErrorResult("Validation failed", errors));
            }

            var result = await _currencyService.CreateCurrencyAsync(createDto);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetCurrency), new { id = result.Data!.Id }, result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// 更新幣別資料
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CurrencyDto>>> UpdateCurrency(string id, [FromBody] UpdateCurrencyDto updateDto)
        {
            _logger.LogInformation("API called: PUT /api/currencies/{Id} with data: {@UpdateDto}", id, updateDto);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<CurrencyDto>.ErrorResult("Validation failed", errors));
            }

            var result = await _currencyService.UpdateCurrencyAsync(id, updateDto);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// 刪除幣別資料
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCurrency(string id)
        {
            _logger.LogInformation("API called: DELETE /api/currencies/{Id}", id);
            var result = await _currencyService.DeleteCurrencyAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
    }
}
