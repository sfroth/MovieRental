using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Moq;
using MovieRental.API.Controllers;
using MovieRental.API.Models;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;
using NUnit.Framework;

namespace MovieRental.API.Tests.API.Controllers
{
    [TestFixture]
	public class KioskControllerTests
	{
	    private Mock<IKioskService> _kioskService;
	    private Mock<IMovieService> _movieService;
	    private Mock<IAccountService> _accountService;
	    private KioskController _kioskController;

	    [OneTimeSetUp]
	    public void OneTime()
	    {
	        AutoMapperConfig.Configure();
	    }

	    [SetUp]
	    public void SetUp()
	    {
	        _kioskService = new Mock<IKioskService>();
	        _movieService = new Mock<IMovieService>();
	        _accountService = new Mock<IAccountService>();
	        _kioskController = new KioskController(_kioskService.Object, _movieService.Object, _accountService.Object)
	        {
	            User = new GenericPrincipal(new GenericIdentity("Wash", "Admin"), new[] { "Admin" })
	        };
        }

        [Test]
	    public void GetKiosk()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        var actionResult = _kioskController.GetKiosk(1);
	        var conNegResult = actionResult as OkNegotiatedContentResult<KioskModel>;
	        Assert.IsNotNull(conNegResult);
	        Assert.IsNotNull(conNegResult.Content);
	        Assert.AreEqual("Store", conNegResult.Content.Name);
	    }

