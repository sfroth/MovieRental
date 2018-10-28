using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace MovieRental.Entities.Models
{
	/// <summary>
	/// User account
	/// </summary>
	public class Account
	{
		public enum AccountRole
		{
			User = 1,
			Admin = 2
		}
		public int ID { get; set; }
		[MaxLength(40)]
		[Required]
		public string Username { get; set; }
		[MaxLength(100)]
		[Required]
		[IgnoreDataMember]
		public string Password { get; set; }
		[IgnoreDataMember]
		public AccountRole Role { get; set; }
		public bool Active { get; set; }

		[NotMapped]
		public string UserRole
		{
			get { return Role.ToString(); }
			set
			{
				switch (value)
				{
					case "User":
						Role = AccountRole.User;
						break;
					case "Admin":
						Role = AccountRole.Admin;
						break;
					default:
						throw new ArgumentException("Invalid Role");
				}
			}
		}
	}
}
