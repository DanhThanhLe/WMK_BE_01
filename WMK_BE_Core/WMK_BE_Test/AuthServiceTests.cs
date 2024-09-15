using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Cryptography;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel;
using WMK_BE_BusinessLogic.Helpers;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;

namespace WMK_BE_BusinessLogic.Tests.Service.Implement
{
	public class AuthServiceTests
	{
		private readonly AuthService _authService;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;

		public AuthServiceTests()
		{

			_configurationMock = new Mock<IConfiguration>();
			_mapperMock = new Mock<IMapper>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_unitOfWorkMock.Setup(x => x.UserRepository).Returns(_userRepositoryMock.Object);
			_authService = new AuthService(_unitOfWorkMock.Object , _configurationMock.Object , _mapperMock.Object)
			{

			};
		}

		[Theory]
		[InlineData("test@example.com" , "password" , 400 , "Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!")]
		[InlineData("test@example.com" , "Password" , 400 , "Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!")]
		[InlineData("test@example.com" , "Password1" , 400 , "Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!")]
		[InlineData("test@example.com" , "P1!" , 400 , "Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!")]
		public async Task Password_Validation(string emailOrUserName , string password , int expectedStatusCode , string expectedMessage)
		{
			var loginModel = new LoginModel
			{
				EmailOrUserName = emailOrUserName ,
				Password = password
			};

			_userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(emailOrUserName))
				.ReturnsAsync(It.IsAny<User?>);

			var result = await _authService.LoginAsync(loginModel);

			Assert.Equal(expectedStatusCode , result.StatusCode);
			Assert.Equal(expectedMessage , result.Message);
		}

		[Fact]
		public async Task LoginAsync_WithInvalidCredentials_ReturnsErrorMessage()
		{
			var loginModel = new LoginModel
			{
				EmailOrUserName = "test@example.com" ,
				Password = "Wrong_password@123"
			};

			var user = new User
			{
				Id = Guid.NewGuid() ,
				Email = "test@example.com" ,
				PasswordHash = HashHelper.GetSignature256("Password@123") ,
				Role = Role.Customer ,
				EmailConfirm = EmailConfirm.Confirm ,
				Status = BaseStatus.Available
			};

			_userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
				.ReturnsAsync(user);

			var result = await _authService.LoginAsync(loginModel);

			Assert.Equal(401 , result.StatusCode);
			Assert.Equal("Wrong password!" , result.Message);
		}

		[Fact]
		public async Task LoginAsync_WithNonExistingUser_ReturnsErrorMessage()
		{
			var loginModel = new LoginModel
			{
				EmailOrUserName = "non_existing_user@example.com" ,
				Password = "Strong_password@123"
			};

			_userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
				.ReturnsAsync((User)null);

			var result = await _authService.LoginAsync(loginModel);

			Assert.Equal(404 , result.StatusCode);
			Assert.Equal("User not exist!" , result.Message);
		}
		[Fact]
		public async Task LoginAsync_WithValidCredentials_ReturnsToken()
		{
			var loginModel = new LoginModel
			{
				EmailOrUserName = "test@example.com" ,
				Password = "Strong_password@123"
			};

			var user = new User
			{
				Id = Guid.NewGuid() ,
				Email = "test@example.com" ,
				PasswordHash = HashHelper.GetSignature256("Strong_password@123") ,
				Role = Role.Customer ,
				EmailConfirm = EmailConfirm.Confirm ,
				Status = BaseStatus.Available
			};
			var signingKey = new byte[16];
			RandomNumberGenerator.Fill(signingKey);
			var base64Key = Convert.ToBase64String(signingKey);

			_userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
				.ReturnsAsync(user);

			_configurationMock.Setup(x => x["JWT:IssuerSigningKey"])
				.Returns(base64Key);

			_configurationMock.Setup(x => x["JWT:ValidIssuer"])
				.Returns("test_issuer");

			_configurationMock.Setup(x => x["JWT:ValidAudience"])
				.Returns("test_audience");

			var result = await _authService.LoginAsync(loginModel);

			Assert.Equal(200 , result.StatusCode);
			Assert.NotNull(result.Message);
		}
	}
}
