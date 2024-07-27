using LibraryAPI.Data;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services.impl
{
    public class PenaltyService : IPenaltyService
    {
        private readonly LibraryAPIContext _context;

        public PenaltyService(LibraryAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all penalties associated with a specific member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>A list of penalty responses for the specified member.</returns>
        public async Task<ServiceResult<IEnumerable<PenaltyResponse>>> GetPenaltiesByMemberIdAsync(string memberId)
        {
            var penalties = await _context.Penalties!
                .Where(p => p.MemberId == memberId)
                .Include(p => p.Member)
                .Select(p => new PenaltyResponse()
                {
                    PenaltyId = p.PenaltyId,
                    DailyFee = p.DailyFee,
                    TotalFee = p.TotalFee,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    OverdueDays = p.OverdueDays,
                    Type = p.Type,
                    MemberName = p.Member!.ApplicationUser!.Name,
                })
                .ToListAsync();
            return ServiceResult<IEnumerable<PenaltyResponse>>.SuccessResult(penalties);
        }

        /// <summary>
        /// Retrieves a specific penalty by its ID.
        /// </summary>
        /// <param name="id">The ID of the penalty.</param>
        /// <returns>The penalty details.</returns>
        public async Task<ServiceResult<PenaltyResponse>> GetPenaltyByIdAsync(long id)
        {
            var penalty = await _context.Penalties!
                .Include(p => p.Member)
                .Select(p => new PenaltyResponse()
                {
                    PenaltyId = p.PenaltyId,
                    DailyFee = p.DailyFee,
                    TotalFee = p.TotalFee,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    OverdueDays = p.OverdueDays,
                    Type = p.Type,
                    MemberName = p.Member!.ApplicationUser!.Name,
                })
                .FirstOrDefaultAsync(p => p.PenaltyId == id);

            if (penalty == null)
            {
                return ServiceResult<PenaltyResponse>.FailureResult("Penalty not found");
            }

            return ServiceResult<PenaltyResponse>.SuccessResult(penalty);
        }

        /// <summary>
        /// Calculates penalties for overdue book returns.
        /// </summary>
        /// <param name="loanReturnRequest">The loan return request containing details of the loan return.</param>
        /// <returns>A success result if penalties are calculated successfully.</returns>
        public async Task<ServiceResult<bool>> CalculatePenaltiesAsync(LoanReturnRequest loanReturnRequest)
        {
            if (loanReturnRequest.ReturnDate > loanReturnRequest.DueDate)
            {
                int overdueDays = (loanReturnRequest.ReturnDate.Value - loanReturnRequest.DueDate).Days;
                decimal dailyFee = 0.5m;
                var penalty = new Penalty
                {
                    DailyFee = dailyFee,
                    StartDate = loanReturnRequest.DueDate,
                    EndDate = loanReturnRequest.ReturnDate,
                    OverdueDays = overdueDays,
                    MemberId = loanReturnRequest.MemberId,
                    TotalFee = dailyFee * overdueDays
                };

                penalty.CalculatePenaltyDetails();

                await _context.Penalties!.AddAsync(penalty);

                try
                {
                    await _context.SaveChangesAsync();
                    return ServiceResult<bool>.SuccessResult(true);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return ServiceResult<bool>.FailureResult("Error occurred while calculating penalties.");
                }
            }

            return ServiceResult<bool>.SuccessResult(true);
        }
    }
}
