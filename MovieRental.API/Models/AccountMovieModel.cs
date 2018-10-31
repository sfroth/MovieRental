using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRental.API.Models
{
	public class AccountMovieModel
	{
		public int AccountID { get; set; }
		public int MovieID { get; set; }
		public string Username { get; set; }
		public string Title { get; set; }
		public DateTime RentalDate { get; set; }
		public DateTime? ReturnDate { get; set; }
	}
}