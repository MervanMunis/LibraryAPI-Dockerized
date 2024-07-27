using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Response
{
    public class PublisherAddressResponse
    {
        public int PublisherAddressId { get; set; }

        public string? Street { get; set; }

        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;
    }
}
