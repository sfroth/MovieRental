using MovieRental.Business.Transfer;
using MovieRental.Entities.Models;
using System.Collections.Generic;

namespace MovieRental.Business.Service.Interface
{
	public interface IMovieService
	{
		/// <summary>
		/// Get Movie by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Movie Get(int id);

		/// <summary>
		/// Save (create or update) movie
		/// </summary>
		/// <param name="movie"></param>
		void Save(Movie movie);

		/// <summary>
		/// Search for movies by search parameters
		/// </summary>
		/// <param name="searchParams"></param>
		/// <returns></returns>
		IEnumerable<Movie> Search(MovieSearchParams searchParams);
	}
}
