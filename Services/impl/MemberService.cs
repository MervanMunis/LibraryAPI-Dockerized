using LibraryAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using LibraryAPI.Models.Enums;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;

namespace LibraryAPI.Services.impl
{
    public class MemberService : IMemberService
    {
        private readonly LibraryAPIContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberService(LibraryAPIContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves all members from the database.
        /// </summary>
        /// <returns>A list of member responses.</returns>
        public async Task<ServiceResult<IEnumerable<MemberResponse>>> GetAllMembersAsync()
        {
            var members = await _context.Members!
                .Include(m => m.ApplicationUser)
                .Include(m => m.Penalty)
                .Include(m => m.Loans)
                .Select(m => new MemberResponse()
                {
                    MemberId = m.MemberId,
                    IdNumber = m.ApplicationUser!.IdNumber,
                    Name = m.ApplicationUser.Name,
                    LastName = m.ApplicationUser.LastName,
                    Gender = m.ApplicationUser.Gender,
                    BirthDate = m.ApplicationUser.BirthDate,
                    RegisterDate = m.ApplicationUser.RegisterDate,
                    MemberEducation = m.MemberEducation,
                    MemberStatus = m.MemberStatus,
                    PenaltyType = m.Penalty!.Select(p => p.Type).ToList()!,
                    LoanedBookNames = m.Loans!.Select(l => l.BookCopy!.Book!.Title).ToList()
                })
                .ToListAsync();

            return ServiceResult<IEnumerable<MemberResponse>>.SuccessResult(members);
        }

        /// <summary>
        /// Retrieves a specific member by their ID.
        /// </summary>
        /// <param name="id">The ID of the member.</param>
        /// <returns>The member details.</returns>
        public async Task<ServiceResult<MemberResponse>> GetMemberByIdAsync(string id)
        {
            var member = await _context.Members!
                .Include(m => m.ApplicationUser)
                .Include(m => m.Penalty)
                .Include(m => m.Loans)
                .Select(m => new MemberResponse()
                {
                    MemberId = m.MemberId,
                    IdNumber = m.ApplicationUser!.IdNumber,
                    Name = m.ApplicationUser.Name,
                    LastName = m.ApplicationUser.LastName,
                    Gender = m.ApplicationUser.Gender,
                    BirthDate = m.ApplicationUser.BirthDate,
                    RegisterDate = m.ApplicationUser.RegisterDate,
                    MemberEducation = m.MemberEducation,
                    MemberStatus = m.MemberStatus,
                    PenaltyType = m.Penalty!.Select(p => p.Type).ToList()!,
                    LoanedBookNames = m.Loans!.Select(l => l.BookCopy!.Book!.Title).ToList()
                })
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return ServiceResult<MemberResponse>.FailureResult("Member not found");
            }

            return ServiceResult<MemberResponse>.SuccessResult(member);
        }

        /// <summary>
        /// Retrieves a specific member by their ID number.
        /// </summary>
        /// <param name="idNumber">The ID number of the member.</param>
        /// <returns>The member details.</returns>
        public async Task<ServiceResult<MemberResponse>> GetMemberByIdNumberAsync(string idNumber)
        {
            var member = await _context.Members!
                .Include(m => m.ApplicationUser)
                .Include(m => m.Penalty)
                .Include(m => m.Loans)
                .Select(m => new MemberResponse()
                {
                    MemberId = m.MemberId,
                    IdNumber = m.ApplicationUser!.IdNumber,
                    Name = m.ApplicationUser.Name,
                    LastName = m.ApplicationUser.LastName,
                    Gender = m.ApplicationUser.Gender,
                    BirthDate = m.ApplicationUser.BirthDate,
                    RegisterDate = m.ApplicationUser.RegisterDate,
                    MemberEducation = m.MemberEducation,
                    MemberStatus = m.MemberStatus,
                    PenaltyType = m.Penalty!.Select(p => p.Type).ToList()!,
                    LoanedBookNames = m.Loans!.Select(l => l.BookCopy!.Book!.Title).ToList()
                })
                .FirstOrDefaultAsync(m => m.IdNumber == idNumber);

            if (member == null)
            {
                return ServiceResult<MemberResponse>.FailureResult("Member not found");
            }

            return ServiceResult<MemberResponse>.SuccessResult(member);
        }

        /// <summary>
        /// Adds a new member to the database.
        /// </summary>
        /// <param name="memberRequest">The member details.</param>
        /// <returns>A success message if the member is created successfully.</returns>
        public async Task<ServiceResult<string>> AddMemberAsync(MemberRequest memberRequest)
        {
            // Check for existing user with same email or username
            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => (u.Email == memberRequest.Email || u.UserName == memberRequest.UserName) && u.IdNumber != memberRequest.IdNumber);

            if (existingUser != null)
            {
                return ServiceResult<string>.FailureResult("A member with the same email or username already exists with a different ID number.");
            }

            // Check for existing member with same ID number
            var existingMember = await _context.Members!
                .Include(m => m.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ApplicationUser!.IdNumber == memberRequest.IdNumber);

