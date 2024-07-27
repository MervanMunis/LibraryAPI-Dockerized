using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

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
