using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoTelegram.Infrastructure.Extensions.Results
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return GetActionResultFromResultMetadata(result);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            return GetActionResultFromResultMetadata(result);
        }

        private static IActionResult GetActionResultFromResultMetadata(IResultBase result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result);

            var error = result.Errors.FirstOrDefault(x => x.HasMetadataKey("HttpStatusCode"));

            if (error == null)
                return new BadRequestObjectResult(result);

            return new ObjectResult(result) { StatusCode = (int)error.Metadata["HttpStatusCode"] };
        }
    }
}
