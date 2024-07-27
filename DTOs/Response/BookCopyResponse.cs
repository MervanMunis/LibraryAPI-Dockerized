using System;
namespace LibraryAPI.DTOs.Response
{
	public class BookCopyResponse
	{
        public long BookId { get; set; }

        public long BookCopyId { get; set; }

        public string ISBN { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? BookCopyStatus { get; set; }
    }
}

