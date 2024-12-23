using FluentAssertions;
using MongoDB.Bson;
using Moq;
using Mozaodol.Application.CrossCutting.Models;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Application.UseCases.ChatUseCases;
using Mozaodol.Domain.Entities.ChatEntities;
using Mozaodol.Domain.Entities.UserEntities;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;
using System.Net;

namespace Mozaodol.UnitTests.Systems.Application.UseCases.ChatUseCases
{
    public class TestListUserChatsUseCase
    {
        private readonly ListUserChatsUseCase _controller;
        private readonly Mock<IChatRepository> _chatRepository;
        private readonly Mock<IUserRepository> _userRepository;


        public TestListUserChatsUseCase()
        {
            _chatRepository = new Mock<IChatRepository>();
            _userRepository = new Mock<IUserRepository>();
            _controller = new ListUserChatsUseCase(
                _userRepository.Object,
                _chatRepository.Object);
            _controller.SetUserInfo(new UserInfo
            {
                Id = new ObjectId(),
                Username = "Teste"
            });
        }

        [Fact]
        public async Task Handle_SuccessfulRequest_ReturnStatusOk()
        {
            // arrange

            _chatRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync([new Chat {
                    _id = new ObjectId(),
                    UsersIds = [new ObjectId(), new ObjectId()]
                }]);

            _userRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync([new User {
                    _id = new ObjectId(),
                    Username = "teste",
                    Name = "Test"
                }]);

            _userRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(new User
                {
                    _id = new ObjectId(),
                    Username = "teste",
                    Name = "Test"
                });
            // act
            var result = await _controller.Handle(
                null,
                new CancellationToken());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().NotBeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();
            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Handle_UserNotExists_ReturnUnauthorized()
        {
            // arrange
            _userRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync((User?)null);
            // act
            var result = await _controller.Handle(
                null,
                new CancellationToken());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();
            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Handle_NoChats_ReturnUnauthorized()
        {
            // arrange

            _chatRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync([]);

            _userRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(new User
                {
                    _id = new ObjectId(),
                    Username = "teste",
                    Name = "Test"
                });
            // act
            var result = await _controller.Handle(
                null,
                new CancellationToken());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();
            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Handle_NoUsersInChat_ReturnInternalError()
        {
            // arrange

            _chatRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync([new Chat {
                    _id = new ObjectId(),
                    UsersIds = [new ObjectId(), new ObjectId()]
                }]);

            _userRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync([]);

            _userRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(new User
                {
                    _id = new ObjectId(),
                    Username = "teste",
                    Name = "Test"
                });
            // act
            var result = await _controller.Handle(
                null,
                new CancellationToken());
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
