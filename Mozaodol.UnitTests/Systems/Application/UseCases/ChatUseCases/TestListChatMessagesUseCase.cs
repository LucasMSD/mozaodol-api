using FluentAssertions;
using MongoDB.Bson;
using Moq;
using Mozaodol.Application.DTOs.MessageDTOs;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Application.UseCases.ChatUseCases;
using Mozaodol.Domain.Entities.ChatEntities;
using Mozaodol.Domain.Entities.MessageEntities;
using Mozaodol.Domain.Entities.UserEntities;
using Mozaodol.Domain.Enums;
using Mozaodol.Domain.Repositories.ChatRepositories;
using Mozaodol.Domain.Repositories.MessageRepositories;
using Mozaodol.Domain.Repositories.UserRepositories;
using System.Net;

namespace Mozaodol.UnitTests.Systems.Application.UseCases.ChatUseCases
{
    public class TestListChatMessagesUseCase
    {

        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMessageRepository> _mockMessageRepository;

        public TestListChatMessagesUseCase()
        {
            _mockChatRepository = new Mock<IChatRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMessageRepository = new Mock<IMessageRepository>();
        }

        private (List<User>, Chat, List<Message>) SetupSuccessfulValues
        {
            get
            {
                var chatId = new ObjectId("67342c7f9f1820083f78f824");
                List<User> users =
                [
                    new ()
                {
                    _id = new ObjectId("67267b422e059ef12b4350eb"),
                    Name = "A",
                    Username = "userA",
                    ChatsIds = [chatId]
                },
                new ()
                {
                    _id = new ObjectId("6727e9c4da6823485a7cf212"),
                    Name = "B",
                    Username = "userB",
                    ChatsIds = [chatId],
                }
                ];

                var chat = new Chat()
                {
                    _id = chatId,
                    UsersIds = users.Select(x => x._id).ToList()
                };

                List<Message> messages =
                [
                    new Message(){
                    _id = new ObjectId("67581314b4502b2ca171d19f"),
                    ChatId = chatId,
                    UserId = users[0]._id,
                    Text = "Mensagem do usuário A",
                    Status = MessageStatus.Seen,
                    ExternalId = "externaIdA",
                    Timestamp = new DateTime(2024, 12, 1, 9, 5, 0),
                },
                new Message(){
                    _id = new ObjectId("6758130bb4502b2ca171d19e"),
                    ChatId = chatId,
                    UserId = users[1]._id,
                    Text = "Mensagem do usuário B",
                    Status = MessageStatus.Sent,
                    ExternalId = "externaIdB",
                    Timestamp = new DateTime(2024, 12, 1, 9, 5, 0).AddSeconds(2),
                }
                ];

                return (users, chat, messages);
            }
        }

        private MessageDto CreateExpectedMessage(Message message, User user) => new()
        {
            _id = message._id,
            ChatId = message.ChatId,
            ExternalId = message.ExternalId,
            Status = message.Status,
            Text = message.Text,
            Timestamp = message.Timestamp.ToString("HH:mm"),
            UserId = message.UserId,
            UserUsername = user.Username
        };

        [Fact]
        public async Task Handle_SuccessfulListWithContent_ReturnResultOk()
        {
            // arrange
            var (users, chat, messages) = SetupSuccessfulValues;
            _mockChatRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(chat);

            _mockUserRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync(users);

            _mockMessageRepository
                .Setup(x => x.GetByChat(It.IsAny<ObjectId>()))
                .ReturnsAsync(messages);

            // act
            var result = await new ListChatMessagesUseCase(
                _mockChatRepository.Object,
                _mockUserRepository.Object,
                _mockMessageRepository.Object)
                .Handle(chat._id, It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().NotBeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();
            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.OK);
        }


        [Fact]
        public async Task Handle_ShoudlMatchExpectedValues_ReturnResultOk()
        {
            // arrange
            var (users, chat, messages) = SetupSuccessfulValues;

            _mockChatRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(chat);

            _mockUserRepository
                .Setup(x => x.Get(It.Is<IEnumerable<ObjectId>>(y => y.SequenceEqual(chat.UsersIds))))
                .ReturnsAsync(users);

            _mockMessageRepository
                .Setup(x => x.GetByChat(It.Is<ObjectId>(y => y == chat._id)))
                .ReturnsAsync(messages);

            // act
            var result = await new ListChatMessagesUseCase(
                _mockChatRepository.Object,
                _mockUserRepository.Object,
                _mockMessageRepository.Object)
                .Handle(chat._id, It.IsAny<CancellationToken>());
            // assert

            result.ValueOrDefault.Select(x => x.Timestamp).Should().BeInDescendingOrder();
            result.ValueOrDefault.Should().NotContainNulls();
            result.ValueOrDefault.Should().HaveCount(2);

            var expectedMessageA = CreateExpectedMessage(messages[0], users[0]);
            var expectedMessageB = CreateExpectedMessage(messages[1], users[1]);

            result.ValueOrDefault.Should().ContainEquivalentOf(expectedMessageA);
            result.ValueOrDefault.Should().ContainEquivalentOf(expectedMessageB);
        }

        [Fact]
        public async Task Handle_ChatDoesntExist_ReturnResultNotFound()
        {
            // arrange
            _mockChatRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync((Chat?)null);

            // act
            var result = await new ListChatMessagesUseCase(
                _mockChatRepository.Object,
                _mockUserRepository.Object,
                _mockMessageRepository.Object)
                .Handle(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Handle_ChatUsersDontExist_ReturnInternalError()
        {
            // arrange
            _mockChatRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(new Chat());

            _mockUserRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync(new List<User>());

            // act
            var result = await new ListChatMessagesUseCase(
                _mockChatRepository.Object,
                _mockUserRepository.Object,
                _mockMessageRepository.Object)
                .Handle(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeFalse();
            result.IsFailed.Should().BeTrue();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Handle_NoMessagesInChat_ReturnOkNoContent()
        {
            // arrange
            _mockChatRepository
                .Setup(x => x.Get(It.IsAny<ObjectId>()))
                .ReturnsAsync(new Chat());

            _mockUserRepository
                .Setup(x => x.Get(It.IsAny<IEnumerable<ObjectId>>()))
                .ReturnsAsync([new User()]);

            _mockMessageRepository
                .Setup(x => x.GetByChat(It.IsAny<ObjectId>()))
                .ReturnsAsync([]);

            // act
            var result = await new ListChatMessagesUseCase(
                _mockChatRepository.Object,
                _mockUserRepository.Object,
                _mockMessageRepository.Object)
                .Handle(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.ValueOrDefault.Should().BeNullOrEmpty();
            result.IsSuccess.Should().BeTrue();
            result.IsFailed.Should().BeFalse();
            result.TryGetStatusCode(out var statusCode).Should().BeTrue();

            ((HttpStatusCode)statusCode).Should().BeDefined().And.Be(HttpStatusCode.NoContent);
        }
    }
}
