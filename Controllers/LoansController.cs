using System.Security.Claims;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Retrieves loans by the currently logged-in employee.
        /// </summary>
        /// <returns>A list of loans associated with the employee.</returns>
        [HttpGet("employee/")] // GET: api/Loans/employee
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<IEnumerable<LoanResponse>>> GetLoansByEmployeeId()
        {
            var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _loanService.GetLoansByEmployeeIdAsync(employeeId);

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves loans by the currently logged-in member.
        /// </summary>
        /// <returns>A list of loans associated with the member.</returns>
        [HttpGet("member")] // GET: api/Loans/member
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<LoanResponse>>> GetLoansByMemberId()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _loanService.GetLoansByMemberIdAsync(memberId);
            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific loan by its ID.
        /// </summary>
        /// <param name="id">The ID of the loan.</param>
        /// <returns>The loan details.</returns>
        [HttpGet("{id}")] // GET: api/Loans/5
        [Authorize(Roles = "Member,Librarian")]
        public async Task<ActionResult<LoanResponse>> GetLoan(int id)
        {
            var result = await _loanService.GetLoanByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new loan.
        /// </summary>
        /// <param name="loanRequest">The loan details.</param>
        /// <returns>A success message if the loan is created successfully.</returns>
        [HttpPost] // POST: api/Loans
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PostLoan([FromBody] LoanRequest loanRequest)
        {
            loanRequest.EmployeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _loanService.AddLoanAsync(loanRequest);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.SuccessMessage);
        }

        /// <summary>
        /// Updates an existing loan's status.
        /// </summary>
        /// <param name="id">The ID of the loan to update.</param>
        /// <param name="loanUpdateDTO">The loan update details.</param>
        /// <returns>A success message if the loan is updated successfully.</returns>
        [HttpPut("{id}")] // PUT: api/Loans/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> PutLoan(int id, [FromBody] LoanUpdateRequest loanUpdateDTO)
        {
            loanUpdateDTO.LoanId = id;
            var result = await _loanService.UpdateLoanAsync(loanUpdateDTO);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Loan status updated successfully.");
        }

        /// <summary>
        /// Marks a loan as returned and updates the book copy status.
        /// </summary>
        /// <param name="id">The ID of the loan to return.</param>
        /// <returns>A success message if the book is returned successfully.</returns>
        [HttpPatch("{id}/return")] // PATCH: api/Loans/5/return
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> ReturnBook(int id)
        {
            var result = await _loanService.ReturnBookAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok("Book returned successfully.");
        }
    }
}
