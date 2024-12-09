using FluentResults;
using ProjetoTelegram.Application.DTOs.UserDTOs;

namespace ProjetoTelegram.Application.UseCases.UserUseCases
{
    public class GetCurrentUserDtoUseCase :
        DefaultUseCase<object?, UserDto>,
        IGetCurrentUserDtoUseCase
    {
        private readonly IGetUserDtoUseByIdUseCase _getUserDtoUseByIdCase;

        public GetCurrentUserDtoUseCase(
            IGetUserDtoUseByIdUseCase getUserDtoUseByIdCase)
        {
            _getUserDtoUseByIdCase = getUserDtoUseByIdCase;
        }

        public override async Task<Result<UserDto>> Handle(object? input, CancellationToken cancellationToken)
        {
            var result = await _getUserDtoUseByIdCase.Handle(User.Id, cancellationToken);

            if (result.IsFailed) return Result.Fail(result.Errors);
            return result;
        }
    }
}
