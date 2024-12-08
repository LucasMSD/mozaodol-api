using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using ProjetoTelegram.Api.Controllers;
using ProjetoTelegram.Application.CrossCutting.Models;
using System.Security.Claims;

namespace ProjetoTelegram.UnitTests.Systems.Api.Controllers
{
    public class TestBaseController
    {
        [Fact]
        public void GetUser_OnSuccess_ReturnCompleteUserInfo()
        {
            // arrange
            ObjectId userId = default;
            string username = "teste";
            var mockBaseController = new Mock<DefaultController>() { CallBase = true };

            var identity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            }, "TesteAuth");
            var principal = new ClaimsPrincipal(identity);
            mockBaseController.Object.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
            // act
            var result = mockBaseController.Object.GetUser();
            // assert

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<UserInfo>();

            result.Id.Should().Be(userId);
            result.Username.Should().Be(username);
        }

        [Fact]
        public void GetUser_OnMissingIdentities_ThrowException()
        {
            // arrange
            var mockBaseController = new Mock<DefaultController>() { CallBase = true };

            var principal = new ClaimsPrincipal();
            mockBaseController.Object.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
            // act
            // assert
            var result = mockBaseController.Object
                .Invoking(x => x.GetUser())
                .Should()
                .Throw<UnauthorizedAccessException>();
        }
    }
}
