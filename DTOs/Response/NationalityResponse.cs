using System;
using LibraryAPI.Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.DTOs.Response
{
	public class NationalityResponse
	{
        public short NationalityId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string NationalityCode { get; set; } = string.Empty;
    }
}

