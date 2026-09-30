using Framework.Configuration;
using Framework.Enums;
using Framework.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Framework.Repositories.File
{
    public interface IFileRepositoryModels
    {
        public DbSet<FileModel> Files { get; set; }
    }

    public enum FileStoreOption
    {
        Local = 1,
        Database = 2,
    }

    public class FileRepository<TDbContext> : IFileRepository
        where TDbContext : DbContext, IFileRepositoryModels
    {
        private readonly TDbContext _dbContext;
        private readonly DbSet<FileModel> _dbSet;

        public FileRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<FileModel>();
        }

        public async Task<List<FileModel>> Create(List<IFormFile> files, int? userId = null, UserRole? role = null, long? maxFileSize = null, string[]? allowedExtensions = null, FileStoreOption option = FileStoreOption.Database)
        {
            List<FileModel> createdFiles = new();

            foreach (IFormFile formFile in files.Where(f => f.Length > 0))
                createdFiles.Add(await Create(formFile, userId, role, maxFileSize, allowedExtensions, option));

            return createdFiles;
        }

        public async Task<FileModel> CreateRaw(byte[]? content, string fileName, int? userId = null, UserRole? role = null, long? maxFileSize = null, string[]? allowedExtensions = null, FileStoreOption option = FileStoreOption.Database)
        {
            FileModel newFileModel = new()
            {
                Name = fileName,
                Created = DateTime.Now,
                UserId = userId,
                Role = role,
            };

            if (option is FileStoreOption.Database)
            {
                newFileModel.Content = content;
                newFileModel.FileGuid = Guid.NewGuid();
            }
            else
                newFileModel.FileGuid = Guid.NewGuid();

            await _dbSet.AddAsync(newFileModel);

            if (option is FileStoreOption.Local)
                using (var fs = new FileStream(GetFilePath(newFileModel), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    fs.Write(content, 0, content.Length);
                    fs.Close();
                }

            await _dbContext.SaveChangesAsync();
            return newFileModel;
        }

        public async Task<FileModel> Create(IFormFile formFile, int? userId = null, UserRole? role = null, long? maxFileSize = null, string[]? allowedExtensions = null, FileStoreOption option = FileStoreOption.Database)
        {
            Console.WriteLine("Wbilem do create");

            byte[]? content = GetContent(formFile);

            if (formFile.Length <= 0) throw new Exception("No file");

            if (maxFileSize is not null)
                if (formFile.Length / 1024 / 1024 > maxFileSize)
                    throw new Exception($"Max allowed file size is {maxFileSize}Mb");

            if (allowedExtensions is not null)
                if (!allowedExtensions.Select(e => e.ToLower()).Contains(formFile.FileName[(formFile.FileName.LastIndexOf('.') + 1)..].ToLower()))
                    throw new Exception("Not allowed extension");

            FileModel newFileModel = new()
            {
                Name = formFile.FileName,
                Created = DateTime.Now,
                UserId = userId,
                Role = role,
            };

            if (option is FileStoreOption.Database)
            {
                newFileModel.Content = content;
                newFileModel.FileGuid = Guid.NewGuid();
            }
            else
                newFileModel.FileGuid = Guid.NewGuid();

            await _dbSet.AddAsync(newFileModel);

            if (option is FileStoreOption.Local)
                using (var fs = new FileStream(GetFilePath(newFileModel), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    fs.Write(content, 0, content.Length);
                    fs.Close();
                }

            await _dbContext.SaveChangesAsync();
            return newFileModel;
        }

        public byte[] GetContent(IFormFile formFile)
        {
            byte[]? content;
            using (Stream stream = formFile.OpenReadStream())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    content = memoryStream.ToArray();
                }
            }

            return content;
        }

        public async Task Delete(int fileId)
        {
            FileModel model = await _dbSet.Where(f => f.Id == fileId).FirstOrDefaultAsync()
                 ?? throw new Exception("File not found");

            if(model.Content is null) 
            {
                string path = GetFilePath(model);
                if (!System.IO.File.Exists(path)) throw new FileNotFoundException($"File not found: {path}");

                if (model.Content is not null) System.IO.File.Delete(path);
            }
            
            _dbSet.Remove(model);
            //await _dbContext.SaveChangesAsync();
        }

        private static string GetFilePath(FileModel model)
        {
            if (!Directory.Exists(FrameworkConfiguration.SaveFilePath))
                throw new Exception($"Save file path not created");
            DirectoryInfo dir = new(FrameworkConfiguration.SaveFilePath);
            if (!dir.Exists) dir.Create();

            return dir.FullName + Path.DirectorySeparatorChar + $"{model.Id}_{model.FileGuid}_{model.Name}";
        }

        public async Task<FileModel> Get(int fileId)
        {
            FileModel model = await _dbSet.Where(f => f.Id == fileId).FirstOrDefaultAsync()
                 ?? throw new Exception("File not found");

            if (model.Content is not null) return model;

            string path = GetFilePath(model);
            if (!System.IO.File.Exists(path)) throw new FileNotFoundException($"File not found: {path}");

            model.Content = await System.IO.File.ReadAllBytesAsync(path);
            return model;
        }
    }
}
