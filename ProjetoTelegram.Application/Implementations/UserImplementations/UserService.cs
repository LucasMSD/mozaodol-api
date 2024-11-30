using FluentResults;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ContactDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using ProjetoTelegram.Domain.Repositories.UserRepositories;
using System.Text.Json;

namespace ProjetoTelegram.Application.Implementations.UserImplementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _distributedCache;

        public UserService(IUserRepository userRepository, IDistributedCache distributedCache)
        {
            _userRepository = userRepository;
            _distributedCache = distributedCache;
        }

        public async Task<Result<List<UserDto>>> AddContact(ObjectId userId, AddContactModel addContact)
        {
            var getUserResult = await _userRepository.Get(userId);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário.").WithErrors(getUserResult.Errors);
            if (getUserResult.Value == null) return Result.Fail("Usuário não encontrado");

            getUserResult.Value.Contacts = getUserResult.Value.Contacts == null ? new List<ObjectId>() : getUserResult.Value.Contacts;
            getUserResult.Value.Contacts.Add(addContact._id);
            var updateResult = await _userRepository.UpdateContacts(getUserResult.Value._id, getUserResult.Value.Contacts);
            if (updateResult.IsFailed) return Result.Fail("Erro ao adicionar o contato.").WithErrors(updateResult.Errors);

            return await GetContacts(getUserResult.Value._id);
        }

        public async Task<Result<UserDto>> Get(ObjectId userId)
        {
            var getUserResult = await _userRepository.Get(userId);
            if (getUserResult.IsFailed) return Result.Fail("Erro ao buscar usuário.").WithErrors(getUserResult.Errors);

            return new UserDto
            {
                Name = getUserResult.Value.Name,
                Username = getUserResult.Value.Username,
                _id = getUserResult.Value._id,
                PushToken = getUserResult.Value.PushToken,
            };
        }

        public async Task<Result<List<UserDto>>> GetAll()
        {
            var getUsersResult = await _userRepository.GetAll();
            if (getUsersResult.IsFailed) return Result.Fail("Lista de usuárioa").WithErrors(getUsersResult.Errors);
            if (!getUsersResult.Value.Any()) return Result.Fail("Nenhum usuário foi encontrado");

            return getUsersResult.Value.Select(user => new UserDto
            {
                _id = user._id,
                Username = user.Username,
                Name = user.Name,
                PushToken = user.PushToken,
            }).ToList();
        }

        public async Task<Result<List<UserDto>>> GetContacts(ObjectId userId)
        {
            var getUsersResult = await _userRepository.Get(userId);
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar lista de contatos.").WithErrors(getUsersResult.Errors);
            if (getUsersResult.Value == null) return Result.Fail("Usuário não encontrado.");

            if (getUsersResult.Value == null || getUsersResult.Value.Contacts == null || getUsersResult.Value.Contacts.Count == 0)
                return Result.Fail("Lista de contatos não encontrada");


            var getContactsResult = await _userRepository.Get(getUsersResult.Value.Contacts);
            if (getContactsResult.IsFailed) return Result.Fail("Lista de contatos.").WithErrors(getContactsResult.Errors);
            if (!getContactsResult.Value.Any()) return Result.Fail("Contatos não encontrados.");

            return getContactsResult.Value.Select(x => new UserDto
            {
                _id = x._id,
                Name = x.Name,
                Username = x.Username
            }).ToList();
        }

        public async Task<Result<List<UserDto>>> RemoveContact(ObjectId userId, ObjectId contactId)
        {
            var getUsersResult = await _userRepository.Get(userId);
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar lista de contatos.").WithErrors(getUsersResult.Errors);
            if (getUsersResult.Value == null) return Result.Fail("Usuário não encontrado.");

            getUsersResult.Value.Contacts.Remove(contactId);

            var updateResult = await _userRepository.UpdateContacts(getUsersResult.Value._id, getUsersResult.Value.Contacts);
            if (updateResult.IsFailed) return Result.Fail("Erro ao atualizar lista de contatos").WithErrors(updateResult.Errors);

            return await GetContacts(getUsersResult.Value._id);
        }

        public async Task<Result> UpdatePushToken(ObjectId userId, UpdatePushTokenModel updatePushTokenModel)
        {
            var getUsersResult = await _userRepository.Get(userId);
            if (getUsersResult.IsFailed) return Result.Fail("Erro ao buscar lista de contatos.").WithErrors(getUsersResult.Errors);
            if (getUsersResult.Value == null) return Result.Fail("Usuário não encontrado.");

            var updateResult = await _userRepository.UpdatePushToken(userId, updatePushTokenModel.PushToken);
            if (updateResult.IsFailed) return Result.Fail("Erro ao atualizar push token.").WithErrors(updateResult.Errors);

            var userStateJson = await _distributedCache.GetStringAsync(userId.ToString());

            return Result.Ok();

            // todo: refatorar
            if (string.IsNullOrEmpty(userStateJson)) return Result.Fail("");

            var userState = JsonSerializer.Deserialize<UserState>(userStateJson);
            userState.PushToken = updatePushTokenModel.PushToken;

            await _distributedCache.RemoveAsync(userId.ToString());
            await _distributedCache.SetStringAsync(userId.ToString(), JsonSerializer.Serialize(userState));
        }
    }
}
