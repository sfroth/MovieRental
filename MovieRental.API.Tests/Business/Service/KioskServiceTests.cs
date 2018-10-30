using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using MovieRental.Business.Service;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities;
using MovieRental.Entities.Models;
using NUnit.Framework;

namespace MovieRental.API.Tests.Business.Service
{
    [TestFixture]
    public class KioskServiceTests
    {
        private Mock<IDataContext> _dataContext;
        private Mock<IMovieService> _movieService;
        private Mock<ICacheService> _cacheService;
        private Mock<DbSet<Kiosk>> _kioskDbSet;
        private Mock<DbSet<Movie>> _movieDbSet;
        private Mock<DbSet<KioskMovie>> _kioskMovieDbSet;
        private IKioskService _kioskService;

        [SetUp]
        public void SetUp()
        {
            _dataContext = new Mock<IDataContext>();
            _kioskDbSet = Util.GetQueryableMockDbSet(
                new Kiosk { ID = 1, Name = "Vons", Address = new Address { StreetAddress1 = "123 Main St", City = "Los Angeles", StateProvince = "California", Country = "US", PostalCode = "90028"} },
                new Kiosk { ID = 2, Name = "Ralphs", Address = new Address { StreetAddress1 = "124 Main St", City = "Los Angeles", StateProvince = "California", Country = "US", PostalCode = "90028" } }
            );
            _movieDbSet = Util.GetQueryableMockDbSet(
                new Movie { ID = 1, Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) },
				new Movie { ID = 2, Title = "Star Wars", ReleaseDate = new DateTime(1977, 6, 1) }
			);
            _kioskMovieDbSet = Util.GetQueryableMockDbSet(
                new KioskMovie { ID = 1, Kiosk = new Kiosk { ID = 1 }, Movie = new Movie { ID = 1 }, Stock = 2 }
            );
			_kioskDbSet.Object.ToList().ForEach(f => f.Address.Geocode());
			_dataContext.Setup(x => x.Kiosks).Returns(_kioskDbSet.Object);
            _dataContext.Setup(x => x.Movies).Returns(_movieDbSet.Object);
			_dataContext.Setup(x => x.KioskMovies).Returns(_kioskMovieDbSet.Object);
            _movieService = new Mock<IMovieService>();
            _cacheService = new Mock<ICacheService>();
            _kioskService = new KioskService(_dataContext.Object, _cacheService.Object, _movieService.Object);
        }

