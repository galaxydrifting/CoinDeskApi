using Microsoft.Extensions.Localization;
using CoinDeskApi.Core.Interfaces;

namespace CoinDeskApi.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create("Messages", "CoinDeskApi.Api");
        }

        public string GetString(string key)
        {
            var localized = _localizer[key];
            return localized.ResourceNotFound ? key : localized.Value;
        }

        public string GetString(string key, params object[] arguments)
        {
            var localized = _localizer[key, arguments];
            return localized.ResourceNotFound ? key : localized.Value;
        }
    }
}
