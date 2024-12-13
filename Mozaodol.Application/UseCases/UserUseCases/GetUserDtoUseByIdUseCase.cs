using FluentResults;
using MongoDB.Bson;
using Mozaodol.Application.DTOs.UserDTOs;
using Mozaodol.Domain.Repositories.UserRepositories;

namespace Mozaodol.Application.UseCases.UserUseCases
{
    public class GetUserDtoUseByIdUseCase :
        DefaultUseCase<ObjectId, UserDto>,
        IGetUserDtoUseByIdUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserDtoUseByIdUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public override async Task<Result<UserDto>> Handle(ObjectId input, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(input);
            if (user == null) return Result.Fail("Usuário não existe.");

            return new UserDto
            {
                Name = user.Name,
                Username = user.Username,
                _id = user._id,
                PushToken = user.PushToken,
            };
        }
    }
}