        [Test]
        public void SaveNew()
        {
            _kioskService.Save(new Kiosk { Name = "Vons", Address = new Address { StreetAddress1 = "123 Broad St", City = "Los Angeles", StateProvince = "California", Country = "US", PostalCode = "90028" } });
            _kioskDbSet.Verify(x => x.Add(It.IsAny<Kiosk>()), Times.Once);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _cacheService.Verify(s => s.Delete(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveUpdate()
        {
            _dataContext.Setup(s => s.GetOriginalValue(It.IsAny<Kiosk>(), It.IsAny<string>())).Returns(new Address { ID = 1 });
            _kioskService.Save(new Kiosk { ID = 1, Name = "Vons", Address = new Address { StreetAddress1 = "123 Broad St", City = "Los Angeles", StateProvince = "California", Country = "US", PostalCode = "90028" } });
            _kioskDbSet.Verify(x => x.Add(It.IsAny<Kiosk>()), Times.Never);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
            _cacheService.Verify(s => s.Delete(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SaveNoName()
        {
            var ex = Assert.Throws<ArgumentException>(() => _kioskService.Save(new Kiosk { ID = 1, Name = "", Address = new Address { StreetAddress1 = "123 Broad St", City = "Los Angeles", StateProvince = "California", Country = "US", PostalCode = "90028" } }));
            Assert.AreEqual("Kiosk name cannot be empty", ex.Message);
			_dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

		[Test]
        public void SaveNoAddress()
        {
            var ex = Assert.Throws<ArgumentException>(() => _kioskService.Save(new Kiosk { ID = 1, Name = "Vons" }));
            Assert.AreEqual("Address must be set", ex.Message);
			_dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

		[Test]
        public void SaveInvalidAddress()
        {
            var ex = Assert.Throws<ArgumentException>(() => _kioskService.Save(new Kiosk { ID = 1, Name = "Vons", Address = new Address() }));
            Assert.AreEqual("Address must contain at least Street, City, and Country", ex.Message);
			_dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

		[Test]
        public void GetById()
        {
            _cacheService.Setup(m => m.Get<Kiosk>(It.IsAny<string>())).Returns((Kiosk)null);
            var kiosk = _kioskService.Get(1);
            Assert.AreEqual(1, kiosk.ID);
            _cacheService.Verify(s => s.Set("kiosk_1", It.IsAny<Kiosk>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public void GetByIdFromCache()
        {
            _cacheService.Setup(m => m.Get<Kiosk>(It.IsAny<string>())).Returns(new Kiosk { ID = 1, Name = "Vons" });
            var kiosk = _kioskService.Get(1);
            Assert.AreEqual(1, kiosk.ID);
            _cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<Movie>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Test]
        public void GetByLocation()
        {
			_cacheService.Setup(s => s.Get<List<int>>(It.IsAny<string>())).Returns((List<int>)null);
			var ret = _kioskService.Get(new Address { PostalCode = "90028" }, 20);
			Assert.AreEqual(2, ret.Count());
			_cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public void GetByLocationWithMovie()
        {
			_cacheService.Setup(s => s.Get<List<int>>(It.IsAny<string>())).Returns((List<int>)null);
			var ret = _kioskService.Get(new Address { PostalCode = "90028" }, 20, 1);
			Assert.AreEqual(1, ret.Count());
			_cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
        public void GetByLocationFromCache()
        {
			_cacheService.Setup(s => s.Get<List<int>>(It.IsAny<string>())).Returns(new List<int> { 1 });
			var ret = _kioskService.Get(new Address { PostalCode = "90028" }, 20);
			Assert.AreEqual(1, ret.Count());
			_cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Never);
		}

		[Test]
        public void Delete()
        {
			_kioskService.Delete(1);
			_kioskMovieDbSet.Verify(s => s.Remove(It.IsAny<KioskMovie>()), Times.Once);
			_kioskDbSet.Verify(s => s.Remove(It.IsAny<Kiosk>()), Times.Once);
			_dataContext.Verify(s => s.SaveChanges(), Times.Once);
			_cacheService.Verify(s => s.Delete(It.IsAny<string>()), Times.Once);
		}

		[Test]
        public void GetMovies()
        {
			_cacheService.Setup(s => s.Get<List<int>>(It.IsAny<string>())).Returns((List<int>)null);
			var ret = _kioskService.GetMovies(1);
			Assert.AreEqual(1, ret.Count());
			_cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Once);
		}

		[Test]
        public void GetMoviesFromCache()
        {
			_cacheService.Setup(s => s.Get<List<int>>(It.IsAny<string>())).Returns(new List<int> { 1 });
			_movieService.Setup(s => s.Get(1)).Returns(_movieDbSet.Object.First());
			var ret = _kioskService.GetMovies(1);
			Assert.AreEqual(1, ret.Count());
			_cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Never);
		}

		[Test]
        public void AddMovieNew()
        {
			_kioskService.AddMovie(1, 2, 1);
			_kioskMovieDbSet.Verify(x => x.Add(It.IsAny<KioskMovie>()), Times.Once);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
		public void AddMovieNotFound()
		{
			var ex = Assert.Throws<ArgumentException>(() => _kioskService.AddMovie(1, 3, 1));
			Assert.AreEqual("Movie and Kiosk must exist", ex.Message);
			_dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

		[Test]
        public void AddMovieUpdate()
        {
			_kioskService.AddMovie(1, 1, 1);
			_kioskMovieDbSet.Verify(x => x.Add(It.IsAny<KioskMovie>()), Times.Never);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
        public void RemoveMovieFound()
        {
			_kioskService.RemoveMovie(1, 1, 1);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
        public void RemoveMovieNotFound()
        {
			_kioskService.RemoveMovie(1, 6, 1);
			_dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

		[Test]
		public void RemoveMovieNegative()
		{
			_kioskService.RemoveMovie(1, 1, 10);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}
	}
}
