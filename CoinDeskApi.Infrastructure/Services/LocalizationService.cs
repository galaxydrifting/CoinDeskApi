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
            return _localizer[key];
        }

        public string GetString(string key, params object[] arguments)
        {
            return _localizer[key, arguments];
        }
    }
}
