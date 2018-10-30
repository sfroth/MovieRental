using Moq;
using MovieRental.Business.Service;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities;
using MovieRental.Entities.Models;
using NUnit.Framework;
using System;
using System.Data.Entity;

namespace MovieRental.API.Tests.Business.Service
{
	[TestFixture]
	public class AccountServiceTests
	{
		private Mock<IDataContext> _dataContext;
	    private Mock<DbSet<Account>> _accountDbSet;
	    private Mock<DbSet<Movie>> _movieDbSet;
	    private Mock<DbSet<AccountMovie>> _accountMovieDbSet;
		private IAccountService _accountService;

		[SetUp]
		public void SetUp()
		{
			_dataContext = new Mock<IDataContext>();
			_accountDbSet = Util.GetQueryableMockDbSet(
				new Account { ID = 1, Username = "MalReynolds", Password = "serenity", UserRole = "Admin", Active = true },
				new Account { ID = 2, Username = "Wash", Password = "leaf", UserRole = "Admin", Active = true },
				new Account { ID = 3, Username = "River", Password = "reaver", UserRole = "User", Active = true },
				new Account { ID = 3, Username = "Jayne", Password = "cobb", UserRole = "User", Active = false }
			);
		    _movieDbSet = Util.GetQueryableMockDbSet(
		        new Movie { ID = 1, Title = "Serenity", ReleaseDate = new DateTime(2005, 9, 1) }
		    );
            _accountMovieDbSet = Util.GetQueryableMockDbSet(
		        new AccountMovie { ID = 1, Account = new Account { ID = 1 }, Movie = new Movie { ID = 1 }, RentalDate = new DateTime(2018, 10, 1)}
		    );
            _dataContext.Setup(x => x.Accounts).Returns(_accountDbSet.Object);
		    _dataContext.Setup(x => x.Movies).Returns(_movieDbSet.Object);
		    _dataContext.Setup(x => x.AccountMovies).Returns(_accountMovieDbSet.Object);
			_accountService = new AccountService(_dataContext.Object);
		}

		[Test]
		public void Deactivate()
		{
			_accountService.Deactivate(1);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
		public void GetById()
		{
			var acct = _accountService.Get(1);
			Assert.AreEqual(1, acct.ID);
		}

		[Test]
		public void GetByUsername()
		{
			var acct = _accountService.Get("River");
			Assert.AreEqual(3, acct.ID);
		}

		[Test]
		public void GetByUsernameAndPassword()
		{
			var acct = _accountService.Get("Wash", "leaf");
			Assert.AreEqual(2, acct.ID);
		}

		[Test]
		public void GetByUsernameAndPasswordBadLogin()
		{
			var acct = _accountService.Get("Wash", "wind");
			Assert.IsNull(acct);
		}

		[Test]
		public void GetByUsernameAndPasswordInactive()
		{
			var acct = _accountService.Get("Jayne", "cobb");
			Assert.IsNull(acct);
		}

		[Test]
		public void SaveNew()
		{
			_accountService.Save(new Account { Username = "Simon", Password = "Brother", UserRole = "User" });
			_accountDbSet.Verify(x => x.Add(It.IsAny<Account>()), Times.Once);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
		public void SaveUpdate()
		{
			_dataContext.Setup(c => c.GetOriginalValue(It.IsAny<Account>(), It.IsAny<string>()));
			_accountService.Save(new Account { ID = 1, Username = "MalReynolds", UserRole = "Admin" });
			_accountDbSet.Verify(x => x.Add(It.IsAny<Account>()), Times.Never);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
		public void SaveNoUsername()
		{
			var ex = Assert.Throws<ArgumentException>(() => _accountService.Save(new Account { ID = 1, Username = "", Password = "asdf", UserRole = "User" }));
			Assert.AreEqual("Username cannot be empty", ex.Message);
		    _dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

        [Test]
		public void SaveUsernameConflict()
		{
			var ex = Assert.Throws<ArgumentException>(() => _accountService.Save(new Account { Username = "River", Password = "asdf", UserRole = "User" }));
			Assert.AreEqual("Username is already taken", ex.Message);
		    _dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

        [Test]
		public void SaveNoPassword()
		{
			var ex = Assert.Throws<ArgumentException>(() => _accountService.Save(new Account { Username = "Book", Password = "", UserRole = "User" }));
			Assert.AreEqual("Password cannot be empty", ex.Message);
		    _dataContext.Verify(x => x.SaveChanges(), Times.Never);
		}

        [Test]
	    public void Rent()
        {
            _accountService.Rent(1, 1);
            _accountMovieDbSet.Verify(s => s.Add(It.IsAny<AccountMovie>()), Times.Once);
            _dataContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
	    public void RentNoAccount()
	    {
	        var ex = Assert.Throws<ArgumentException>(() => _accountService.Rent(8, 1));
	        Assert.AreEqual("Account not found", ex.Message);
            _accountMovieDbSet.Verify(s => s.Add(It.IsAny<AccountMovie>()), Times.Never);
	        _dataContext.Verify(x => x.SaveChanges(), Times.Never);
	    }

        [Test]
	    public void RentNoMovie()
	    {
	        var ex = Assert.Throws<ArgumentException>(() => _accountService.Rent(1, 6));
	        Assert.AreEqual("Movie not found", ex.Message);
	        _accountMovieDbSet.Verify(s => s.Add(It.IsAny<AccountMovie>()), Times.Never);
	        _dataContext.Verify(x => x.SaveChanges(), Times.Never);
	    }

	    [Test]
	    public void Return()
	    {
	        _accountService.Return(1, 1);
	        _dataContext.Verify(x => x.SaveChanges(), Times.Once);
	    }

	    [Test]
	    public void ReturnNoRental()
	    {
	        var ex = Assert.Throws<ArgumentException>(() => _accountService.Return(8, 1));
	        Assert.AreEqual("No active rental found", ex.Message);
	        _dataContext.Verify(x => x.SaveChanges(), Times.Never);
	    }
    }
}