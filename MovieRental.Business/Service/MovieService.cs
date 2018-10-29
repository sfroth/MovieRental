using MovieRental.Business.Service.Interface;
using MovieRental.Business.Transfer;
using MovieRental.Entities;
using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieRental.Business.Service
{
	public class MovieService : IMovieService
	{
		private readonly IDataContext _context;
		private readonly ICacheService _cacheService;

		public MovieService(IDataContext context, ICacheService cacheService)
		{
			_context = context;
			_cacheService = cacheService;
		}

		/// <summary>
		/// Get Movie by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Movie Get(int id)
		{
			var cacheKey = $"movie_{id}";
			var movie = _cacheService.Get<Movie>(cacheKey);
			if (movie == null)
			{
				movie = _context.Movies.FirstOrDefault(m => m.ID == id);
				_cacheService.Set(cacheKey, movie, TimeSpan.FromDays(1));  // TODO: Do research to determine what the ideal value is for this TTL
			}
			return movie;
		}

		/// <summary>
		/// Save (create or update) movie
		/// </summary>
		/// <param name="movie"></param>
		public void Save(Movie movie)
		{
			// validate object
			if (string.IsNullOrWhiteSpace(movie.Title))
			{
				throw new ArgumentException("Movie Title cannot be empty");
			}
			if (movie.ReleaseDate == DateTime.MinValue)
			{
				throw new ArgumentException("Release Date must be set");
			}
			var existing = _context.Movies.FirstOrDefault(m => string.Compare(m.Title, movie.Title, true) == 0 && m.ReleaseDate == movie.ReleaseDate && m.ID != movie.ID);
			if (existing != null)
			{
				throw new ArgumentException("Movie listing already exists");
			}
			if (movie.ID == 0)
			{
				_context.Movies.Add(movie);
			}
			_context.SaveChanges();

			// Remove item from cache so it can be refreshed on next pull
			var cacheKey = $"movie_{movie.ID}";
			_cacheService.Delete(cacheKey);
		}

		/// <summary>
		/// Search for movies by search parameters
		/// </summary>
		/// <param name="searchParams"></param>
		/// <returns></returns>
		public IEnumerable<Movie> Search(MovieSearchParams searchParams)
		{
			var cacheKey = $"moviesearch_{searchParams.GetHashCode()}";
			var resultIds = _cacheService.Get<List<int>>(cacheKey);

			// Search results are saved as a list of IDs so that updates to movies to not break cache
			if (resultIds == null)
			{
				var searchedMovies = _context.Movies.Where(m => string.IsNullOrEmpty(searchParams.SearchTerm) ? true : m.Title.Contains(searchParams.SearchTerm));
				if (searchParams.ReleaseFrom.HasValue)
				{
					searchedMovies = searchedMovies.Where(m => m.ReleaseDate >= searchParams.ReleaseFrom.Value.Date);
				}
				if (searchParams.ReleaseTo.HasValue)
				{
					searchedMovies = searchedMovies.Where(m => m.ReleaseDate <= searchParams.ReleaseTo.Value.Date);
				}
				var ids = searchedMovies.Select(m => m.ID).ToList();
				_cacheService.Set(cacheKey, ids, TimeSpan.FromHours(1));  // TODO: Do research to determine what the ideal value is for this TTL
				return searchedMovies.ToList();
			}
			else
			{
				// call Get to take advantage of single-item caching
				return resultIds.Select(m => Get(m)).ToList();
			}
		}
	}
}