	    [Test]
	    public void GetKioskFailDbError()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
	        var actionResult = _kioskController.GetKiosk(1);
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error getting kiosk", conNegResult.Exception.Message);
	    }

	    [Test]
	    public void GetMoviesAtKiosk()
	    {
	        _kioskService.Setup(m => m.GetMovies(It.IsAny<int>())).Returns(new List<Movie> { new Movie { Title = "Serenity" } });
	        var actionResult = _kioskController.GetMoviesAtKiosk(1);
	        var conNegResult = actionResult as OkNegotiatedContentResult<IEnumerable<Movie>>;
	        Assert.IsNotNull(conNegResult);
	        Assert.IsNotNull(conNegResult.Content);
	        Assert.AreEqual("Serenity", conNegResult.Content.First().Title);
	    }

	    [Test]
	    public void GetMoviesAtKioskFailDbError()
	    {
	        _kioskService.Setup(m => m.GetMovies(It.IsAny<int>())).Throws(new Exception("Some message"));
	        var actionResult = _kioskController.GetMoviesAtKiosk(1);
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error getting movies", conNegResult.Exception.Message);
	    }

	    [Test]
	    public void GetKiosksNear()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<Address>(), It.IsAny<int>(), It.IsAny<int?>())).Returns(new List<Kiosk> { new Kiosk { Name = "Store" } });
	        var actionResult = _kioskController.GetKiosksNear(new Address { PostalCode = "91701" }, 10);
	        var conNegResult = actionResult as OkNegotiatedContentResult<IEnumerable<KioskModel>>;
	        Assert.IsNotNull(conNegResult);
	        Assert.IsNotNull(conNegResult.Content);
	        Assert.AreEqual("Store", conNegResult.Content.First().Name);
	    }

	    [Test]
	    public void GetKiosksNearFailDbError()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<Address>(), It.IsAny<int>(), It.IsAny<int?>())).Throws(new Exception("Some message"));
	        var actionResult = _kioskController.GetKiosksNear(new Address { PostalCode = "91701" }, 10);
            var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error getting kiosks", conNegResult.Exception.Message);
	    }

        [Test]
        public void UpdateKiosk()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
            var actionResult = _kioskController.UpdateKiosk(1, new KioskModel { Name = "Store2" });
            var conNegResult = actionResult as OkNegotiatedContentResult<KioskModel>;
            Assert.IsNotNull(conNegResult);
            Assert.IsNotNull(conNegResult.Content);
            Assert.AreEqual("Store2", conNegResult.Content.Name);
            _kioskService.Verify(s => s.Save(It.IsAny<Kiosk>()), Times.Once);
        }

        [Test]
        public void UpdateKioskFailValidation()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
            _kioskService.Setup(m => m.Save(It.IsAny<Kiosk>())).Throws(new ArgumentException("Message to test"));
            var actionResult = _kioskController.UpdateKiosk(1, new KioskModel { Name = "Store2" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Message to test", conNegResult.Exception.Message);
        }

        [Test]
        public void UpdateKioskFailDbError()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
            var actionResult = _kioskController.UpdateKiosk(1, new KioskModel { Name = "Store" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Error updating kiosk", conNegResult.Exception.Message);
        }

        [Test]
        public void CreateKiosk()
        {
            var actionResult = _kioskController.CreateKiosk(new KioskModel { Name = "Store" });
            var conNegResult = actionResult as OkNegotiatedContentResult<KioskModel>;
            Assert.IsNotNull(conNegResult);
            Assert.IsNotNull(conNegResult.Content);
            Assert.AreEqual("Store", conNegResult.Content.Name);
            _kioskService.Verify(s => s.Save(It.IsAny<Kiosk>()), Times.Once);
        }

        [Test]
        public void CreateKioskFailValidation()
        {
            _kioskService.Setup(m => m.Save(It.IsAny<Kiosk>())).Throws(new ArgumentException("Message to test"));
            var actionResult = _kioskController.CreateKiosk(new KioskModel { Name = "Store" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Message to test", conNegResult.Exception.Message);
        }

        [Test]
        public void CreateKioskFailDbError()
        {
            _kioskService.Setup(m => m.Save(It.IsAny<Kiosk>())).Throws(new Exception("Some message"));
            var actionResult = _kioskController.CreateKiosk(new KioskModel { Name = "Store" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Error creating kiosk", conNegResult.Exception.Message);
        }

	    [Test]
	    public void DeleteKiosk()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        var actionResult = _kioskController.DeleteKiosk(1);
	        var conNegResult = actionResult as OkResult;
	        Assert.IsNotNull(conNegResult);
	        _kioskService.Verify(s => s.Delete(It.IsAny<int>()), Times.Once);
	    }

	    [Test]
	    public void DeleteKioskFailDbError()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        _kioskService.Setup(m => m.Delete(It.IsAny<int>())).Throws(new Exception("Some message"));
	        var actionResult = _kioskController.DeleteKiosk(1);
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error deleting kiosk", conNegResult.Exception.Message);
	    }

	    [Test]
	    public void AddMoviesToKiosk()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Movie { Title = "Serenity" });
	        var actionResult = _kioskController.AddMoviesToKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as OkResult;
	        Assert.IsNotNull(conNegResult);
	        _kioskService.Verify(s => s.AddMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
	    }

	    [Test]
	    public void AddMoviesToKioskFailValidationKiosk()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns((Kiosk)null);
	        var actionResult = _kioskController.AddMoviesToKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Kiosk not found", conNegResult.Exception.Message);
	        _kioskService.Verify(s => s.AddMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
	    }

	    [Test]
	    public void AddMoviesToKioskFailValidationMovie()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns((Movie)null);
	        var actionResult = _kioskController.AddMoviesToKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Invalid Movie IDs", conNegResult.Exception.Message);
	        _kioskService.Verify(s => s.AddMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
	    }

        [Test]
	    public void AddMoviesToKioskFailDbError()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
	        var actionResult = _kioskController.AddMoviesToKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
            var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error adding movies", conNegResult.Exception.Message);
	    }

	    [Test]
	    public void RemoveMovieFromKiosk()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Movie { Title = "Serenity" });
	        var actionResult = _kioskController.RemoveMovieFromKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as OkResult;
	        Assert.IsNotNull(conNegResult);
	        _kioskService.Verify(s => s.RemoveMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
	    }

        [Test]
	    public void RemoveMovieFromKioskFailValidationKiosk()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns((Kiosk)null);
	        var actionResult = _kioskController.RemoveMovieFromKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Kiosk not found", conNegResult.Exception.Message);
	        _kioskService.Verify(s => s.RemoveMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
	    }

	    [Test]
	    public void RemoveMovieFromKioskFailValidationMovie()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
	        _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns((Movie)null);
	        var actionResult = _kioskController.RemoveMovieFromKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Invalid Movie IDs", conNegResult.Exception.Message);
	        _kioskService.Verify(s => s.RemoveMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
	    }

	    [Test]
	    public void RemoveMovieFromKioskFailDbError()
	    {
	        _kioskService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
	        var actionResult = _kioskController.RemoveMovieFromKiosk(1, new List<MovieStockModel> { new MovieStockModel { MovieID = 1, Stock = 2 } });
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error removing movies", conNegResult.Exception.Message);
	    }

        [Test]
        public void RentMovie()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
            _kioskService.Setup(m => m.GetMovies(It.IsAny<int>())).Returns(new List<Movie> { new Movie { ID = 1, Title = "Serenity" }});
            _accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash" });
            var actionResult = _kioskController.RentMovie(1, 1);
            var conNegResult = actionResult as OkResult;
            Assert.IsNotNull(conNegResult);
            _kioskService.Verify(s => s.RemoveMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _accountService.Verify(s => s.Rent(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void RentMovieFailValidationKiosk()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns((Kiosk)null);
            var actionResult = _kioskController.RentMovie(1, 1);
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Kiosk not found", conNegResult.Exception.Message);
            _kioskService.Verify(s => s.RemoveMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void RentMovieFailValidationMovie()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
            _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns((Movie)null);
            var actionResult = _kioskController.RentMovie(1, 1);
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Movie not in stock at this kiosk", conNegResult.Exception.Message);
            _kioskService.Verify(s => s.RemoveMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void RentMovieFailDbError()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
            var actionResult = _kioskController.RentMovie(1, 1);
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Error renting movie", conNegResult.Exception.Message);
        }

        [Test]
        public void ReturnMovie()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Kiosk { Name = "Store" });
            _kioskService.Setup(m => m.GetMovies(It.IsAny<int>())).Returns(new List<Movie> { new Movie { ID = 1, Title = "Serenity" } });
            _accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash" });
            var actionResult = _kioskController.ReturnMovie(1, 1);
            var conNegResult = actionResult as OkResult;
            Assert.IsNotNull(conNegResult);
            _kioskService.Verify(s => s.AddMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _accountService.Verify(s => s.Return(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ReturnMovieFailValidationKiosk()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Returns((Kiosk)null);
            var actionResult = _kioskController.ReturnMovie(1, 1);
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Kiosk not found", conNegResult.Exception.Message);
            _kioskService.Verify(s => s.AddMovie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void ReturnMovieFailDbError()
        {
            _kioskService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
            var actionResult = _kioskController.ReturnMovie(1, 1);
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Error returning movie", conNegResult.Exception.Message);
        }
    }
}
