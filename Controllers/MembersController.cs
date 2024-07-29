using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Retrieves all members from the database.
        /// </summary>
        /// <returns>A list of member responses.</returns>
        [HttpGet] // GET: api/Members
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<IEnumerable<MemberResponse>>> GetMembers()
        {
            var result = await _memberService.GetAllMembersAsync();

            if (!result.Success)
            {
                return Problem(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific member by their ID.
        /// </summary>
        /// <param name="id">The ID of the member.</param>
        /// <returns>The member details.</returns>
        [HttpGet] // GET: api/Members
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<MemberResponse>> GetMember()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var result = await _memberService.GetMemberByIdNumberAsync(memberId);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a specific member by their ID number.
        /// </summary>
        /// <param name="idNumber">The ID number of the member.</param>
        /// <returns>The member details.</returns>
        [HttpGet("{idNumber}")] // GET: api/Members/{IdNumber}
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<MemberResponse>> GetMemberByIdNumber(string idNumber)
        {
            var result = await _memberService.GetMemberByIdNumberAsync(idNumber);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Adds a new member to the database.
        /// </summary>
        /// <param name="memberRequest">The member details.</param>
        /// <returns>A success message if the member is created successfully.</returns>
        [HttpPost] // POST: api/Members
        public async Task<ActionResult<string>> PostMember([FromBody] MemberRequest memberRequest)
        {
            var result = await _memberService.AddMemberAsync(memberRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Welcome to the library.");
        }

        /// <summary>
        /// Updates an existing member's details.
        /// </summary>
        /// <param name="memberRequest">The member update details.</param>
        /// <returns>A success message if the member is updated successfully.</returns>
        [HttpPut] // PUT: api/Members
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<string>> PutMember([FromBody] MemberRequest memberRequest)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _memberService.UpdateMemberAsync(memberId, memberRequest);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The member is updated successfully.");
        }


        /// <summary>
        /// Removes a member by setting their status to removed.
        /// </summary>
        /// <returns>A success message if the member is removed successfully.</returns>
        [HttpPatch("Remove")] // Patch: api/Members/5
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<string>> RemoveMember()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _memberService.SetMemberStatusAsync(memberId, MemberStatus.RemovedAccount.ToString());

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The member account is set to removed.");
        }

        /// <summary>
        /// Sets the status of a member to blocked.
        /// </summary>
        /// <param name="idNumber">The ID number of the member.</param>
        /// <returns>A success message if the status is updated successfully.</returns>
        [HttpPatch("{id}/status/blocked")] // PATCH: api/Members/5/status/blocked
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<string>> SetMemberBlockedStatus(string idNumber)
        {
            var result = await _memberService.SetMemberStatusAsync(idNumber, MemberStatus.BlockedAccount.ToString());

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The member account is set to blocked.");
        }

        /// <summary>
        /// Updates a member's password.
        /// </summary>
        /// <param name="updatePasswordDTO">The current and new password details.</param>
        /// <returns>A success message if the password is updated successfully.</returns>
        [HttpPatch("password")] // PATCH: api/Members/5/password
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<string>> UpdateMemberPassword([FromBody] UpdatePasswordDTO updatePasswordDTO)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _memberService.UpdateMemberPasswordAsync(memberId, updatePasswordDTO.CurrentPassword, updatePasswordDTO.NewPassword);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The password is updated successfully.");
        }

        /// <summary>
        /// Adds a new address for a member.
        /// </summary>
        /// <param name="memberAddress">The member address details.</param>
        /// <returns>A success message if the address is added successfully.</returns>
        [HttpPost("address")] // POST: api/Members/5/address
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<string>> PostMemberAddress([FromBody] MemberAddress memberAddress)
        {
            memberAddress.MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _memberService.AddMemberAddressAsync(memberAddress);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok("The member's address is added successfully.");
        }
    }
}
