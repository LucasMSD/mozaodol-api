﻿using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Mozaodol.Application.DTOs.ResultDtos;

namespace Mozaodol.Application.Extensions.Results
{
    public static class ResultExtension
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return GetActionResultFromResultMetadata(result);
        }

        private static IActionResult GetActionResultFromResultMetadata<T>(Result<T> result)
        {
            if (!result.TryGetStatusCode(out var statusCode))
                return result.IsFailed
                    ? new BadRequestObjectResult(result.ToResultDto()) :
                      new OkObjectResult(result.ToResultDto());

            if (statusCode == 204) return new NoContentResult();
            return new ObjectResult(result.IsFailed ? result.ToResultDto() : result.ToResultDto()) { StatusCode = statusCode };
        }

        public static bool TryGetStatusCode<T>(this Result<T> result, out int value)
        {
            value = -1;
            var reason = result.Reasons.LastOrDefault(x => x.HasMetadataKey("HttpStatusCode"));

            if (reason is null) return false;

            bool res = reason.Metadata.TryGetValue("HttpStatusCode", out var valueObject);

            if (!res || valueObject is null || valueObject is not int) return false;
            value = (int)valueObject;

            return true;
        }

        public static ResultDto ToResultDto<T>(this Result<T> result)
            => ResultDto.FromResult(result);

        public static Result<T> ToResult<T>(this T value, int statusCode)
            => Result.Ok(value).WithReason(new Success("").WithMetadata("HttpStatusCode", statusCode));

        public static Result<T> SetStatusCode<T>(this Result<T> result, int statusCode)
            => result.WithReason(new Success("").WithMetadata("HttpStatusCode", statusCode));

        public static Result SetStatusCode(this Result result, int statusCode)
            => result.WithReason(new Success("").WithMetadata("HttpStatusCode", statusCode));
    }
}
