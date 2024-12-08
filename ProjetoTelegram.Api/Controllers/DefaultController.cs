using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Application.CrossCutting.Models;
using ProjetoTelegram.Application.UseCases;
using ProjetoTelegram.Infrastructure.Extensions.Results;
using System.Security.Claims;

namespace ProjetoTelegram.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public abstract class DefaultController : BaseController
    {
        protected UserInfo UserInfo { get; set; }

        [NonAction]
        public virtual UserInfo GetUser()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
            var username = User.Identity?.Name ?? throw new UnauthorizedAccessException();

            return new UserInfo
            {
                Id = new ObjectId(userId.Value),
                Username = username,
            };
        }
    }
}
