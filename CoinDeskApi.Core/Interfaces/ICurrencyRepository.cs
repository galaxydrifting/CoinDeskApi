using CoinDeskApi.Core.DTOs;
using CoinDeskApi.Core.Entities;

namespace CoinDeskApi.Core.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<Currency?> GetByIdAsync(string id);
        Task<Currency> CreateAsync(Currency currency);
        Task<Currency> UpdateAsync(Currency currency);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
