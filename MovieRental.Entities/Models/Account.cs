using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public string Password { get; set; }
		public AccountRole Role { get; set; }

		public string UserRole { get { return Role.ToString(); } }
	}
}
