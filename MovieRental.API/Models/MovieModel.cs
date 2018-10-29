using System;

namespace MovieRental.API.Models
{
	public class MovieModel
	{
		public string Title { get; set; }
		public string ImdbId { get; set; }
		public DateTime ReleaseDate { get; set; }
	}
}