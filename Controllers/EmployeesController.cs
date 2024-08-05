using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;
using LibraryAPI.Models.Enums;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HeadOfLibrary")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        /// <returns>A list of employee responses.</returns>
        [HttpGet] // GET: /api/Employees
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific employee by their ID number.
        /// </summary>
        /// <param name="idNumber">The ID number of the employee.</param>
        /// <returns>An employee response if found, otherwise an error message.</returns>
        [HttpGet("{idNumber}")] // GET: /api/Employees/TC numarası
        public async Task<ActionResult<EmployeeResponse>> GetEmployee(string idNumber)
        {
            var result = await _employeeService.GetEmployeeByIdNumberAsync(idNumber);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new employee.
        /// </summary>
        /// <param name="employeeRequest">The employee request data.</param>
        /// <returns>A success message if the employee is created successfully.</returns>
        [HttpPost] // POST: /api/Employees
        public async Task<ActionResult<string>> PostEmployee([FromBody] EmployeeRequest employeeRequest)
        {
            var result = await _employeeService.AddEmployeeAsync(employeeRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("welcome to the club.");
        }

        /// <summary>
        /// Updates an existing employee's details.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="employeeRequest">The updated employee request data.</param>
        /// <returns>A success message if the employee is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: /api/Employees/2
        public async Task<ActionResult<string>> PutEmployee(string id, [FromBody] EmployeeRequest employeeRequest)
        {
            var result = await _employeeService.UpdateEmployeeAsync(id, employeeRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The employee is updated successfully.");
        }

        /// <summary>
        /// Sets an employee's status to active.
        /// </summary>
        /// <param name="id">The ID of the employee whose status is to be updated.</param>
        /// <returns>A success message if the employee's status is updated successfully.</returns>
        [HttpPatch("{id}/status/active")] // PATCH: /api/Employees/3/status/active
        public async Task<ActionResult<string>> SetEmployeeActiveStatus(string id)
        {
            var result = await _employeeService.SetEmployeeStatusAsync(id, EmployeeStatus.Working.ToString());

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The employee is set to active status.");
        }

        /// <summary>
        /// Sets an employee's status to quit.
        /// </summary>
        /// <param name="id">The ID of the employee whose status is to be updated.</param>
        /// <returns>A success message if the employee's status is updated successfully.</returns>
        [HttpPatch("{id}/status/quit")] // PATCH: /api/Employees/3/status/quit
        public async Task<ActionResult<string>> SetEmployeeQuitStatus(string id)
        {
            var result = await _employeeService.SetEmployeeStatusAsync(id, EmployeeStatus.Quit.ToString());

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The employee is set to quit status.");
        }

        /// <summary>
        /// Updates an employee's password.
        /// </summary>
        /// <param name="id">The ID of the employee whose password is to be updated.</param>
        /// <param name="updatePasswordDTO">The current and new password data.</param>
        /// <returns>A success message if the password is updated successfully.</returns>
        [HttpPatch("{id}/password")] // PATCH: /api/Employees/3/password
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> UpdateEmployeePassword(string id, [FromBody] UpdatePasswordRequest updatePasswordDTO)
        {
            var result = await _employeeService.UpdateEmployeePasswordAsync(id, updatePasswordDTO.CurrentPassword, updatePasswordDTO.NewPassword);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The password is updated successfully.");
        }

        /// <summary>
        /// Adds an address for an employee.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <param name="employeeAddress">The employee address data.</param>
        /// <returns>A success message if the address is added successfully.</returns>
        [HttpPost("{id}/address")] // POST: /api/Employees/3/address
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostEmployeeAddress(string id, [FromBody] EmployeeAddress employeeAddress)
        {
            employeeAddress.EmployeeId = id;

            var result = await _employeeService.AddEmployeeAddressAsync(employeeAddress);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The employee's address is added successfully.");
        }
    }
}
