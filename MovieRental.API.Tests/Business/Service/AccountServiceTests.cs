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
			_dataContext.Setup(x => x.Accounts).Returns(_accountDbSet.Object);
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
			_accountService.Save(new Account { ID = 1, Username = "MalReynolds", Password = "Inara", UserRole = "Admin" });
			_accountDbSet.Verify(x => x.Add(It.IsAny<Account>()), Times.Never);
			_dataContext.Verify(x => x.SaveChanges(), Times.Once);
		}

		[Test]
		public void SaveNoUsername()
		{
			var ex = Assert.Throws<ArgumentException>(() => _accountService.Save(new Account { ID = 1, Username = "", Password = "asdf", UserRole = "User" }));
			Assert.AreEqual("Username cannot be empty", ex.Message);
		}

		[Test]
		public void SaveUsernameConflict()
		{
			var ex = Assert.Throws<ArgumentException>(() => _accountService.Save(new Account { Username = "River", Password = "asdf", UserRole = "User" }));
			Assert.AreEqual("Username is already taken", ex.Message);
		}

		[Test]
		public void SaveNoPassword()
		{
			var ex = Assert.Throws<ArgumentException>(() => _accountService.Save(new Account { Username = "Book", Password = "", UserRole = "User" }));
			Assert.AreEqual("Password cannot be empty", ex.Message);
		}
	}
}