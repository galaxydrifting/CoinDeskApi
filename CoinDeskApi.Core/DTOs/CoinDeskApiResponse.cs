namespace CoinDeskApi.Core.DTOs
{
    public class CoinDeskApiResponse
    {
        public Time Time { get; set; } = new();
        public string Disclaimer { get; set; } = string.Empty;
        public string ChartName { get; set; } = string.Empty;
        public Dictionary<string, BpiInfo> Bpi { get; set; } = new();
    }

    public class Time
    {
        public string Updated { get; set; } = string.Empty;
        public string UpdatedISO { get; set; } = string.Empty;
        public string Updateduk { get; set; } = string.Empty;
    }

    public class BpiInfo
    {
        public string Code { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Rate_float { get; set; }
    }
}
