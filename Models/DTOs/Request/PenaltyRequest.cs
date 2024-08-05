namespace LibraryAPI.Models.DTOs.Request
{
    public class PenaltyRequest
    {
        public decimal DailyFee { get; set; } = 0.5M;

        public string MemberId { get; set; } = string.Empty;
    }
}
