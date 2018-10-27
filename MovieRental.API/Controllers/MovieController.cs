using MovieRental.Business.Service;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MovieRental.API.Controllers
{
    public class MovieController : ApiController
    {
		private readonly IMovieService _movieService;

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
		public IHttpActionResult SearchMovies()// SearchParms
		{
			//search movies by term or by more complex search params
			throw new NotImplementedException();
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
			var movie = _movieService.Get(id);
			return Ok(movie);
		}

		/// <summary>
		/// Update movie - admin action only
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPut]
		[Route("movie/{id}")]
		public IHttpActionResult UpdateMovie(int id)// MovieModel movie
		{
			// update movie - verify user has rights to do so

			throw new NotImplementedException();
		}

		/// <summary>
		/// Create movie - admin action only
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("movie")]
		public IHttpActionResult CreateMovie()// MovieModel movie
		{
			// create movie - verify user has rights to do so
			throw new NotImplementedException();
		}
	}
}