            if (existingMember != null)
            {
                if (existingMember.MemberStatus == MemberStatus.BlockedAccount.ToString())
                {
                    return ServiceResult<string>.FailureResult("The member with this ID number is blocked and cannot create a new account.");
                }
                else if (existingMember.MemberStatus == MemberStatus.RemovedAccount.ToString())
                {
                    existingMember.MemberStatus = MemberStatus.ActiveAccount.ToString();
                    existingMember.ApplicationUser!.Email = memberRequest.Email;
                    existingMember.ApplicationUser.UserName = memberRequest.UserName;
                    existingMember.ApplicationUser.PasswordHash = _userManager.PasswordHasher.HashPassword(existingMember.ApplicationUser, memberRequest.Password);
                    _context.Update(existingMember).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return ServiceResult<string>.SuccessMessageResult("The member account is reactivated.");
                }
            }

            var user = new ApplicationUser
            {
                IdNumber = memberRequest.IdNumber,
                Name = memberRequest.Name,
                LastName = memberRequest.LastName,
                Gender = memberRequest.Gender,
                BirthDate = memberRequest.BirthDate,
                UserName = memberRequest.UserName,
                PhoneNumber = memberRequest.PhoneNumber,
                Email = memberRequest.Email,
            };

            var result = await _userManager.CreateAsync(user, memberRequest.Password);
            if (!result.Succeeded)
            {
                return ServiceResult<string>.FailureResult(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Assign "Member" role to the user
            await _userManager.AddToRoleAsync(user, "Member");

            var member = new Member
            {
                MemberId = user.Id,
                MemberEducation = memberRequest.EducationalDegree,
                MemberStatus = MemberStatus.ActiveAccount.ToString(),
            };

            await _context.Members!.AddAsync(member);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.SuccessMessageResult("Member successfully created!");
        }

        /// <summary>
        /// Updates an existing member's details.
        /// </summary>
        /// <param name="id">The ID of the member to update.</param>
        /// <param name="memberRequest">The member update details.</param>
        /// <returns>A success message if the member is updated successfully.</returns>
        public async Task<ServiceResult<bool>> UpdateMemberAsync(string id, UpdateMemberRequest updateMemberRequest)
        {
            var member = await _context.Members!
                .Include(m => m.ApplicationUser)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return ServiceResult<bool>.FailureResult("Member not found");
            }

            var user = member.ApplicationUser;
            if (user == null)
            {
                return ServiceResult<bool>.FailureResult("User not found");
            }

            // Check for existing user with same email or username
            var existingUser = await _userManager.Users
                .FirstOrDefaultAsync(u => (u.Email == updateMemberRequest.Email || u.UserName == updateMemberRequest.UserName) && u.Id != user.Id && u.IdNumber != updateMemberRequest.IdNumber);

            if (existingUser != null)
            {
                return ServiceResult<bool>.FailureResult("A member with the same email or username already exists with a different ID number.");
            }

            user.IdNumber = updateMemberRequest.IdNumber;
            user.Name = updateMemberRequest.Name;
            user.LastName = updateMemberRequest.LastName;
            user.Gender = updateMemberRequest.Gender;
            user.BirthDate = updateMemberRequest.BirthDate;
            user.UserName = updateMemberRequest.UserName;
            user.PhoneNumber = updateMemberRequest.PhoneNumber;
            user.Email = updateMemberRequest.Email;

            var userResult = await _userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                return ServiceResult<bool>.FailureResult(string.Join(", ", userResult.Errors.Select(e => e.Description)));
            }

            member.MemberEducation = updateMemberRequest.EducationalDegree;

            _context.Update(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Members!.AnyAsync(m => m.MemberId == id))
                {
                    return ServiceResult<bool>.FailureResult("Member not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Sets the status of a member.
        /// </summary>
        /// <param name="idNumber">The ID number of the member.</param>
        /// <param name="status">The new status of the member.</param>
        /// <returns>A success message if the status is updated successfully.</returns>
        public async Task<ServiceResult<bool>> SetMemberStatusAsync(string idNumber, string status)
        {
            var member = await _context.Members!.FirstOrDefaultAsync(m => m.ApplicationUser!.IdNumber == idNumber);

            if (member == null)
            {
                return ServiceResult<bool>.FailureResult("Member not found");
            }

            member.MemberStatus = status;

            _context.Update(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Members!.AnyAsync(e => e.MemberId == idNumber))
                {
                    return ServiceResult<bool>.FailureResult("Member not found");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds a new address for a member.
        /// </summary>
        /// <param name="memberAddress">The member address details.</param>
        /// <returns>A success message if the address is added successfully.</returns>
        public async Task<ServiceResult<bool>> AddMemberAddressAsync(MemberAddress memberAddress)
        {
            var member = await _context.Members!.FindAsync(memberAddress.MemberId);
            if (member == null)
            {
                return ServiceResult<bool>.FailureResult("Member not found");
            }

            await _context.MemberAddresses!.AddAsync(memberAddress);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.SuccessResult(true);
        }

        /// <summary>
        /// Updates a member's password.
        /// </summary>
        /// <param name="id">The ID of the member.</param>
        /// <param name="currentPassword">The current password of the member.</param>
        /// <param name="newPassword">The new password for the member.</param>
        /// <returns>A success message if the password is updated successfully.</returns>
        public async Task<ServiceResult<bool>> UpdateMemberPasswordAsync(string id, string currentPassword, string newPassword)
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
