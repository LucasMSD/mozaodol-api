using FluentResults;

namespace Mozaodol.Domain.Services
{
    public interface IStorageProvider
    {
        Task<Result<string>> GetDownloadUrl(string name);
        Task<Result> Upload(Stream stream, string name, string extension);
    }
}
