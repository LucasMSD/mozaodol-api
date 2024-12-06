using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using ProjetoTelegram.Api.Controllers.Chat;
using ProjetoTelegram.Application.UseCases.ChatUseCases;
using System.Net;

namespace ProjetoTelegram.UnitTests.Systems.Api.Controllers
{
    public class TestChatController
    {
        #region List
        [Fact]
        public async Task List_OnSuccessRequest_ReturnStatusCode200()
        {
            // arrange
            var mockUseCase = new Mock<IListUserChatsUseCase>();
            mockUseCase
                .Setup(x => x.Handle(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // act
            var result = await new ChatController().List(
                mockUseCase.Object,
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            ((OkObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task List_OnSuccessButNoContentRequest_ReturnStatusCode204()
        {
            // arrange
            var mockUseCase = new Mock<IListUserChatsUseCase>();
            mockUseCase
                .Setup(x => x.Handle(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // act
            var result = await new ChatController().List(
                mockUseCase.Object,
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
            ((NoContentResult)result).StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
        #endregion

        #region List Messages
        [Fact]
        public async Task ListMessages_OnSuccesRequest_ReturnStatusCode200()
        {
            // arrange
            var mockUseCase = new Mock<IListChatMessagesUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // act
            var result = await new ChatController().ListMessages(
                mockUseCase.Object,
                It.IsAny<ObjectId>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            ((OkObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task ListMessages_OnNoContentRequest_ReturnStatusCode204()
        {
            // arrange
            var mockUseCase = new Mock<IListChatMessagesUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // act
            var result = await new ChatController().ListMessages(
                mockUseCase.Object,
                It.IsAny<ObjectId>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
            ((NoContentResult)result).StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task ListMessages_OnNotFoundRequest_ReturnStatusCode404()
        {
            // arrange
            var mockUseCase = new Mock<IListChatMessagesUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(""));
            // act
            var result = await new ChatController().ListMessages(
                mockUseCase.Object,
                It.IsAny<ObjectId>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
        #endregion
    }
}
