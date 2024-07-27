using LibraryAPI.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Request
{
    public class PenaltyRequest
    {
        public decimal DailyFee { get; set; } = 0.5M;

        public string MemberId { get; set; } = string.Empty;
    }
}
