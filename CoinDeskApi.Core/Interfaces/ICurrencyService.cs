using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Core.Interfaces
{
    public interface ICurrencyService
    {
        Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAllCurrenciesAsync();
        Task<ApiResponse<CurrencyDto>> GetCurrencyByIdAsync(string id);
        Task<ApiResponse<CurrencyDto>> CreateCurrencyAsync(CreateCurrencyDto createDto);
        Task<ApiResponse<CurrencyDto>> UpdateCurrencyAsync(string id, UpdateCurrencyDto updateDto);
        Task<ApiResponse<bool>> DeleteCurrencyAsync(string id);
    }
}
