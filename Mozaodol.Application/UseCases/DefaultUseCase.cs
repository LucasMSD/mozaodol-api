using FluentResults;
using Mozaodol.Application.CrossCutting.Models;

namespace Mozaodol.Application.UseCases
{
    public abstract class DefaultUseCase<TInput, TResponse> : IUseCase<TInput,TResponse>
    {
        public UserInfo User { get; set; }

        public abstract Task<Result<TResponse>> Handle(TInput input, CancellationToken cancellationToken);

        public IUseCase<TInput, TResponse> SetUserInfo(UserInfo user)
        {
            User = user;
            return this;
        }
    }
}
