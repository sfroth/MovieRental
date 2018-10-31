using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using MovieRental.API.Controllers;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;
using NUnit.Framework;
using System.Web.Http.Results;
using MovieRental.API.Models;
using MovieRental.Business.Transfer;

namespace MovieRental.API.Tests.API.Controllers
{
	[TestFixture]
	public class MovieControllerTests
	{
		private Mock<IMovieService> _movieService;
		private MovieController _movieController;

        [OneTimeSetUp]
	    public void OneTime()
	    {
            // Uncomment this when running only movie unit tests, comment for running all tests
	        //AutoMapperConfig.Configure();
	    }

        [SetUp]
		public void SetUp()
		{
			_movieService = new Mock<IMovieService>();
			_movieController = new MovieController(_movieService.Object);
		}

		[Test]
		public void GetMovie()
		{
			_movieService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Movie { Title = "Serenity" });
			var actionResult = _movieController.GetMovie(1);
			var conNegResult = actionResult as OkNegotiatedContentResult<Movie>;
			Assert.IsNotNull(conNegResult);
			Assert.IsNotNull(conNegResult.Content);
			Assert.AreEqual("Serenity", conNegResult.Content.Title);
		}

	    [Test]
	    public void GetMovieFailDbError()
	    {
	        _movieService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
	        var actionResult = _movieController.GetMovie(1);
	        var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error getting movie", conNegResult.Exception.Message);
	    }

	    [Test]
	    public void SearchMovies()
	    {
	        _movieService.Setup(m => m.Search(It.IsAny<MovieSearchParams>())).Returns(new List<Movie> { new Movie { Title = "Serenity" } });
	        var actionResult = _movieController.SearchMovies(new MovieSearchParams {SearchTerm = "sere"});
	        var conNegResult = actionResult as OkNegotiatedContentResult<IEnumerable<Movie>>;
	        Assert.IsNotNull(conNegResult);
	        Assert.IsNotNull(conNegResult.Content);
	        Assert.AreEqual("Serenity", conNegResult.Content.First().Title);
	    }

	    [Test]
	    public void SearchMoviesFailDbError()
	    {
	        _movieService.Setup(m => m.Search(It.IsAny<MovieSearchParams>())).Throws(new Exception("Some message"));
	        var actionResult = _movieController.SearchMovies(new MovieSearchParams { SearchTerm = "sere" });
            var conNegResult = actionResult as ExceptionResult;
	        Assert.IsNotNull(conNegResult);
	        Assert.AreEqual("Error searching movies", conNegResult.Exception.Message);
	    }

        [Test]
        public void UpdateMovie()
        {
            _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Movie { Title = "Serenity" });
            var actionResult = _movieController.UpdateMovie(1, new MovieModel { Title = "Serenity2" });
            var conNegResult = actionResult as OkNegotiatedContentResult<Movie>;
            Assert.IsNotNull(conNegResult);
            Assert.IsNotNull(conNegResult.Content);
            Assert.AreEqual("Serenity2", conNegResult.Content.Title);
            _movieService.Verify(s => s.Save(It.IsAny<Movie>()), Times.Once);
        }

        [Test]
        public void UpdateMovieFailValidation()
        {
            _movieService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Movie { Title = "Serenity" });
            _movieService.Setup(m => m.Save(It.IsAny<Movie>())).Throws(new ArgumentException("Message to test"));
            var actionResult = _movieController.UpdateMovie(1, new MovieModel { Title = "Serenity2" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Message to test", conNegResult.Exception.Message);
        }

        [Test]
        public void UpdateMovieFailDbError()
        {
            _movieService.Setup(m => m.Get(It.IsAny<int>())).Throws(new Exception("Some message"));
            var actionResult = _movieController.UpdateMovie(1, new MovieModel { Title = "Serenity" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Error updating movie", conNegResult.Exception.Message);
        }

        [Test]
        public void CreateMovie()
        {
            var actionResult = _movieController.CreateMovie(new MovieModel { Title = "Serenity" });
            var conNegResult = actionResult as OkNegotiatedContentResult<Movie>;
            Assert.IsNotNull(conNegResult);
            Assert.IsNotNull(conNegResult.Content);
            Assert.AreEqual("Serenity", conNegResult.Content.Title);
            _movieService.Verify(s => s.Save(It.IsAny<Movie>()), Times.Once);
        }

        [Test]
        public void CreateMovieFailValidation()
        {
            _movieService.Setup(m => m.Save(It.IsAny<Movie>())).Throws(new ArgumentException("Message to test"));
            var actionResult = _movieController.CreateMovie(new MovieModel { Title = "Serenity" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Message to test", conNegResult.Exception.Message);
        }

        [Test]
        public void CreateMovieFailDbError()
        {
            _movieService.Setup(m => m.Save(It.IsAny<Movie>())).Throws(new Exception("Some message"));
            var actionResult = _movieController.CreateMovie(new MovieModel { Title = "Serenity" });
            var conNegResult = actionResult as ExceptionResult;
            Assert.IsNotNull(conNegResult);
            Assert.AreEqual("Error creating movie", conNegResult.Exception.Message);
        }
    }
}
