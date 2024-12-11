using FluentResults;

namespace Mozaodol.Application.DTOs.ResultDtos
{
    public class ResultDto
    {
        public object? Value { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorDto[]? Erros { get; set; }

        public static ResultDto FromResult<T>(Result<T> result)
        {
            return new ResultDto
            {
                Value = result.ValueOrDefault,
                Erros = result.Errors.Count != 0 ?
                    result.Errors.Select(x => new ErrorDto() { Message = x.Message }).ToArray() :
                    null,
                IsSuccess = result.IsSuccess
            };
        }
    }
}
