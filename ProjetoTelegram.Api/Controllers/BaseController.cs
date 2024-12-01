using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Application.CrossCutting.Models;
using ProjetoTelegram.Application.UseCases;
using System.Security.Authentication;
using System.Security.Claims;

namespace ProjetoTelegram.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseController : Controller
    {
        protected UserInfo UserInfo { get; set; }

        [NonAction]
        public virtual UserInfo GetUser()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ?? throw new AuthenticationException();
            var username = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            return new UserInfo
            {
                Id = new ObjectId(userId.Value),
                Username = username?.Value ?? string.Empty,
            };
        }

        public virtual async Task<TResponse> RunAsync<TInput, TResponse>(IUseCase<TInput, TResponse> useCase, TInput input, CancellationToken cancellationToken)
        {
            useCase.User = GetUser();
            return await useCase.Handle(input, cancellationToken);
        }
    }
}
