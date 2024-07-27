using LibraryAPI.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Response
{
    public class PenaltyResponse
    {
        public long PenaltyId { get; set; }

        public decimal DailyFee { get; set; } = 0.5M;

        public decimal? TotalFee { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? OverdueDays { get; set; }

        public string? Type { get; set; }

        public string MemberName { get; set; } = string.Empty;
    }
}
