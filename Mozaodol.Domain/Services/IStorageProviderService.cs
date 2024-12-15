using FluentResults;

namespace Mozaodol.Domain.Services
{
    public interface IStorageProviderService
    {
        Task<Result<string>> GetDownloadUrl(string name);
        Task<Result> Upload(Stream stream, string name, string extension);
    }
}
