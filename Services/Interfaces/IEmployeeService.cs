using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;

namespace LibraryAPI.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<ServiceResult<IEnumerable<EmployeeResponse>>> GetAllEmployeesAsync();

        Task<ServiceResult<EmployeeResponse>> GetEmployeeByIdNumberAsync(string idNumber);

        Task<ServiceResult<string>> AddEmployeeAsync(EmployeeRequest employeeRequest);

        Task<ServiceResult<bool>> UpdateEmployeeAsync(string id, EmployeeRequest employeeRequest);

        Task<ServiceResult<bool>> SetEmployeeStatusAsync(string id, string status);

        Task<ServiceResult<bool>> AddEmployeeAddressAsync(EmployeeAddress employeeAddress);

        Task<ServiceResult<bool>> UpdateEmployeePasswordAsync(string id, string currentPassword, string newPassword);
    }
}
