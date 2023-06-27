using System;
using System.ComponentModel.DataAnnotations;

namespace SanitationPortal.Models.DTOs
{
	public class Account
	{
		public int Id { get; set; }

		[Required]
		public int EmployeeId { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string MiddleName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? DigestPassword { get; set; }
		public string EncodeLookUp { get; set; } = string.Empty;
		public bool IsActive { get; set; }
	}
}

