using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjetoTelegram.Api.Controllers.User;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.UseCases.UserUseCases;
using System.Net;

namespace ProjetoTelegram.UnitTests.Systems.Api.Controllers
{
    public class TesteUserController
    {
        #region GetCurrent
        [Fact]
        public async Task GetCurrent_OnSuccess_ReturnStatusCode200()
        {
            // arrange
            var mockUseCase = new Mock<IGetCurrentUserDtoUseCase>();
            mockUseCase
                .Setup(x => x.Handle(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // act
            var result = await new UserController().GetCurrent(
                mockUseCase.Object,
                It.IsAny<CancellationToken>());
            // aserrt

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            ((OkObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetCurrent_OnNotFound_ReturnStatusCode404()
        {
            // arrange
            var mockUseCase = new Mock<IGetCurrentUserDtoUseCase>();
            mockUseCase
                .Setup(x => x.Handle(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(""));
            // act
            var result = await new UserController().GetCurrent(
                mockUseCase.Object,
                It.IsAny<CancellationToken>());
            // aserrt

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        #endregion

        #region UpdatePushToken
        [Fact]
        public async Task UpdatePushToken_OnSuccessRequest_ReturnStatusCode200()
        {
            // arrange
            var mockUseCase = new Mock<IUpdatePushTokenUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<UpdatePushTokenDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());
            // act
            var result = await new UserController().UpdatePushToken(
                mockUseCase.Object,
                It.IsAny<UpdatePushTokenDTO>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            ((OkObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdatePushToken_OnNotFound_ReturnStatusCode404()
        {
            // arrange
            var mockUseCase = new Mock<IUpdatePushTokenUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<UpdatePushTokenDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(""));
            // act
            var result = await new UserController().UpdatePushToken(
                mockUseCase.Object,
                It.IsAny<UpdatePushTokenDTO>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        #endregion
    }
}
