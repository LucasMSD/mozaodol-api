using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjetoTelegram.Api.Controllers.Auth;
using ProjetoTelegram.Application.DTOs.AuthDTOs;
using ProjetoTelegram.Application.UseCases.Auth.AuthUseCases;
using System.Net;

namespace ProjetoTelegram.UnitTests.Systems.Api.Controllers
{
    public class TestAuthController
    {
        #region Signup
        [Fact]
        public async Task Signup_OnCreatedRequest_ReturnStatusCode201()
        {
            // arrange
            var mockUseCase = new Mock<IAuthSignupUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthSignupModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok().WithReason(new Success("").WithMetadata("HttpStatusCode", 201)));

            // act
            var result = await new AuthController().Signup(
                mockUseCase.Object,
                It.IsAny<AuthSignupModel>(),
                It.IsAny<CancellationToken>());
            // assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public async Task Signup_OnBadRequest_ReturnStatusCode400()
        {
            // arrange
            var mockUseCase = new Mock<IAuthSignupUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthSignupModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new Error("").WithMetadata("HttpStatusCode", 400)));
            // act
            var result = await new AuthController().Signup(
                mockUseCase.Object,
                It.IsAny<AuthSignupModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Signup_OnConflictRequest_ReturnStatusCode409()
        {
            // arrange
            var mockUseCase = new Mock<IAuthSignupUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthSignupModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new Error("").WithMetadata("HttpStatusCode", 409)));
            // act
            var result = await new AuthController().Signup(
                mockUseCase.Object,
                It.IsAny<AuthSignupModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Signup_OnUnprocessableEntityRequest_ReturnStatusCode422()
        {
            // arrange
            var mockUseCase = new Mock<IAuthSignupUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthSignupModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new Error("").WithMetadata("HttpStatusCode", 422)));
            // act
            var result = await new AuthController().Signup(
                mockUseCase.Object,
                It.IsAny<AuthSignupModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        }
        #endregion

        #region Login
        [Fact]
        public async Task Login_OnSuccessRequest_ReturnStatusCode200()
        {
            // arrange
            var mockUseCase = new Mock<IAuthLoginUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthLoginModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok().WithReason(new Success("").WithMetadata("HttpStatusCode", 200)));
            // act
            var result = await new AuthController().Login(
                mockUseCase.Object,
                It.IsAny<AuthLoginModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_OnBadRequest_ReturnStatusCode400()
        {
            // arrange
            var mockUseCase = new Mock<IAuthLoginUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthLoginModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new Error("").WithMetadata("HttpStatusCode", 400)));
            // act
            var result = await new AuthController().Login(
                mockUseCase.Object,
                It.IsAny<AuthLoginModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_OnUnauthorized_ReturnStatusCode401()
        {
            // arrange
            var mockUseCase = new Mock<IAuthLoginUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthLoginModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new Error("").WithMetadata("HttpStatusCode", 401)));
            // act
            var result = await new AuthController().Login(
                mockUseCase.Object,
                It.IsAny<AuthLoginModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_OnForbidden_ReturnForbidResult()
        {
            // arrange
            var mockUseCase = new Mock<IAuthLoginUseCase>();
            mockUseCase
                .Setup(x => x.Handle(It.IsAny<AuthLoginModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(new Error("").WithMetadata("HttpStatusCode", 403)));
            // act
            var result = await new AuthController().Login(
                mockUseCase.Object,
                It.IsAny<AuthLoginModel>(),
                It.IsAny<CancellationToken>());
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }
        #endregion
    }
}
