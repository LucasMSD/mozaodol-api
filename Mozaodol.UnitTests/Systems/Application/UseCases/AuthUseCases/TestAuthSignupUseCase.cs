using FluentAssertions;
using MongoDB.Bson;
using Moq;
using Mozaodol.Application.DTOs.AuthDTOs;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Application.UseCases.Auth.AuthUseCases;
using Mozaodol.Domain.Entities.UserEntities;
using Mozaodol.Domain.Repositories.UserRepositories;
using System.Net;

namespace Mozaodol.UnitTests.Systems.Application.UseCases.AuthUseCases
{
    public class TestAuthSignupUseCase
    {
        [Fact]
        public async Task Handle_SuccessSignup_ReturnResultOk()
        {
            // arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.Exists(It.IsAny<string>()))
                .ReturnsAsync(false);
            mockUserRepository
                .Setup(x => x.Add(It.IsAny<User>()))
                .ReturnsAsync(new User
                {
                    _id = new ObjectId(),
                    Username = "test",
                    Name = "test",
                    PushToken = "test"
                });
            // act
            var result = await new AuthSignupUseCase(mockUserRepository.Object)
                .Handle(new AuthSignupModel
                {
                    Name = "teste",
                    Username = "teste",
                    Password = "dsds"
                }, It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ReturnResultConflict()
        {
            // arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(x => x.Exists(It.IsAny<string>()))
                .ReturnsAsync(true);
            mockUserRepository
                .Setup(x => x.Add(It.IsAny<User>()))
                .ReturnsAsync(new User
                {
                    _id = new ObjectId(),
                    Username = "test",
                    Name = "test",
                    PushToken = "test"
                });
            // act
            var result = await new AuthSignupUseCase(mockUserRepository.Object)
                .Handle(new AuthSignupModel
                {
                    Name = "teste",
                    Username = "teste",
                    Password = "dsds"
                }, It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNull();
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.Conflict);
        }
    }
}
