using System.ComponentModel.DataAnnotations;

namespace CoinDeskApi.Core.Entities
{
    public class Currency
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; } = string.Empty; // 幣別代碼

        [Required]
        [StringLength(50)]
        public string ChineseName { get; set; } = string.Empty; // 中文名稱

        [StringLength(100)]
        public string? EnglishName { get; set; } // 英文名稱

        [StringLength(10)]
        public string? Symbol { get; set; } // 符號

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
