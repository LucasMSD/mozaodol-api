using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Application.UseCases;
using ProjetoTelegram.Infrastructure.Extensions.Results;

namespace ProjetoTelegram.Api.Controllers
{
    public abstract class BaseController : Controller
    {
        [NonAction]
        public virtual async Task<IActionResult> RunAsync<TInput, TResponse>(
            IUseCase<TInput, TResponse> useCase,
            TInput input,
            CancellationToken cancellationToken)
            => (await useCase.Handle(input, cancellationToken)).ToActionResult();
    }
}
