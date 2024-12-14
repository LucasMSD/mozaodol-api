using FluentAssertions;
using FluentResults;
using MongoDB.Bson;
using Moq;
using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Application.UseCases.Auth.AuthUseCases;
using Mozaodol.Domain.Entities.UserEntities;
using Mozaodol.Domain.Repositories.UserRepositories;
using Mozaodol.Domain.Services;
using System.Net;

namespace Mozaodol.UnitTests.Systems.Application.UseCases.AuthUseCases
{
    public class TestAuthLoginUseCase
    {
        private Mock<IUserRepository> mockUserRepository;
        private Mock<ITokenService> mockTokenService;
        private AuthLoginUseCase authLoginUseCase;

        private User user = new()
        {
            Name = "Test",
            Username = "Test",
            _id = new ObjectId()
        };

        public TestAuthLoginUseCase()
        {
            mockUserRepository = new Mock<IUserRepository>();
            mockTokenService = new Mock<ITokenService>();

            authLoginUseCase = new AuthLoginUseCase(
                mockUserRepository.Object,
                mockTokenService.Object);
        }

        [Fact]
        public async Task Handle_SuccessfulLogin_ReturnResultOk()
        {
            // arrange
            mockUserRepository
                .Setup(x => x.GetByLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            mockTokenService
                .Setup(x => x.GenerateToken(It.IsAny<ObjectId>(), It.IsAny<string>()))
                .Returns(Result.Ok("valor do token"));
            var authLoginModel = new AuthLoginModel()
            {
                Username = "Test",
                Password = "teste"
            };
            // act
            var result = await authLoginUseCase.Handle(
                    authLoginModel,
                    It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().NotBeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Handle_WrongLoginCombination_ReturnResultUnauthorized()
        {
            // arrange
            mockUserRepository
                .Setup(x => x.GetByLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            mockTokenService
                .Setup(x => x.GenerateToken(It.IsAny<ObjectId>(), It.IsAny<string>()))
                .Returns(Result.Ok("Valor do token"));
            var authLoginModel = new AuthLoginModel()
            {
                Username = "Test",
                Password = "teste"
            };
            // act
            var result = await authLoginUseCase.Handle(
                    authLoginModel,
                    It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Handle_TokenNotGenerated_ReturnInternalError()
        {
            // arrange
            mockUserRepository
                .Setup(x => x.GetByLogin(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(user);

            mockTokenService
                .Setup(x => x.GenerateToken(It.IsAny<ObjectId>(), It.IsAny<string>()))
                .Returns(Result.Fail("sds"));
            var authLoginModel = new AuthLoginModel()
            {
                Username = "Test",
                Password = "teste"
            };
            // act
            var result = await authLoginUseCase.Handle(
                    authLoginModel,
                    It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.InternalServerError);
        }
    }
}
