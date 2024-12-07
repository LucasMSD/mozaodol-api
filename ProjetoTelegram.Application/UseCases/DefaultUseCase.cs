using FluentResults;
using ProjetoTelegram.Application.CrossCutting.Models;

namespace ProjetoTelegram.Application.UseCases
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
