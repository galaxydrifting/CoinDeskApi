using CoinDeskApi.Core.DTOs;

namespace CoinDeskApi.Core.Interfaces
{
    public interface ICoinDeskService
    {
        Task<CoinDeskApiResponse> GetCurrentPriceAsync();
        Task<TransformedCoinDeskResponse> GetTransformedDataAsync();
    }
}
