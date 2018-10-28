using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MovieRental.API.Controllers
{
    public class KioskController : ApiController
    {
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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get kiosks near location, optionally by movie
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpGet]
		[Route("kiosks")]
		public IHttpActionResult GetKiosksNear() // pass in location and optionally movie
		{
			// grab all movies at given kiosk
			throw new NotImplementedException();
		}

		/// <summary>
		/// Update kiosk - admin action only
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPut]
		[Route("kiosk/{id}")]
		public IHttpActionResult UpdateKiosk(int id)// Kioskodel kiosk
		{
			// update kiosk - verify user has rights to do so

			throw new NotImplementedException();
		}

		/// <summary>
		/// Create kiosk - admin action only
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("kiosk")]
		public IHttpActionResult CreateKiosk()// KioskModel kiosk
		{
			// create kiosk - verify user has rights to do so
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete kiosk - admin action only
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpDelete]
		[Route("kiosk/{id}")]
		public IHttpActionResult DeleteKiosk(int id)
		{
			// delete kiosk - verify user has rights to do so

			throw new NotImplementedException();
		}

		/// <summary>
		/// Add list of movie ids to kiosk identified by id - admin action only
		/// </summary>
		/// <param name="id"></param>
		/// <param name="movieIds"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[Route("kiosk/{id}/addmovies")]
		public IHttpActionResult AddMoviesToKiosk(int id, IEnumerable<int> movieIds) // list of movie ids
		{
			// add movies to kiosk - verify user has rights to do so
			throw new NotImplementedException();
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
			// rent movie from kiosk - verify in stock, then decrement count of movie at kiosk
			throw new NotImplementedException();
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
			// rent movie from kiosk - increment count of movie at kiosk
			throw new NotImplementedException();
		}
	}
}
