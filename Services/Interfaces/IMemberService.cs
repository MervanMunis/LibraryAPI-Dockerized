using LibraryAPI.DTOs;
using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;
using System.Reflection.Metadata;

namespace LibraryAPI.Services.Interfaces
{
    public interface IMemberService
    {
        Task<ServiceResult<IEnumerable<MemberResponse>>> GetAllMembersAsync();

        Task<ServiceResult<MemberResponse>> GetMemberByIdNumberAsync(string idNumber);

        Task<ServiceResult<MemberResponse>> GetMemberByIdAsync(string id);

        Task<ServiceResult<string>> AddMemberAsync(MemberRequest memberRequest);

        Task<ServiceResult<bool>> UpdateMemberAsync(string id, MemberRequest memberRequest);

        Task<ServiceResult<bool>> SetMemberStatusAsync(string idNumber, string status);

        Task<ServiceResult<bool>> AddMemberAddressAsync(MemberAddress memberAddress);

        Task<ServiceResult<bool>> UpdateMemberPasswordAsync(string id, string currentPassword, string newPassword);
    }
}
