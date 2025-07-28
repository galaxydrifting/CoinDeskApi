namespace CoinDeskApi.Core.DTOs
{
    public class TransformedCoinDeskResponse
    {
        public string UpdateTime { get; set; } = string.Empty;
        public List<CurrencyInfo> Currencies { get; set; } = new();
    }

    public class CurrencyInfo
    {
        public string Code { get; set; } = string.Empty;
        public string ChineseName { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
    }
}
