﻿using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.DTOs.Request
{
    public class LoanRequest
    {
        public short HowManyDays { get; set; }

        public long BookCopyId { get; set; }

        public long BookId { get; set; }

        public string? MemberIdNumber { get; set; }

        [JsonIgnore]
        public string MemberId { get; set; } = string.Empty;

        [JsonIgnore]
        public string EmployeeId { get; set; } = string.Empty;
    }
}
