using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieRental.Entities.Models;

namespace MovieRental.Business.Service.Interface
{
    public interface IKioskService
    {
        /// <summary>
        /// Save (create or update) kiosk
        /// </summary>
        /// <param name="kiosk"></param>
        void Save(Kiosk kiosk);

        /// <summary>
        /// Get Kiosk by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Kiosk Get(int id);

        /// <summary>
        /// Get kiosks by location/distance/movie
        /// </summary>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        IEnumerable<Kiosk> Get(Address location, int distance, int? movieId = null);

        /// <summary>
        /// Delete Kiosk by id
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);

        /// <summary>
        /// Get movies at given kiosk
        /// </summary>
        /// <param name="kioskId"></param>
        /// <returns></returns>
        IEnumerable<Movie> GetMovies(int kioskId);

        /// <summary>
        /// Add movie stock to given kiosk
        /// </summary>
        /// <param name="kioskId"></param>
        /// <param name="movieId"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        void AddMovie(int kioskId, int movieId, int stock);

        /// <summary>
        /// Remove movie stock from given kiosk
        /// </summary>
        /// <param name="kioskId"></param>
        /// <param name="movieId"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        void RemoveMovie(int kioskId, int movieId, int stock);
    }
}
