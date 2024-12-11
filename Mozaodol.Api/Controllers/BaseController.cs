using Microsoft.AspNetCore.Mvc;
using Mozaodol.Application.Extensions.Results;
using Mozaodol.Application.UseCases;

namespace Mozaodol.Api.Controllers
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
