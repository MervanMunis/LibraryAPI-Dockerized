using LibraryAPI.Data;
using LibraryAPI.DTOs;
using LibraryAPI.Entities.Models;
using LibraryAPI.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.DTOs.Response;
using LibraryAPI.DTOs.Request;

namespace LibraryAPI.Services.impl
{
    public class EmployeeService : IEmployeeService
    {
        private readonly LibraryAPIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeService(LibraryAPIContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves all employees from the database.
        /// </summary>
        /// <returns>A list of employee responses.</returns>
        public async Task<ServiceResult<IEnumerable<EmployeeResponse>>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees!
                .Include(e => e.ApplicationUser)
                .Include(e => e.Department)
                .Include(e => e.EmployeeAddresses)
                .Select(e => new EmployeeResponse()
                {
                    EmployeeId = e.EmployeeId,
                    IdNumber = e.ApplicationUser!.IdNumber,
                    Name = e.ApplicationUser.Name,
                    LastName = e.ApplicationUser.LastName,
                    UserName = e.ApplicationUser.UserName,
                    Email = e.ApplicationUser.Email,
                    PhoneNumber = e.ApplicationUser.PhoneNumber,
                    Gender = e.ApplicationUser.Gender,
                    BirthDate = e.ApplicationUser.BirthDate,
                    RegisterDate = e.ApplicationUser.RegisterDate,
                    Status = e.Status,
                    Salary = e.Salary,
                    EmployeeShift = e.EmployeeShift,
                    EmployeeTitle = e.EmployeeTitle
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<EmployeeResponse>>.SuccessResult(employees);
        }

        /// <summary>
        /// Retrieves a specific employee by their ID number.
        /// </summary>
        /// <param name="idNumber">The ID number of the employee.</param>
        /// <returns>An employee response if found, otherwise an error message.</returns>
        public async Task<ServiceResult<EmployeeResponse>> GetEmployeeByIdNumberAsync(string idNumber)
        {
            var employee = await _context.Employees!
                .Include(e => e.ApplicationUser)
                .Include(e => e.Department)
                .Include(e => e.EmployeeAddresses)
                .Select(e => new EmployeeResponse()
                {
                    EmployeeId = e.EmployeeId,
                    IdNumber = e.ApplicationUser!.IdNumber,
                    Name = e.ApplicationUser.Name,
                    LastName = e.ApplicationUser.LastName,
                    Gender = e.ApplicationUser.Gender,
                    BirthDate = e.ApplicationUser.BirthDate,
                    RegisterDate = e.ApplicationUser.RegisterDate,
                    Status = e.Status,
                    Salary = e.Salary,
                    EmployeeShift = e.EmployeeShift,
                    EmployeeTitle = e.EmployeeTitle
                })
                .FirstOrDefaultAsync(e => e.IdNumber == idNumber);

            if (employee == null)
            {
                return ServiceResult<EmployeeResponse>.FailureResult("Employee not found");
            }

            return ServiceResult<EmployeeResponse>.SuccessResult(employee);
        }

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employeeRequest">The employee request data.</param>
        /// <returns>A success message if the employee is created successfully.</returns>
        public async Task<ServiceResult<string>> AddEmployeeAsync(EmployeeRequest employeeRequest)
        {
            // Check if a user with the provided email already exists
            var existingUser = await _userManager.FindByEmailAsync(employeeRequest.Email);
            if (existingUser != null)
            {
                return ServiceResult<string>.FailureResult("A user with this email already exists.");
            }

            var department = _context.Departments!.FirstOrDefaultAsync(d => d.DepartmentId == employeeRequest.DepartmentId);

            if (department == null)
            {
                return ServiceResult<string>.FailureResult("The department does not exist.");
            }

            // Create a new ApplicationUser object
            var user = new ApplicationUser
            {
                IdNumber = employeeRequest.IdNumber,
                Name = employeeRequest.Name,
                LastName = employeeRequest.LastName,
                Gender = employeeRequest.Gender,
                BirthDate = employeeRequest.BirthDate,
                UserName = employeeRequest.UserName,
                PhoneNumber = employeeRequest.PhoneNumber,
                Email = employeeRequest.Email,
            };

            // Create the user in the Identity system
            var result = await _userManager.CreateAsync(user, employeeRequest.Password);
            if (!result.Succeeded)
            {
                return ServiceResult<string>.FailureResult(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Assign role to the user based on employee title
            var roleResult = await _userManager.AddToRoleAsync(user, employeeRequest.EmployeeTitle!.ToString());
            if (!roleResult.Succeeded)
            {
                return ServiceResult<string>.FailureResult(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Create a new Employee record
            var employee = new Employee
            {
                EmployeeId = user.Id,
                Salary = employeeRequest.Salary,
                EmployeeShift = employeeRequest.EmployeeShift,
                EmployeeTitle = employeeRequest.EmployeeTitle,
                DepartmentId = employeeRequest.DepartmentId,
                Status = EmployeeStatus.Working.ToString()
            };

            // Add employee to the database
            await _context.Employees!.AddAsync(employee);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Employee successfully created!");
        }

        /// <summary>
        /// Updates an existing employee's details.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="employeeRequest">The updated employee request data.</param>
        /// <returns>A success message if the employee is updated successfully.</returns>
        public async Task<ServiceResult<bool>> UpdateEmployeeAsync(string id, EmployeeRequest employeeRequest)
        {
            // Find the employee by ID
            var employee = await _context.Employees!.FindAsync(id);
            if (employee == null)
            {
                return ServiceResult<bool>.FailureResult("Employee not found");
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return ServiceResult<bool>.FailureResult("User not found");
            }

            // Update user details
            user.IdNumber = employeeRequest.IdNumber;
            user.Name = employeeRequest.Name;
            user.LastName = employeeRequest.LastName;
            user.Gender = employeeRequest.Gender;
            user.BirthDate = employeeRequest.BirthDate;
            user.UserName = employeeRequest.UserName;
            user.PhoneNumber = employeeRequest.PhoneNumber;
            user.Email = employeeRequest.Email;

            var userResult = await _userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                return ServiceResult<bool>.FailureResult(string.Join(", ", userResult.Errors.Select(e => e.Description)));
            }

            // Update role if the title has changed
            if (employee.EmployeeTitle != employeeRequest.EmployeeTitle)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove the current role
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return ServiceResult<bool>.FailureResult(string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                }

                // Add the new role based on the updated title
                var addRoleResult = await _userManager.AddToRoleAsync(user, employeeRequest.EmployeeTitle!.ToString());
                if (!addRoleResult.Succeeded)
                {
                    return ServiceResult<bool>.FailureResult(string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                }
            }

            // Update employee details
            employee.Salary = employeeRequest.Salary;
            employee.EmployeeShift = employeeRequest.EmployeeShift;
            employee.EmployeeTitle = employeeRequest.EmployeeTitle;
            employee.DepartmentId = employeeRequest.DepartmentId;

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Employees.AnyAsync(e => e.EmployeeId == id))
                {
                    return ServiceResult<bool>.FailureResult("Employee not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of an employee.
        /// </summary>
        /// <param name="id">The ID of the employee whose status is to be updated.</param>
        /// <param name="status">The new status.</param>
        /// <returns>A success message if the employee's status is updated successfully.</returns>
        public async Task<ServiceResult<bool>> SetEmployeeStatusAsync(string id, string status)
        {
            var employee = await _context.Employees!.FindAsync(id);
            if (employee == null)
            {
                return ServiceResult<bool>.FailureResult("Employee not found");
            }

            employee.Status = status;

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Employees.AnyAsync(e => e.EmployeeId == id))
                {
                    return ServiceResult<bool>.FailureResult("Employee not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds a new address for an employee.
        /// </summary>
        /// <param name="employeeAddress">The employee address data.</param>
        /// <returns>A success message if the address is added successfully.</returns>
        public async Task<ServiceResult<bool>> AddEmployeeAddressAsync(EmployeeAddress employeeAddress)
        {
            var employee = await _context.Employees!.FindAsync(employeeAddress.EmployeeId);
            if (employee == null)
            {
                return ServiceResult<bool>.FailureResult("Employee not found");
            }

            await _context.EmployeeAddresses!.AddAsync(employeeAddress);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }

        /// <summary>
        /// Updates an employee's password.
        /// </summary>
        /// <param name="id">The ID of the employee whose password is to be updated.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>A success message if the password is updated successfully.</returns>
        public async Task<ServiceResult<bool>> UpdateEmployeePasswordAsync(string id, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return ServiceResult<bool>.FailureResult("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return ServiceResult<bool>.FailureResult(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return ServiceResult<bool>.SuccessResult(true);
        }
    }
}
