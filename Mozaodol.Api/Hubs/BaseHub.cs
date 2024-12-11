using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Mozaodol.Application.CrossCutting.Models;
using Mozaodol.Application.UseCases;
using System.Security.Claims;

namespace Mozaodol.Api.Hubs
{
    public abstract class BaseHub : Hub
    {
        protected UserInfo UserInfo { get; set; }

        private UserInfo GetUserInfo()
        {
            var userName = Context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            var userInfo = new UserInfo()
            {
                Id = new ObjectId(Context.UserIdentifier),
                Username = userName?.Value ?? string.Empty,
                Connection = Context.ConnectionId
            };

            return userInfo;
        }
        protected async Task RunAsync<TInput, TResponse>(
            IUseCase<TInput, TResponse> useCase,
            TInput input)
        {
            useCase.SetUserInfo(GetUserInfo());
            await useCase.Handle(input, CancellationToken.None);
        }
    }
}
