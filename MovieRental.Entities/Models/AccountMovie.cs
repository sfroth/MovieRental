using System;
using System.ComponentModel.DataAnnotations;

namespace MovieRental.Entities.Models
{
	/// <summary>
	/// Rental association between an account and a movie
	/// </summary>
	public class AccountMovie
	{
		public int ID { get; set; }
		[Required]
		public virtual Account Account { get; set; }
		[Required]
		public virtual Movie Movie { get; set; }
		public DateTime RentalDate { get; set; }
		public DateTime? ReturnDate { get; set; }
	}
}
