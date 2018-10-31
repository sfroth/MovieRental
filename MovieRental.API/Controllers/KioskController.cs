using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using log4net;
using MovieRental.API.Models;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;

namespace MovieRental.API.Controllers
{
    public class KioskController : ApiController
    {
        private readonly IKioskService _kioskService;
        private readonly IMovieService _movieService;
        private readonly IAccountService _accountService;
        private readonly ILog _log = LogManager.GetLogger("MovieRental");

        public KioskController(IKioskService kioskService, IMovieService movieService, IAccountService accountService)
        {
            _kioskService = kioskService;
            _movieService = movieService;
            _accountService = accountService;
        }

        /// <summary>
        /// Get Kiosk by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
		[HttpGet]
		[Route("kiosk/{id}")]
		public IHttpActionResult GetKiosk(int id)
		{
			// get kiosk by id
			try
			{
				var kiosk = _kioskService.Get(id);
				return Ok(Mapper.Map<KioskModel>(kiosk));
			}
			catch (Exception ex)
			{
				_log.Error($"Error getting kiosk: {ex}");
				return InternalServerError(new ApplicationException("Error getting kiosk"));
			}
		}

		/// <summary>
		/// Get list of movies at the given kiosk
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("kiosk/{id}/movies")]
		public IHttpActionResult GetMoviesAtKiosk(int id)
		{
            // grab all movies at given kiosk
		    try
		    {
		        var movies = _kioskService.GetMovies(id);
		        return Ok(movies);
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error getting movies: {ex}");
		        return InternalServerError(new ApplicationException("Error getting movies"));
		    }
		}

        /// <summary>
        /// Get kiosks near location, optionally by movie
        /// </summary>
        /// <returns></returns>
        [Authorize]
		[HttpGet]
		[Route("kiosks")]
		public IHttpActionResult GetKiosksNear([FromUri]Address location, int distance = 10, int? movieId = null) // pass in location and distance and optionally movie
		{
            // get list of kiosks by distance from location
		    try
		    {
		        var kiosks = _kioskService.Get(location, distance, movieId).Select(Mapper.Map<KioskModel>);
		        return Ok(kiosks);
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error getting kiosks: {ex}");
		        return InternalServerError(new ApplicationException("Error getting kiosks"));
		    }
		}

        /// <summary>
        /// Update kiosk - admin action only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
		[HttpPut]
		[Route("kiosk/{id}")]
		public IHttpActionResult UpdateKiosk(int id, KioskModel kiosk)
		{
            // update kiosk - verify user has rights to do so
		    Kiosk dbKiosk;
		    try
		    {
		        dbKiosk = _kioskService.Get(id);

		        Mapper.Map(kiosk, dbKiosk);
		        _kioskService.Save(dbKiosk);
		    }
		    catch (ArgumentException ex)
		    {
		        return InternalServerError(new ApplicationException(ex.Message));
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error updating kiosk: {ex}");
		        return InternalServerError(new ApplicationException("Error updating kiosk"));
		    }
            //return kiosk information
            return Ok(Mapper.Map<KioskModel>(dbKiosk));
		}

