using Moq;
using MovieRental.API.Controllers;
using MovieRental.API.Models;
using MovieRental.Business.Service.Interface;
using MovieRental.Entities.Models;
using NUnit.Framework;
using System;
using System.Security.Principal;
using System.Web.Http.Results;

namespace MovieRental.API.Tests.API.Controllers
{
	[TestFixture]
	public class AccountControllerTests
	{
		private Mock<IAccountService> _accountService;
		private AccountController _accountController;

		public void SetupFixture()
		{
		}

		[SetUp]
		public void SetUp()
		{
			AutoMapperConfig.Configure();
			_accountService = new Mock<IAccountService>();
			_accountController = new AccountController(_accountService.Object)
			{
				User = new GenericPrincipal(new GenericIdentity("Wash", "Admin"), new[] { "Admin" })
			};
		}

		[TearDown]
		public void TearDown()
		{
			AutoMapper.Mapper.Reset();
		}

		[Test]
		public void GetAccountById()
		{
			_accountService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Account { Username = "MalReynolds" });
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash", UserRole = "Admin" });
			var actionResult = _accountController.GetAccount(1);
			var conNegResult = actionResult as OkNegotiatedContentResult<Account>;
			Assert.IsNotNull(conNegResult);
			Assert.IsNotNull(conNegResult.Content);
			Assert.AreEqual("MalReynolds", conNegResult.Content.Username);
		}

		[Test]
		public void GetAccountByLogin()
		{
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash" });
			var actionResult = _accountController.GetAccount();
			var conNegResult = actionResult as OkNegotiatedContentResult<Account>;
			Assert.IsNotNull(conNegResult);
			Assert.IsNotNull(conNegResult.Content);
			Assert.AreEqual("Wash", conNegResult.Content.Username);
		}

