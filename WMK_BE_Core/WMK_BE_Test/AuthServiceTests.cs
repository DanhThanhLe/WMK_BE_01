using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WMK_BE_BusinessLogic.BusinessModel.RequestModel.AuthModel;
using WMK_BE_BusinessLogic.BusinessModel.ResponseModel.UserModel;
using WMK_BE_BusinessLogic.Helpers;
using WMK_BE_BusinessLogic.ResponseObject;
using WMK_BE_BusinessLogic.Service.Implement;
using WMK_BE_BusinessLogic.Service.Interface;
using WMK_BE_BusinessLogic.ValidationModel;
using WMK_BE_RecipesAndPlans_DataAccess.Enums;
using WMK_BE_RecipesAndPlans_DataAccess.Models;
using WMK_BE_RecipesAndPlans_DataAccess.Repository.Interface;
using Xunit;

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
            _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object, _mapperMock.Object)
            {

            };
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                EmailOrUserName = "test@example.com",
                Password = "Strong_password@123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = HashHelper.GetSignature256("Strong_password@123"),
                Role = Role.Customer,
                EmailConfirm = EmailConfirm.Confirm,
                Status = BaseStatus.Available
            };
            var signingKey = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(signingKey);
            var base64Key = Convert.ToBase64String(signingKey);

            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync(user);

            _configurationMock.Setup(x => x["JWT:IssuerSigningKey"])
    .Returns(base64Key);

            _configurationMock.Setup(x => x["JWT:ValidIssuer"])
                .Returns("test_issuer");

            _configurationMock.Setup(x => x["JWT:ValidAudience"])
                .Returns("test_audience");

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Message);
        }

        public async Task Password_ContainsAtLeastOneUppercaseLetter()
        {
            //_loginModelValidator.ShouldHaveValidationErrorFor(x => x.Password, "password")
            //    .WithErrorMessage("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!");
            // Arrange
            var loginModel = new LoginModel
            {
                EmailOrUserName = "test@example.com",
                Password = "password"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync(It.IsAny<User?>);

            var result = await _authService.LoginAsync(loginModel);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!", result.Message);
        }

        [Fact]
        public async Task Password_ContainsAtLeastOneDigit()
        {
            //_loginModelValidator.ShouldHaveValidationErrorFor(x => x.Password, "Password")
            //    .WithErrorMessage("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!");
            var loginModel = new LoginModel
            {
                EmailOrUserName = "test@example.com",
                Password = "Password"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync(It.IsAny<User?>);

            var result = await _authService.LoginAsync(loginModel);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!", result.Message);

        }

        [Fact]
        public async Task Password_ContainsAtLeastOneSpecialCharacter()
        {
            //_loginModelValidator.ShouldHaveValidationErrorFor(x => x.Password, "Password1")
            //    .WithErrorMessage("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!");
            var loginModel = new LoginModel
            {
                EmailOrUserName = "test@example.com",
                Password = "Password1"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync(It.IsAny<User?>);

            var result = await _authService.LoginAsync(loginModel);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!", result.Message);


        }

        [Fact]
        public async Task Password_IsAtLeastSixCharactersLong()
        {
            //_loginModelValidator.ShouldHaveValidationErrorFor(x => x.Password, "P1!")
            //    .WithErrorMessage("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!");
            var loginModel = new LoginModel
            {
                EmailOrUserName = "test@example.com",
                Password = "P1!"
            };
            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync(It.IsAny<User?>);

            var result = await _authService.LoginAsync(loginModel);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Password must contain at least 1 uppercase letter, 1 digit, 1 special character, and be at least 6 characters long!", result.Message);

        }


        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ReturnsErrorMessage()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                EmailOrUserName = "test@example.com",
                Password = "Wrong_password@123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = HashHelper.GetSignature256("Password@123"),
                Role = Role.Customer,
                EmailConfirm = EmailConfirm.Confirm,
                Status = BaseStatus.Available
            };

            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Wrong password!", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistingUser_ReturnsErrorMessage()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                EmailOrUserName = "non_existing_user@example.com",
                Password = "Strong_password@123"
            };

            _userRepositoryMock.Setup(x => x.GetByEmailOrUserNameAsync(loginModel.EmailOrUserName))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("User not exist!", result.Message);
        }
    }
}
