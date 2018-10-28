using System;
using System.ComponentModel.DataAnnotations;

namespace MovieRental.Entities.Models
{
	/// <summary>
	/// Movie details
	/// </summary>
	public class Movie
	{
		public int ID { get; set; }
		[Required]
		public string Title { get; set; }
		public string ImdbId { get; set; }
		public DateTime ReleaseDate { get; set; }

		// TODO: Enhancement option: Expand movie details. Consider adding classes for Studio, Director, Actors
	}
}
