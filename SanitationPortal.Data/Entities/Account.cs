using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SanitationPortal.Data.Entities
{
	public class Account
	{

			[Key]
			public int Id { get; set; }

			[Required]
		    public int EmployeeId { get; set; }

			public string FirstName { get; set; } = string.Empty;

		    public string MiddleName { get; set; } = string.Empty;

		    public string LastName { get; set; } = string.Empty;

			public string EmailAddress { get; set; } = string.Empty;

		    public string DigestPassword { get; set; } = string.Empty;

		    public string EncodeLookUp { get; set; } = string.Empty;

			public bool IsActive { get; set; } = true;

	}
	
}

