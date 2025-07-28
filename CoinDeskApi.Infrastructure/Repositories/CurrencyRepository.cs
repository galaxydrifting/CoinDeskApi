using Microsoft.EntityFrameworkCore;
using CoinDeskApi.Core.Entities;
using CoinDeskApi.Core.Interfaces;
using CoinDeskApi.Infrastructure.Data;

namespace CoinDeskApi.Infrastructure.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDbContext _context;

        public CurrencyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
        {
            return await _context.Currencies
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<Currency?> GetByIdAsync(string id)
        {
            return await _context.Currencies
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Currency> CreateAsync(Currency currency)
        {
            currency.CreatedAt = DateTime.UtcNow;
            currency.UpdatedAt = DateTime.UtcNow;
            
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return currency;
        }

        public async Task<Currency> UpdateAsync(Currency currency)
        {
            currency.UpdatedAt = DateTime.UtcNow;
            
            _context.Currencies.Update(currency);
            await _context.SaveChangesAsync();
            return currency;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var currency = await GetByIdAsync(id);
            if (currency == null)
                return false;

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Currencies
                .AnyAsync(c => c.Id == id);
        }
    }
}
