using AutoMapper;
using log4net;
using MovieRental.API.Models;
using MovieRental.Business.Service.Interface;
using MovieRental.Business.Transfer;
using MovieRental.Entities.Models;
using System;
using System.Web.Http;

namespace MovieRental.API.Controllers
{
	public class MovieController : ApiController
    {
		private readonly IMovieService _movieService;
		private readonly ILog _log = LogManager.GetLogger("MovieRental");

		public MovieController(IMovieService movieService)
		{
			_movieService = movieService;
		}

		/// <summary>
		/// Search for movies by search params (simple term, or advanced search)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("movies")]
		public IHttpActionResult SearchMovies(MovieSearchParams searchParams)
		{
			//search movies by term or by more complex search params
			try
			{
				var movies = _movieService.Search(searchParams);
				return Ok(movies);
			}
			catch (Exception ex)
			{
				_log.Error($"Error searching movies: {ex}");
				return InternalServerError(new ApplicationException("Error searching movies"));
			}
		}

		/// <summary>
		/// Get Movie by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("movie/{id}")]
		public IHttpActionResult GetMovie(int id)
		{
			// get movie by id
			try
			{
				var movie = _movieService.Get(id);
				return Ok(movie);
			}
			catch (Exception ex)
			{
				_log.Error($"Error getting movie: {ex}");
				return InternalServerError(new ApplicationException("Error getting movie"));
			}
		}

		/// <summary>
		/// Update movie - admin action only
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpPut]
		[Route("movie/{id}")]
		public IHttpActionResult UpdateMovie(int id, MovieModel movie)
		{
			// update movie - verify user has rights to do so
			Movie dbMovie;
			try
			{
				dbMovie = _movieService.Get(id);

				Mapper.Map(movie, dbMovie);
				_movieService.Save(dbMovie);
			}
			catch (ArgumentException ex)
			{
				return InternalServerError(new ApplicationException(ex.Message));
			}
			catch (Exception ex)
			{
				_log.Error($"Error updating movie: {ex}");
				return InternalServerError(new ApplicationException("Error updating movie"));
			}
			//return movie information
			return Ok(dbMovie);
		}

		/// <summary>
		/// Create movie - admin action only
		/// </summary>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("movie")]
		public IHttpActionResult CreateMovie(MovieModel movie)
		{
			Movie dbMovie;
			try
			{
				dbMovie = Mapper.Map<Movie>(movie);
				_movieService.Save(dbMovie);
			}
			catch (ArgumentException ex)
			{
				return InternalServerError(new ApplicationException(ex.Message));
			}
			catch (Exception ex)
			{
				_log.Error($"Error creating movie: {ex}");
				return InternalServerError(new ApplicationException("Error creating movie"));
			}
			//return movie information
			return Ok(dbMovie);
		}
	}
}
