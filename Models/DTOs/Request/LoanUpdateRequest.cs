namespace LibraryAPI.Models.DTOs.Request
{
    public class LoanUpdateRequest
    {
        public long LoanId { get; set; }

        public string LoanStatus { get; set; } = string.Empty;
    }
}