		[Test]
		public void GetAccountFailSecurity()
		{
			_accountController.User = new GenericPrincipal(new GenericIdentity("River", "User"), new[] { "User" });
			_accountService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Account { Username = "MalReynolds" });
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "River" });
			var actionResult = _accountController.GetAccount(1);
			Assert.IsNotNull(actionResult as UnauthorizedResult);
		}

		[Test]
		public void GetAccountFailDbError()
		{
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Throws(new Exception("Some message"));
			var actionResult = _accountController.GetAccount(1);
			var conNegResult = actionResult as ExceptionResult;
			Assert.IsNotNull(conNegResult);
			Assert.AreEqual("Error retrieving account", conNegResult.Exception.Message);
		}

		[Test]
		public void UpdateAccount()
		{
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash", UserRole = "Admin" });
			var actionResult = _accountController.UpdateAccount(new AccountModel { Username = "Hoban" });
			var conNegResult = actionResult as OkNegotiatedContentResult<Account>;
			Assert.IsNotNull(conNegResult);
			Assert.IsNotNull(conNegResult.Content);
			Assert.AreEqual("Hoban", conNegResult.Content.Username);
			Assert.AreEqual(Account.AccountRole.Admin, conNegResult.Content.Role);
			_accountService.Verify(s => s.Save(It.IsAny<Account>()), Times.Once);
		}

		[Test]
		public void UpdateAccountFailSecurity()
		{
			_accountController.User = new GenericPrincipal(new GenericIdentity("River", "User"), new[] { "User" });
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "River", UserRole = "User" });
			var actionResult = _accountController.UpdateAccount(new AccountModel { Username = "River", UserRole = "Admin" });
			Assert.IsNotNull(actionResult as UnauthorizedResult);
		}

		[Test]
		public void UpdateAccountFailValidation()
		{
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash", UserRole = "Admin" });
			_accountService.Setup(m => m.Save(It.IsAny<Account>())).Throws(new ArgumentException("Message to test"));
			var actionResult = _accountController.UpdateAccount(new AccountModel { Username = "Hoban" });
			var conNegResult = actionResult as ExceptionResult;
			Assert.IsNotNull(conNegResult);
			Assert.AreEqual("Message to test", conNegResult.Exception.Message);
		}

		[Test]
		public void UpdateAccountFailDbError()
		{
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Throws(new Exception("Some message"));
			var actionResult = _accountController.UpdateAccount(new AccountModel { Username = "Hoban" });
			var conNegResult = actionResult as ExceptionResult;
			Assert.IsNotNull(conNegResult);
			Assert.AreEqual("Error updating account", conNegResult.Exception.Message);
		}

		[Test]
		public void CreateAccount()
		{
			var actionResult = _accountController.CreateAccount(new AccountModel { Username = "Hoban" });
			var conNegResult = actionResult as OkNegotiatedContentResult<Account>;
			Assert.IsNotNull(conNegResult);
			Assert.IsNotNull(conNegResult.Content);
			Assert.AreEqual("Hoban", conNegResult.Content.Username);
			Assert.AreEqual(Account.AccountRole.User, conNegResult.Content.Role);
			Assert.IsTrue(conNegResult.Content.Active);
			_accountService.Verify(s => s.Save(It.IsAny<Account>()), Times.Once);
		}

		[Test]
		public void CreateAccountFailValidation()
		{
			_accountService.Setup(m => m.Save(It.IsAny<Account>())).Throws(new ArgumentException("Message to test"));
			var actionResult = _accountController.CreateAccount(new AccountModel { Username = "Hoban" });
			var conNegResult = actionResult as ExceptionResult;
			Assert.IsNotNull(conNegResult);
			Assert.AreEqual("Message to test", conNegResult.Exception.Message);
		}

		[Test]
		public void CreateAccountFailDbError()
		{
			_accountService.Setup(m => m.Save(It.IsAny<Account>())).Throws(new Exception("Some message"));
			var actionResult = _accountController.CreateAccount(new AccountModel { Username = "Hoban" });
			var conNegResult = actionResult as ExceptionResult;
			Assert.IsNotNull(conNegResult);
			Assert.AreEqual("Error creating account", conNegResult.Exception.Message);
		}

		[Test]
		public void DeleteAccount()
		{
			_accountService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Account { Username = "MalReynolds", ID = 2 });
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "Wash", UserRole = "Admin" });
			var actionResult = _accountController.DeleteAccount(2);
			var conNegResult = actionResult as OkNegotiatedContentResult<Account>;
			Assert.IsNotNull(conNegResult);
			Assert.IsNotNull(conNegResult.Content);
			Assert.AreEqual("MalReynolds", conNegResult.Content.Username);
			Assert.IsFalse(conNegResult.Content.Active);
			_accountService.Verify(s => s.Deactivate(2), Times.Once);
		}

		[Test]
		public void DeleteAccountFailSecurity()
		{
			_accountController.User = new GenericPrincipal(new GenericIdentity("River", "User"), new[] { "User" });
			_accountService.Setup(m => m.Get(It.IsAny<string>())).Returns(new Account { Username = "River", UserRole = "User", ID = 5 });
			_accountService.Setup(m => m.Get(It.IsAny<int>())).Returns(new Account { Username = "MalReynolds", ID = 1 });
			var actionResult = _accountController.DeleteAccount(1);
			Assert.IsNotNull(actionResult as UnauthorizedResult);
			_accountService.Verify(s => s.Deactivate(It.IsAny<int>()), Times.Never);
		}

		[Test]
		public void DeleteAccountFailDbError()
		{
			_accountService.Setup(m => m.Save(It.IsAny<Account>())).Throws(new Exception("Some message"));
			var actionResult = _accountController.DeleteAccount(1);
			var conNegResult = actionResult as ExceptionResult;
			Assert.IsNotNull(conNegResult);
			Assert.AreEqual("Error deleting account", conNegResult.Exception.Message);
		}
	}
}
