using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;

namespace LibraryAPI.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<ServiceResult<IEnumerable<DepartmentResponse>>> GetAllDepartmentsAsync();

        Task<ServiceResult<DepartmentResponse>> GetDepartmentByIdAsync(short id);

        Task<ServiceResult<string>> AddDepartmentAsync(DepartmentRequest departmentRequest);

        Task<ServiceResult<bool>> UpdateDepartmentAsync(short id, DepartmentRequest departmentRequest);

        Task<ServiceResult<IEnumerable<EmployeeDepartmentResponse>>> GetEmployeesByDepartmentIdAsync(short id);

        Task<ServiceResult<bool>> DeleteDepartmentAsync(short id);
    }
}
