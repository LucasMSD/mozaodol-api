using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Mozaodol.Application.CrossCutting.Models;
using Mozaodol.Application.UseCases;
using System.Security.Claims;

namespace Mozaodol.Api.Controllers
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

        [NonAction]
        public override async Task<IActionResult> RunAsync<TInput, TResponse>(
            IUseCase<TInput, TResponse> useCase,
            TInput input,
            CancellationToken cancellationToken)
        {
            useCase.SetUserInfo(GetUser()); 
            return await base.RunAsync(useCase, input, cancellationToken);
        }
    }
}
