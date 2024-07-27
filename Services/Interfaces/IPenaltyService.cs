using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

namespace LibraryAPI.Services.Interfaces
{
    public interface IPenaltyService
    {
        Task<ServiceResult<IEnumerable<PenaltyResponse>>> GetPenaltiesByMemberIdAsync(string memberId);
        Task<ServiceResult<PenaltyResponse>> GetPenaltyByIdAsync(long id);
        Task<ServiceResult<bool>> CalculatePenaltiesAsync(LoanReturnRequest loanReturnRequest);
    }
}
