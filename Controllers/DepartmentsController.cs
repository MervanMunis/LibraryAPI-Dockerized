using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrieves all departments.
        /// </summary>
        /// <returns>A list of all departments.</returns>
        [HttpGet] // GET: api/Department
        [Authorize(Roles = "Librarian,DepartmentHead,HeadOfLibrary")]
        public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetDepartments()
        {
            var result = await _departmentService.GetAllDepartmentsAsync();
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific department by its ID.
        /// </summary>
        /// <param name="id">The ID of the department to retrieve.</param>
        /// <returns>The details of the department.</returns>
        [HttpGet("{id}")] // GET: api/Department/5
        [Authorize(Roles = "Librarian,DepartmentHead,HeadOfLibrary")]
        public async Task<ActionResult<DepartmentResponse>> GetDepartment(short id)
        {
            var result = await _departmentService.GetDepartmentByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new department.
        /// </summary>
        /// <param name="departmentRequest">The request body containing details of the department to add.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpPost] // POST: api/Department
        [Authorize(Roles = "HeadOfLibrary")]
        public async Task<ActionResult<string>> PostDepartment([FromBody] DepartmentRequest departmentRequest)
        {
            var result = await _departmentService.AddDepartmentAsync(departmentRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The department is successfully created.");
        }

        /// <summary>
        /// Updates an existing department.
        /// </summary>
        /// <param name="id">The ID of the department to update.</param>
        /// <param name="departmentRequest">The request body containing updated details of the department.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpPut("{id}")] // PUT: api/Department/5
        [Authorize(Roles = "HeadOfLibrary")]
        public async Task<ActionResult<bool>> PutDepartment(short id, [FromBody] DepartmentRequest departmentRequest)
        {
            var result = await _departmentService.UpdateDepartmentAsync(id, departmentRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("The department is updated successfully.");
        }

        /// <summary>
        /// Retrieves all employees associated with a specific department.
        /// </summary>
        /// <param name="id">The ID of the department whose employees to retrieve.</param>
        /// <returns>A list of employees in the specified department.</returns>
        [HttpGet("{id}/employees")]
        [Authorize(Roles = "Librarian,DepartmentHead,HeadOfLibrary")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployeesByDepartmentId(short id)
        {
            var result = await _departmentService.GetEmployeesByDepartmentIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Deletes a department.
        /// </summary>
        /// <param name="id">The ID of the department to delete.</param>
        /// <returns>A message indicating the success or failure of the operation.</returns>
        [HttpDelete("{id}")] // DELETE: api/Department/5
        [Authorize(Roles = "HeadOfLibrary")]
        public async Task<ActionResult<bool>> DeleteDepartment(short id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok("The department is deleted successfully.");
        }
    }
}
