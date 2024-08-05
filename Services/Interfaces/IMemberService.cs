using LibraryAPI.Exceptions;
using LibraryAPI.Models.DTOs.Request;
using LibraryAPI.Models.DTOs.Response;
using LibraryAPI.Models.Entities;

namespace LibraryAPI.Services.Interfaces
{
    public interface IMemberService
    {
        Task<ServiceResult<IEnumerable<MemberResponse>>> GetAllMembersAsync();

        Task<ServiceResult<MemberResponse>> GetMemberByIdNumberAsync(string idNumber);

        Task<ServiceResult<MemberResponse>> GetMemberByIdAsync(string id);

        Task<ServiceResult<string>> AddMemberAsync(MemberRequest memberRequest);

        Task<ServiceResult<bool>> UpdateMemberAsync(string id, UpdateMemberRequest updateMemberRequest);

        Task<ServiceResult<bool>> SetMemberStatusAsync(string idNumber, string status);

        Task<ServiceResult<bool>> AddMemberAddressAsync(MemberAddress memberAddress);

        Task<ServiceResult<bool>> UpdateMemberPasswordAsync(string id, string currentPassword, string newPassword);
    }
}
