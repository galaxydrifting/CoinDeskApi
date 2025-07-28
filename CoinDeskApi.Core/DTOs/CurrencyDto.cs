using System.ComponentModel.DataAnnotations;

namespace CoinDeskApi.Core.DTOs
{
    public class CurrencyDto
    {
        [Required]
        [StringLength(10)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ChineseName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EnglishName { get; set; }

        [StringLength(10)]
        public string? Symbol { get; set; }
    }

    public class CreateCurrencyDto
    {
        [Required]
        [StringLength(10)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ChineseName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EnglishName { get; set; }

        [StringLength(10)]
        public string? Symbol { get; set; }
    }

    public class UpdateCurrencyDto
    {
        [Required]
        [StringLength(50)]
        public string ChineseName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EnglishName { get; set; }

        [StringLength(10)]
        public string? Symbol { get; set; }
    }
}