        /// <summary>
        /// Create kiosk - admin action only
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("kiosk")]
		public IHttpActionResult CreateKiosk(KioskModel kiosk)
		{
            // create kiosk - verify user has rights to do so
		    Kiosk dbKiosk;
		    try
		    {
		        dbKiosk = Mapper.Map<Kiosk>(kiosk);
		        _kioskService.Save(dbKiosk);
		    }
		    catch (ArgumentException ex)
		    {
		        return InternalServerError(new ApplicationException(ex.Message));
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error creating kiosk: {ex}");
		        return InternalServerError(new ApplicationException("Error creating kiosk"));
		    }
			//return kiosk information
			return Ok(Mapper.Map<KioskModel>(dbKiosk));
		}

		/// <summary>
		/// Delete kiosk - admin action only
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize(Roles = "Admin")]
		[HttpDelete]
		[Route("kiosk/{id}")]
		public IHttpActionResult DeleteKiosk(int id)
		{
            // delete kiosk - verify user has rights to do so
		    Kiosk kiosk;
		    try
		    {
		        kiosk = _kioskService.Get(id);
                // deactivate account
		        _kioskService.Delete(kiosk.ID);
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error deleting kiosk: {ex}");
		        return InternalServerError(new ApplicationException("Error deleting kiosk"));
		    }
		    return Ok();
		}

        /// <summary>
        /// Add list of movie stocks to kiosk identified by id - admin action only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movies"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("kiosk/{id}/addmovies")]
		public IHttpActionResult AddMoviesToKiosk(int id, IEnumerable<MovieStockModel> movies)
		{
            // add movies to kiosk - verify user has rights to do so
		    try
		    {
                // validate - take advantage of caching to speed this up
		        var kiosk = _kioskService.Get(id);
		        if (kiosk == null)
		        {
		            throw new ArgumentException("Kiosk not found");
		        }

		        var dbMovies = movies.Select(m => _movieService.Get(m.MovieID));
                if (dbMovies.Any(m => m == null))
		        {
		            throw new ArgumentException("Invalid Movie IDs");
		        }

		        foreach (var entry in movies)
		        {
                    _kioskService.AddMovie(id, entry.MovieID, entry.Stock);
		        }
		    }
		    catch (ArgumentException ex)
		    {
		        return InternalServerError(new ApplicationException(ex.Message));
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error adding movies: {ex}");
		        return InternalServerError(new ApplicationException("Error adding movies"));
		    }
		    return Ok();
		}

        /// <summary>
        /// Remove list of movie stocks from kiosk identified by id - admin action only
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movies"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("kiosk/{id}/removemovies")]
        public IHttpActionResult RemoveMovieFromKiosk(int id, IEnumerable<MovieStockModel> movies)
        {
            // remove movies from kiosk - verify user has rights to do so
            try
            {
                // validate - take advantage of caching to speed this up
                var kiosk = _kioskService.Get(id);
                if (kiosk == null)
                {
		            throw new ArgumentException("Kiosk not found");
                }

                var dbMovies = movies.Select(m => _movieService.Get(m.MovieID));
                if (dbMovies.Any(m => m == null))
                {
                    throw new ArgumentException("Invalid Movie IDs");
                }

                foreach (var entry in movies)
                {
                    _kioskService.RemoveMovie(id, entry.MovieID, entry.Stock);
                }
            }
            catch (ArgumentException ex)
            {
                return InternalServerError(new ApplicationException(ex.Message));
            }
            catch (Exception ex)
            {
                _log.Error($"Error removing movies: {ex}");
                return InternalServerError(new ApplicationException("Error removing movies"));
            }
            return Ok();
        }

        /// <summary>
        /// Rent movie from kiosk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [Authorize]
		[HttpPost]
		[Route("kiosk/{id}/rent/{movieId}")]
		public IHttpActionResult RentMovie(int id, int movieId)
		{
            // rent movie from kiosk - verify in stock, then decrement count of movie at kiosk and create rental record
		    try
		    {
		        // validate - take advantage of caching to speed this up
		        var kiosk = _kioskService.Get(id);
		        if (kiosk == null)
		        {
		            throw new ArgumentException("Kiosk not found");
		        }

                var movie = _kioskService.GetMovies(id).FirstOrDefault(m => m.ID == movieId);
		        if (movie == null)
		        {
		            throw new ArgumentException("Movie not in stock at this kiosk");
		        }

	            _kioskService.RemoveMovie(id, movieId, 1);
		        var currentUser = _accountService.Get(User.Identity.Name);
		        _accountService.Rent(currentUser.ID, movieId);
		    }
		    catch (ArgumentException ex)
		    {
		        return InternalServerError(new ApplicationException(ex.Message));
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error renting movie: {ex}");
		        return InternalServerError(new ApplicationException("Error renting movie"));
		    }
		    return Ok();
		}

        /// <summary>
        /// Return movie to kiosk
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [Authorize]
		[HttpPost]
		[Route("kiosk/{id}/return/{movieId}")]
		public IHttpActionResult ReturnMovie(int id, int movieId)
		{
            // rent movie from kiosk - close out rental record and increment count of movie at kiosk
		    try
		    {
		        // validate - take advantage of caching to speed this up
		        var kiosk = _kioskService.Get(id);
		        if (kiosk == null)
		        {
		            throw new ArgumentException("Kiosk not found");
		        }

                var currentUser = _accountService.Get(User.Identity.Name);
		        // if there is no rental record found, this will throw an exception
                // since a rental record requires a valid movie, this essentially validates the movie id
		        _accountService.Return(currentUser.ID, movieId);

		        _kioskService.AddMovie(id, movieId, 1);
		    }
		    catch (ArgumentException ex)
		    {
		        return InternalServerError(new ApplicationException(ex.Message));
		    }
		    catch (Exception ex)
		    {
		        _log.Error($"Error returning movie: {ex}");
		        return InternalServerError(new ApplicationException("Error returning movie"));
		    }
		    return Ok();
		}
    }
}
