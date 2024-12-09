using Microsoft.AspNetCore.Mvc;
using ProjetoTelegram.Application.Extensions.Results;
using ProjetoTelegram.Application.UseCases;

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
