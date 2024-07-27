using LibraryAPI.Entities.Enums;

namespace LibraryAPI.DTOs.Request
{
    public class LoanUpdateRequest
    {
        public long LoanId { get; set; }

        public string LoanStatus { get; set; } = string.Empty;
    }
}
