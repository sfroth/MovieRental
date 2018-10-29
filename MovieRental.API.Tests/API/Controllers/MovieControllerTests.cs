using Moq;
using MovieRental.API.Controllers;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;
using NUnit.Framework;
using System.Web.Http.Results;

namespace MovieRental.API.Tests.API.Controllers
{
	[TestFixture]
	public class MovieControllerTests
	{
		private Mock<IMovieService> _movieService;
		private MovieController _movieController;

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
	}
}
