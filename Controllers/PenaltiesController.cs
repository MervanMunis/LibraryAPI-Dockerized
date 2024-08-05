using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenaltiesController : ControllerBase
    {
        private readonly IPenaltyService _penaltyService;

        public PenaltiesController(IPenaltyService penaltyService)
        {
            _penaltyService = penaltyService;
        }

        /// <summary>
        /// Retrieves all penalties associated with a specific member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>A list of penalties for the specified member.</returns>
        [HttpGet("member")] // GET: api/Penalties/member
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<PenaltyResponse>>> GetPenaltiesByMemberId()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (memberId == null) 
            {
                memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var result = await _penaltyService.GetPenaltiesByMemberIdAsync(memberId);

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific penalty by its ID.
        /// </summary>
        /// <param name="id">The ID of the penalty.</param>
        /// <returns>The details of the penalty.</returns>
        [HttpGet("{id}")] // GET: api/Penalties/5
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<PenaltyResponse>> GetPenalty(long id)
        {
            var result = await _penaltyService.GetPenaltyByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
