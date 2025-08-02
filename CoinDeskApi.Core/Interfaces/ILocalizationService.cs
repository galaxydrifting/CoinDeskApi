namespace CoinDeskApi.Core.Interfaces
{
    public interface ILocalizationService
    {
        string GetString(string key);
        string GetString(string key, params object[] arguments);
    }
}
