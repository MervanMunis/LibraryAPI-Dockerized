using LibraryAPI.Data;
using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Services.impl
{
    /// <summary>
    /// Service class for handling department-related operations.
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly LibraryAPIContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentService"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing the database.</param>
        public DepartmentService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all departments from the database.
        /// </summary>
        /// <returns>A service result containing a list of departments.</returns>
        public async Task<ServiceResult<IEnumerable<DepartmentResponse>>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments!
                .Select(d => new DepartmentResponse()
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<DepartmentResponse>>.SuccessResult(departments);
        }

        /// <summary>
        /// Retrieves a specific department by its ID.
        /// </summary>
        /// <param name="id">The ID of the department to retrieve.</param>
        /// <returns>A service result containing the department details.</returns>
        public async Task<ServiceResult<DepartmentResponse>> GetDepartmentByIdAsync(short id)
        {
            var department = await _context.Departments!
                .Select(d => new DepartmentResponse()
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name
                })
                .FirstAsync();
            if (department == null)
            {
                return ServiceResult<DepartmentResponse>.FailureResult("Department not found");
            }
            return ServiceResult<DepartmentResponse>.SuccessResult(department);
        }

        /// <summary>
        /// Adds a new department to the database.
        /// </summary>
        /// <param name="departmentRequest">The request body containing details of the department to add.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<string>> AddDepartmentAsync(DepartmentRequest departmentRequest)
        {
            var department = new Department
            {
                Name = departmentRequest.Name
            };

            await _context.Departments!.AddAsync(department);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Department successfully created!");
        }

        /// <summary>
        /// Updates an existing department in the database.
        /// </summary>
        /// <param name="id">The ID of the department to update.</param>
        /// <param name="departmentRequest">The request body containing updated details of the department.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> UpdateDepartmentAsync(short id, DepartmentRequest departmentRequest)
        {
            var department = await _context.Departments!.FindAsync(id);
            if (department == null)
            {
                return ServiceResult<bool>.FailureResult("Department not found");
            }

            department.Name = departmentRequest.Name;
            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Departments.AnyAsync(d => d.DepartmentId == id))
                {
                    return ServiceResult<bool>.FailureResult("Department not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves all employees associated with a specific department.
        /// </summary>
        /// <param name="id">The ID of the department whose employees to retrieve.</param>
        /// <returns>A service result containing a list of employees in the department.</returns>
        public async Task<ServiceResult<IEnumerable<EmployeeDepartmentResponse>>> GetEmployeesByDepartmentIdAsync(short id)
        {
            var department = await _context.Departments!
                .Include(d => d.Employees)!
                .ThenInclude(e => e.ApplicationUser)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return ServiceResult<IEnumerable<EmployeeDepartmentResponse>>.FailureResult("Department not found");
            }

            var employeeResponses = department.Employees!.Select(e => new EmployeeDepartmentResponse
            {
                EmployeeId = e.EmployeeId,
                Name = e.ApplicationUser!.Name,
                LastName = e.ApplicationUser.LastName,
                Email = e.ApplicationUser.Email,
                PhoneNumber = e.ApplicationUser.PhoneNumber,
                Gender = e.ApplicationUser.Gender,
                EmployeeTitle = e.EmployeeTitle,
                Status = e.Status
            }).ToList();

            return ServiceResult<IEnumerable<EmployeeDepartmentResponse>>.SuccessResult(employeeResponses);
        }

        /// <summary>
        /// Deletes a department from the database.
        /// </summary>
        /// <param name="id">The ID of the department to delete.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult<bool>> DeleteDepartmentAsync(short id)
        {
            var department = await _context.Departments!.FindAsync(id);
            if (department == null)
            {
                return ServiceResult<bool>.FailureResult("Department not found");
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.SuccessResult(true);
        }
    }
}
