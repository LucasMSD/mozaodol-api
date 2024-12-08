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
            var reasons = result.Reasons.FirstOrDefault(x => x.HasMetadataKey("HttpStatusCode"));

            if (reasons == null)
                // todo: pensar em retorno generico
                return new BadRequestObjectResult(result);

            return new ObjectResult(result) { StatusCode = (int)reasons.Metadata["HttpStatusCode"] };
        }
    }
}
