using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                new Movie { ID = 1, Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) }
            );
            _kioskMovieDbSet = Util.GetQueryableMockDbSet(
                new KioskMovie { ID = 1, Kiosk = new Kiosk { ID = 1 }, Movie = new Movie { ID = 1 }, Stock = 2 }
            );
            _dataContext.Setup(x => x.Kiosks).Returns(_kioskDbSet.Object);
            _dataContext.Setup(x => x.Movies).Returns(_movieDbSet.Object);
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
        }

        [Test]
        public void SaveNoAddress()
        {
            var ex = Assert.Throws<ArgumentException>(() => _kioskService.Save(new Kiosk { ID = 1, Name = "Vons" }));
            Assert.AreEqual("Address must be set", ex.Message);
        }

        [Test]
        public void SaveInvalidAddress()
        {
            var ex = Assert.Throws<ArgumentException>(() => _kioskService.Save(new Kiosk { ID = 1, Name = "Vons", Address = new Address() }));
            Assert.AreEqual("Address must contain at least Street, City, and Country", ex.Message);
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
            throw new NotImplementedException();
        }

        [Test]
        public void GetByLocationWithMovie()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetByLocationFromCache()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Delete()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetMovies()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GetMoviesFromCache()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddMovieNew()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddMovieUpdate()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RemoveMovieFound()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RemoveMovieNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
