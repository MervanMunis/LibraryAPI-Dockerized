using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface ILoanService
    {
        Task<ServiceResult<IEnumerable<LoanResponse>>> GetLoansByEmployeeIdAsync(string employeeId);
        Task<ServiceResult<IEnumerable<LoanResponse>>> GetLoansByMemberIdAsync(string memberId);
        Task<ServiceResult<LoanResponse>> GetLoanByIdAsync(int id);
        Task<ServiceResult<string>> AddLoanAsync(LoanRequest loanRequest);
        Task<ServiceResult<bool>> UpdateLoanAsync(LoanUpdateRequest loanUpdateRequest);
        Task<ServiceResult<bool>> ReturnBookAsync(int id);
    }
}
