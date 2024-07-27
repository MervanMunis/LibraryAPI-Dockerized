﻿using LibraryAPI.DTOs.Request;
using LibraryAPI.DTOs.Response;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using LibraryAPI.Exceptions;

namespace LibraryAPI.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<ServiceResult<IEnumerable<PublisherResponse>>> GetAllPublishersAsync();
        Task<ServiceResult<PublisherResponse>> GetPublisherByIdAsync(long id);
        Task<ServiceResult<string>> AddPublisherAsync(PublisherRequest PublisherRequest);
        Task<ServiceResult<bool>> UpdatePublisherAsync(long id, PublisherRequest PublisherRequest);
        Task<ServiceResult<bool>> SetPublisherStatusAsync(long id, string status);
        Task<ServiceResult<IEnumerable<CategoryBookResponse>>> GetBooksByPublisherIdAsync(long publisherId);
        Task<ServiceResult<PublisherAddress>> GetPublisherAddressAsync(long publisherId);
        Task<ServiceResult<string>> AddOrUpdatePublisherAddressAsync(long publisherId, PublisherAddressRequest address);
    }
}
