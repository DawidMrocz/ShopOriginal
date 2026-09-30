using Framework.Enums;
using Framework.Models.File;
using Microsoft.AspNetCore.Http;

namespace Framework.Repositories.File
{
    public interface IFileRepository
    {
        Task<List<FileModel>> Create(List<IFormFile> files, int? userId = null, UserRole? role = null, long? maxFileSize = null, string[]? allowedExtensions = null, FileStoreOption option = FileStoreOption.Database);
        Task<FileModel> Create(IFormFile formFile, int? userId = null, UserRole? role = null, long? maxFileSize = null, string[]? allowedExtensions = null, FileStoreOption option = FileStoreOption.Database);
        Task<FileModel> CreateRaw(byte[] content,string fileName, int? userId = null, UserRole? role = null, long? maxFileSize = null, string[]? allowedExtensions = null, FileStoreOption option = FileStoreOption.Database);
        Task Delete(int fileId);
        Task<FileModel> Get(int fileId);
        byte[] GetContent(IFormFile formFile);
    }
}
