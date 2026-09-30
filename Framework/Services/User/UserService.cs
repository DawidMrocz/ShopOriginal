using AngleSharp.Css.Values;
using Framework.ApiModels.Access.Request;
using Framework.ApiModels.User;
using Framework.Configuration;
using Framework.Enums;
using Framework.Models.File;
using Framework.Models.User;
using Framework.Repositories.File;
using Framework.Repositories.User;
using Framework.Services.Email;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Services.User
{
    public class UserService<TEntity, TUserId> : IUserService<TEntity, TUserId>
        where TEntity : UserBaseModel<TUserId>, new()
    {
        private readonly IUserRepository<TEntity, TUserId> _userRepository;
        private readonly IEmailService _emailService;
        private readonly IFileRepository _fileRepository;
        public UserService(IUserRepository<TEntity, TUserId> userRepository, IEmailService emailService, IFileRepository fileRepository)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _fileRepository = fileRepository;
        }
        public async Task Block(TUserId id, DateTime? blockTo)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            entity.Blocked = true;

            if(blockTo is not null)
                entity.UnblockTime = blockTo;

            _userRepository.Update(entity);
        }

        public async Task ChangePassword(TUserId id, ChangePasswordRequest request)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            if (!VerifyPasswordHash(request.OldPassword, entity.PasswordHash, entity.PasswordSalt))
                throw new Exception("Nie poprawne hasło");

            (byte[], byte[]) result = CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            entity.PasswordHash = result.Item2;
            entity.PasswordSalt = result.Item1;
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private static (byte[], byte[]) CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            return (passwordSalt, passwordHash);
        }

        public async Task ChangeRole(TUserId id, UserRole role)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            entity.Role = role;

            _userRepository.Update(entity);
        }

        public async Task Delete(TUserId id)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            _userRepository.Delete(entity);
        }

        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public async Task RemindPassword(UserForgotPassword request, string culture)
        {
            TEntity? entity = await _userRepository.GetByEmail(request.Email);

            if (entity is null) return;
         
            string newPassword = CreatePassword(8);

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            entity.PasswordHash = passwordHash;
            entity.PasswordSalt = passwordSalt;

            List<MailAddress> mailAddresses = [new MailAddress(request.Email)];

            StringBuilder title = new();

            if(entity.FirstName is null) 
            {
                title.Append("użytkowniku");
            }
            else
            {
                title.Append($"{entity.FirstName} ");
                if(entity.LastName is not null)
                    title.Append(entity.LastName);
            }

            Dictionary<string, string> bodyParmas = new()
            {
                { "UserName", title.ToString() },
                { "Password", newPassword }
            };

            Dictionary<string, string> subjectParmas = new()
            {
                { "UserName", title.ToString() },
            };

            await _emailService.SendEmail(mailAddresses, "Reminder", true, culture, new MailAddress(FrameworkConfiguration.User), bodyParmas, subjectParmas);
        }

        public async Task Unblock(TUserId id)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            entity.Blocked = false;
            entity.UnblockTime = null;

            _userRepository.Update(entity);
        }

        public async Task<TUserId> Create<TRequest>(TRequest request) where TRequest : ICreateRequest
        {
            TEntity model = new()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Street = request.Street,
                City = request.City,
                Country = request.Country,
                PostalCode = request.PostalCode
            };

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            model.PasswordHash = passwordHash;
            model.PasswordSalt = passwordSalt;

            return await _userRepository.Create(model);
        }

        public async Task Update<TRequest>(TUserId id, TRequest request) where TRequest : IUpdateRequest
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.Street = request.Street;
            entity.City = request.City;
            entity.Country = request.Country;
            entity.PostalCode = request.PostalCode;

            _userRepository.Update(entity);
        }

        public async Task AddProfilePhoto(IFormFile file, TUserId id)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            if (file.Length <= 0) throw new Exception("File is null");
            {
                FileModel newFile = await _fileRepository.Create(file);
                entity.Photo = newFile.Id;
            }
            _userRepository.Update(entity);
        }

        public async Task DeleteProfilePhoto(TUserId id)
        {
            TEntity entity = await _userRepository.GetById(id)
                ?? throw new Exception("User not found");

            if (entity.Photo is not null)
                await _fileRepository.Delete((int)entity.Photo);

            entity.Photo = null;

            _userRepository.Update(entity);
        }

        public async Task Activate(string activateCode)
        {
            Console.WriteLine("Not implemented");

            //TEntity entity = await _userRepository.GetById(4)
            //    ?? throw new Exception("User not found");

            //entity.Activated = true;

            //_userRepository.Update(entity);
        }
    }

    public interface IUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
    public interface ICreateRequest : IUpdateRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
