﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Request
{
    public class DepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}