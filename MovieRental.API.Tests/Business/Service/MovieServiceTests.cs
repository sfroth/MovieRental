using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using MovieRental.Business.Service;
using MovieRental.Business.Service.Interface;
using MovieRental.Business.Transfer;
using MovieRental.Entities;
using MovieRental.Entities.Models;
using NUnit.Framework;

namespace MovieRental.API.Tests.Business.Service
{
    [TestFixture]
	public class MovieServiceTests
	{
	    private Mock<IDataContext> _dataContext;
	    private Mock<DbSet<Movie>> _movieDbSet;
	    private Mock<ICacheService> _cacheService;
	    private IMovieService _movieService;

        [SetUp]
	    public void SetUp()
	    {
	        _dataContext = new Mock<IDataContext>();
	        _movieDbSet = Util.GetQueryableMockDbSet(
	            new Movie { ID = 1, Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) },
	            new Movie { ID = 2, Title = "Star Wars", ReleaseDate = new DateTime(1977, 6, 1) },
	            new Movie { ID = 3, Title = "Wreck It Ralph", ReleaseDate = new DateTime(2010, 1, 1) },
	            new Movie { ID = 3, Title = "The Little Mermaid", ReleaseDate = new DateTime(1988, 1, 1) }
	        );
	        _dataContext.Setup(x => x.Movies).Returns(_movieDbSet.Object);
	        _cacheService = new Mock<ICacheService>();
	        _movieService = new MovieService(_dataContext.Object, _cacheService.Object);
	    }

	    [Test]
	    public void GetById()
	    {
	        _cacheService.Setup(m => m.Get<Movie>(It.IsAny<string>())).Returns((Movie)null);
	        var movie = _movieService.Get(1);
	        Assert.AreEqual(1, movie.ID);
            _cacheService.Verify(s => s.Set("movie_1", It.IsAny<Movie>(), It.IsAny<TimeSpan>()), Times.Once);
	    }

        [Test]
	    public void GetByIdFromCache()
	    {
	        _cacheService.Setup(m => m.Get<Movie>(It.IsAny<string>())).Returns(new Movie { ID = 1, Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) });
	        var movie = _movieService.Get(1);
	        Assert.AreEqual(1, movie.ID);
	        _cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<Movie>(), It.IsAny<TimeSpan>()), Times.Never);
	    }

        [Test]
        public void SaveNew()
	    {
	        _movieService.Save(new Movie { Title = "Friday the 13th", ReleaseDate = new DateTime(1975, 1, 1) });
	        _movieDbSet.Verify(x => x.Add(It.IsAny<Movie>()), Times.Once);
	        _dataContext.Verify(x => x.SaveChanges(), Times.Once);
	        _cacheService.Verify(s => s.Delete(It.IsAny<string>()), Times.Once);
	    }

        [Test]
	    public void SaveUpdate()
	    {
	        _movieService.Save(new Movie { ID = 1, Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) });
	        _movieDbSet.Verify(x => x.Add(It.IsAny<Movie>()), Times.Never);
	        _dataContext.Verify(x => x.SaveChanges(), Times.Once);
	        _cacheService.Verify(s => s.Delete(It.IsAny<string>()), Times.Once);
	    }

        [Test]
	    public void SaveNoTitle()
	    {
	        var ex = Assert.Throws<ArgumentException>(() => _movieService.Save(new Movie { ID = 1, Title = "", ReleaseDate = new DateTime(2005, 9, 1) }));
	        Assert.AreEqual("Movie Title cannot be empty", ex.Message);
	    }

        [Test]
	    public void SaveNoReleaseDate()
	    {
	        var ex = Assert.Throws<ArgumentException>(() => _movieService.Save(new Movie { ID = 1, Title = "Serenity" }));
	        Assert.AreEqual("Release Date must be set", ex.Message);
	    }

        [Test]
	    public void SaveConflict()
	    {
	        var ex = Assert.Throws<ArgumentException>(() => _movieService.Save(new Movie { Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) }));
	        Assert.AreEqual("Movie listing already exists", ex.Message);
	    }

        [Test]
	    public void SearchByTerm()
	    {
	        _cacheService.Setup(m => m.Get<List<int>>(It.IsAny<string>())).Returns((List<int>)null);
	        var results = _movieService.Search(new MovieSearchParams { SearchTerm = "it" });
            Assert.AreEqual(3, results.Count());
            _cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Once);
	    }

        [Test]
	    public void SearchByDate()
	    {
	        _cacheService.Setup(m => m.Get<List<int>>(It.IsAny<string>())).Returns((List<int>)null);
	        var results = _movieService.Search(new MovieSearchParams { ReleaseFrom = new DateTime(2000, 1, 1), ReleaseTo = new DateTime(2015, 1, 1)});
	        Assert.AreEqual(2, results.Count());
	        _cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Once);
	    }

        [Test]
	    public void SearchCached()
	    {
	        _cacheService.Setup(m => m.Get<List<int>>(It.IsAny<string>())).Returns(new List<int> {1,2});
	        var results = _movieService.Search(new MovieSearchParams());
	        Assert.AreEqual(2, results.Count());
	        _cacheService.Verify(s => s.Set(It.IsAny<string>(), It.IsAny<List<int>>(), It.IsAny<TimeSpan>()), Times.Never);
	    }
    }
}
