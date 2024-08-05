using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface IPenaltyService
    {
        Task<ServiceResult<IEnumerable<PenaltyResponse>>> GetPenaltiesByMemberIdAsync(string memberId);
        Task<ServiceResult<PenaltyResponse>> GetPenaltyByIdAsync(long id);
        Task<ServiceResult<bool>> CalculatePenaltiesAsync(LoanReturnRequest loanReturnRequest);
    }
}
