using System;
using SanitationPortal.Data.Entities;
using System.Security.Cryptography;
using System.Text;

namespace SanitationPortal.Models.Extensions
{
	public static class AccountExtension
	{
		public static Account ToEntity(this DTOs.Account account)
		{

			return new Account
			{
				Id = account.Id,
				EmployeeId = account.EmployeeId,
				FirstName = account.FirstName,
				MiddleName = account.MiddleName,
				EmailAddress = account.Email,
				LastName = account.LastName,
				DigestPassword = HashValue(account.DigestPassword ?? ""),
				EncodeLookUp = Base64Encode(account.EncodeLookUp),
				IsActive = account.IsActive

			};

		}

		public static DTOs.Account ToModel(this Account account)
		{
			return new DTOs.Account
			{
				Id = account.Id,
				EmployeeId = account.EmployeeId,
				FirstName = account.FirstName,
				MiddleName = account.MiddleName,
				Email = account.EmailAddress,
				LastName = account.LastName,
				DigestPassword = account.DigestPassword,
				EncodeLookUp = Base64Decode(account.EncodeLookUp),
				IsActive = account.IsActive
			};
		}

		public static string HashValue(string plainText)
		{
			if (!string.IsNullOrEmpty(plainText))

			{
                using var sha1 = SHA1.Create();
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                return Convert.ToHexString(hash).ToLower();
            }

			return string.Empty;

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
			if (!string.IsNullOrEmpty(base64EncodedData))
			{
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }


			return string.Empty;
           
        }
    }
}

