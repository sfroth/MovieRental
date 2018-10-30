using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieRental.Entities;

namespace MovieRental.Business.Service
{
    public class KioskService : IKioskService
    {
        private readonly IDataContext _context;
        private readonly ICacheService _cacheService;
        private readonly IMovieService _movieService;

        public KioskService(IDataContext context, ICacheService cacheService, IMovieService movieService)
        {
            _context = context;
            _cacheService = cacheService;
            _movieService = movieService;
        }

        /// <summary>
        /// Save (create or update) kiosk
        /// </summary>
        /// <param name="kiosk"></param>
        public void Save(Kiosk kiosk)
        {
            // validate object
            if (string.IsNullOrWhiteSpace(kiosk.Name))
            {
                throw new ArgumentException("Kiosk name cannot be empty");
            }
            if (kiosk.Address == null)
            {
                throw new ArgumentException("Address must be set");
            }
            if (string.IsNullOrWhiteSpace(kiosk.Address.StreetAddress1)
                || string.IsNullOrWhiteSpace(kiosk.Address.City)
                || string.IsNullOrWhiteSpace(kiosk.Address.Country))
            {
                throw new ArgumentException("Address must contain at least Street, City, and Country");
            }
            // TODO: Compare to saved version (if any) and only geocode if necessary
            kiosk.Address.Geocode();

            if (kiosk.ID == 0)
            {
                _context.Kiosks.Add(kiosk);
            }
            else
            {
                // make sure to update address, rather than create new one and orphan old
                var orig_address = (Address)_context.GetOriginalValue(kiosk, "Address");
                kiosk.Address.ID = orig_address.ID;
            }
            _context.SaveChanges();

            // Remove item from cache so it can be refreshed on next pull
            var cacheKey = $"kiosk_{kiosk.ID}";
            _cacheService.Delete(cacheKey);
        }

        /// <summary>
        /// Get Kiosk by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Kiosk Get(int id)
        {
            var cacheKey = $"kiosk_{id}";
            var kiosk = _cacheService.Get<Kiosk>(cacheKey);
            if (kiosk == null)
            {
                kiosk = _context.Kiosks.FirstOrDefault(m => m.ID == id);
                _cacheService.Set(cacheKey, kiosk, TimeSpan.FromDays(1));  // TODO: Do research to determine what the ideal value is for this TTL
            }
            return kiosk;
        }

        /// <summary>
        /// Get kiosks by location/distance/movie
        /// </summary>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public IEnumerable<Kiosk> Get(Address location, int distance, int? movieId)
        {
            var cacheKey = $"kiosks_{location.GetHashCode()}_{distance}_{movieId}";
            var resultIds = _cacheService.Get<List<int>>(cacheKey);

            // Search results are saved as a list of IDs so that updates to kiosks do not break cache
            if (resultIds == null)
            {
                location.Geocode();
                var kiosks = _context.Kiosks.Where(m => m.Address.Location.Distance(location.Location) <= distance);
                if (movieId.HasValue)
                {
                    kiosks = kiosks.Where(k =>
                        _context.KioskMovies.Any(m => m.Movie.ID == movieId && m.Kiosk.ID == k.ID));
                }
                kiosks = kiosks.OrderBy(m => m.Address.Location.Distance(location.Location));
                var ids = kiosks.Select(m => m.ID).ToList();
                _cacheService.Set(cacheKey, ids, TimeSpan.FromHours(1));  // TODO: Do research to determine what the ideal value is for this TTL
                return kiosks.ToList();
            }
            else
            {
                // call Get to take advantage of single-item caching
                return resultIds.Select(m => Get(m)).ToList();
            }
        }

        /// <summary>
        /// Delete Kiosk by id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            // delete movies at kiosk
            var movies = _context.KioskMovies.Where(km => km.Kiosk.ID == id);
            foreach (var movie in movies)
            {
                _context.KioskMovies.Remove(movie);
            }

            // delete requires an object passed in, but only looks at the id
            _context.Kiosks.Remove(new Kiosk {ID = id});
            _context.SaveChanges();

            // Remove item from cache
            var cacheKey = $"kiosk_{id}";
            _cacheService.Delete(cacheKey);
        }

        /// <summary>
        /// Get movies at given kiosk
        /// </summary>
        /// <param name="kioskId"></param>
        /// <returns></returns>
        public IEnumerable<Movie> GetMovies(int kioskId)
        {
            var cacheKey = $"kioskmovies_{kioskId}";
            var resultIds = _cacheService.Get<List<int>>(cacheKey);

            // Search results are saved as a list of IDs so that updates to movies do not break cache
            if (resultIds == null)
            {
                var searchedMovies = _context.KioskMovies.Where(m => m.Kiosk.ID == kioskId && m.Stock > 0);
                var ids = searchedMovies.Select(m => m.Movie.ID).ToList();
                _cacheService.Set(cacheKey, ids, TimeSpan.FromHours(1));  // TODO: Do research to determine what the ideal value is for this TTL
                return searchedMovies.Select(m => m.Movie).ToList();
            }
            else
            {
                // call Get to take advantage of single-item caching
                return resultIds.Select(m => _movieService.Get(m)).ToList();
            }
        }

        /// <summary>
        /// Add movie stock to given kiosk
        /// </summary>
        /// <param name="kioskId"></param>
        /// <param name="movieId"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        public void AddMovie(int kioskId, int movieId, int stock)
        {
            var dbEntry = _context.KioskMovies.FirstOrDefault(km => km.Kiosk.ID == kioskId && km.Movie.ID == movieId);
            if (dbEntry == null)
            {
                // validate
                var movie = _context.Movies.FirstOrDefault(m => m.ID == movieId);
                var kiosk = _context.Kiosks.FirstOrDefault(k => k.ID == kioskId);
                if (movie == null || kiosk == null)
                {
                    throw new ArgumentException("Movie and Kiosk must exist");
                }
                dbEntry = new KioskMovie
                {
                    Kiosk = kiosk,
                    Movie = movie,
                    Stock = stock
                };
                _context.KioskMovies.Add(dbEntry);
            }
            else
            {
                dbEntry.Stock += stock;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Remove movie stock from given kiosk
        /// </summary>
        /// <param name="kioskId"></param>
        /// <param name="movieId"></param>
        /// <param name="stock"></param>
        /// <returns></returns>
        public void RemoveMovie(int kioskId, int movieId, int stock)
        {
            var dbEntry = _context.KioskMovies.FirstOrDefault(km => km.Kiosk.ID == kioskId && km.Movie.ID == movieId);
            if (dbEntry != null)
            {
                dbEntry.Stock -= stock;
                if (dbEntry.Stock < 0)
                {
                    dbEntry.Stock = 0;
                }
                _context.SaveChanges();
            }
        }
    }
}
