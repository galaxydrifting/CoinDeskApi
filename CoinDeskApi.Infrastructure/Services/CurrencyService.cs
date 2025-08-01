using AutoMapper;
using CoinDeskApi.Core.DTOs;
using CoinDeskApi.Core.Entities;
using CoinDeskApi.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoinDeskApi.Infrastructure.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(ICurrencyRepository currencyRepository, IMapper mapper, ILogger<CurrencyService> logger)
        {
            _currencyRepository = currencyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAllCurrenciesAsync()
        {
            try
            {
                _logger.LogInformation("Getting all currencies");
                var currencies = await _currencyRepository.GetAllAsync();
                var currencyDtos = _mapper.Map<IEnumerable<CurrencyDto>>(currencies);

                return ApiResponse<IEnumerable<CurrencyDto>>.SuccessResult(currencyDtos, "Currencies retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all currencies");
                return ApiResponse<IEnumerable<CurrencyDto>>.ErrorResult("Failed to retrieve currencies");
            }
        }

        public async Task<ApiResponse<CurrencyDto>> GetCurrencyByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Getting currency with ID: {CurrencyId}", id);
                var currency = await _currencyRepository.GetByIdAsync(id);

                if (currency == null)
                {
                    return ApiResponse<CurrencyDto>.ErrorResult($"Currency with ID '{id}' not found");
                }

                var currencyDto = _mapper.Map<CurrencyDto>(currency);
                return ApiResponse<CurrencyDto>.SuccessResult(currencyDto, "Currency retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currency with ID: {CurrencyId}", id);
                return ApiResponse<CurrencyDto>.ErrorResult("Failed to retrieve currency");
            }
        }

        public async Task<ApiResponse<CurrencyDto>> CreateCurrencyAsync(CreateCurrencyDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating currency with ID: {CurrencyId}", createDto.Id);

                if (await _currencyRepository.ExistsAsync(createDto.Id))
                {
                    return ApiResponse<CurrencyDto>.ErrorResult($"Currency with ID '{createDto.Id}' already exists");
                }

                var currency = _mapper.Map<Currency>(createDto);
                var createdCurrency = await _currencyRepository.CreateAsync(currency);
                var currencyDto = _mapper.Map<CurrencyDto>(createdCurrency);

                return ApiResponse<CurrencyDto>.SuccessResult(currencyDto, "Currency created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating currency with ID: {CurrencyId}", createDto.Id);
                return ApiResponse<CurrencyDto>.ErrorResult("Failed to create currency");
            }
        }

        public async Task<ApiResponse<CurrencyDto>> UpdateCurrencyAsync(string id, UpdateCurrencyDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating currency with ID: {CurrencyId}", id);

                var existingCurrency = await _currencyRepository.GetByIdAsync(id);
                if (existingCurrency == null)
                {
                    return ApiResponse<CurrencyDto>.ErrorResult($"Currency with ID '{id}' not found");
                }

                _mapper.Map(updateDto, existingCurrency);
                var updatedCurrency = await _currencyRepository.UpdateAsync(existingCurrency);
                var currencyDto = _mapper.Map<CurrencyDto>(updatedCurrency);

                return ApiResponse<CurrencyDto>.SuccessResult(currencyDto, "Currency updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating currency with ID: {CurrencyId}", id);
                return ApiResponse<CurrencyDto>.ErrorResult("Failed to update currency");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCurrencyAsync(string id)
        {
            try
            {
                _logger.LogInformation("Deleting currency with ID: {CurrencyId}", id);

                var deleted = await _currencyRepository.DeleteAsync(id);
                if (!deleted)
                {
                    return ApiResponse<bool>.ErrorResult($"Currency with ID '{id}' not found");
                }

                return ApiResponse<bool>.SuccessResult(true, "Currency deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting currency with ID: {CurrencyId}", id);
                return ApiResponse<bool>.ErrorResult("Failed to delete currency");
            }
        }
    }
}
