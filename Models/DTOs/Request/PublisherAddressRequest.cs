namespace LibraryAPI.Models.DTOs.Request
{
    public class PublisherAddressRequest
    {
        public string? Street { get; set; }

        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;
    }
}
