using FluentResults;
using ProjetoTelegram.Application.CrossCutting.Models;

namespace ProjetoTelegram.Application.UseCases
{
    public interface IUseCase<TInput, TResponse>
    {
        protected UserInfo User { get; set; }
        Task<Result<TResponse>> Handle(TInput input, CancellationToken cancellationToken);

        IUseCase<TInput, TResponse> SetUserInfo(UserInfo user);
    }
}
