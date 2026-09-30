using Contracts.RabbitMessages.UserApp;
using Framework.ApiModels.Access.Request;
using Framework.ApiModels.User;
using Framework.Controllers;
using Framework.Enums;
using Framework.Middlewares;
using Framework.Services.Email;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using UserApp.ApiModels.User;
using UserApp.Data;
using UserApp.Models;
using UserApp.Repositories;

namespace UserApp.Controllers
{
    public class AddPhotoRequest
    {
        public IFormFile Photo { get; set; }
    }

    [Route("[controller]")]
    [Autoryzacja(UserRole.User)]
    public class UserController : AuthorizedController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IEmailService _emailService;
        public UserController(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint, IEmailService emailService) : base(serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _emailService = emailService;
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            List<MailAddress> mailAddresses = [new MailAddress("you@example.com")];

            Dictionary<string, string> bodyParmas = new()
            {
                { "UserName","emailll" },
                { "Password", "haslooo" }
            };

            Dictionary<string, string> subjectParmas = new()
            {
                { "UserName","emailll" },
            };

            await _emailService.SendEmail(mailAddresses, "Reminder", true, Culture, new MailAddress("you@example.com"), bodyParmas, subjectParmas);


            return Ok("Wiadomosc z test");
        }

        /// <summary>
        /// Rejestracja użytkownika
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] UserRegisterRequest request)
        {
            try
            {
                Guid userId = await _unitOfWork.UserService.Create(request);
                await _unitOfWork.Save();

                User user = await _unitOfWork.UserRepository.GetById(userId)
                    ?? throw new Exception("User not found");

                int favoirteId = await _unitOfWork.FavoriteService.Create(userId);

                await _publishEndpoint.Publish(new UserCreated
                {
                    Id = user.Id,
                    FavoriteId = favoirteId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Street = user.Street,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    Country = user.Country
                });

                return Ok("User have been created");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Aktywacja użytkownika
        /// </summary>
        /// <param name="activateCode"></param>
        /// <returns></returns>
        [HttpPost("activate")]
        [AllowAnonymous]
        public async Task<IActionResult> Activate([FromRoute] string activateCode)
        {
            try
            {
                await _unitOfWork.UserService.Activate(activateCode);
                await _unitOfWork.Save();
                return Ok("User have been created");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Dodanie zdjęia profilowego
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("photo")]
        public async Task<IActionResult> AddPhoto([FromForm] AddPhotoRequest request)
        {
            try
            {
                await _unitOfWork.UserService.AddProfilePhoto(request.Photo, Guid.Parse(AuthorizedUser!.Id));
                await _unitOfWork.Save();
                return Ok("Photo has been added");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Usunięcie zdęcia profilowego
        /// </summary>
        /// <returns></returns>
        [HttpDelete("photo")]
        public async Task<IActionResult> DeletePhoto()
        {
            try
            {
                await _unitOfWork.UserService.DeleteProfilePhoto(Guid.Parse(AuthorizedUser!.Id));
                await _unitOfWork.Save();
                return Ok("Photo has been deleted");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Usunięcie konta
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            try
            {
                await _unitOfWork.UserService.Delete(Guid.Parse(AuthorizedUser!.Id));
                await _unitOfWork.Save();
                return Ok("User have been deleted");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Aktualizacja konta
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequest request)
        {
            try
            {
                await _unitOfWork.UserService.Update(Guid.Parse(AuthorizedUser!.Id), request);
                await _unitOfWork.Save();
                return Ok("User have been updated");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Zablokowanie konta
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("block/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Block([FromRoute] Guid userId, [FromQuery] DateTime? blockTo)
        {
            try
            {
                await _unitOfWork.UserService.Block(userId, blockTo);
                await _unitOfWork.Save();
                return Ok("User have been blocked");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Odblokowanie konta
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("unblock/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unblock([FromRoute] Guid userId)
        {
            try
            {
                await _unitOfWork.UserService.Unblock(userId);
                await _unitOfWork.Save();
                return Ok();
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Zmiana hasła
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                await _unitOfWork.UserService.ChangePassword(Guid.Parse(AuthorizedUser!.Id), request);
                await _unitOfWork.Save();
                return Ok();
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Przypomnienie hasła
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPassword request)
        {
            try
            {
                await _unitOfWork.UserService.RemindPassword(request, Culture);
                await _unitOfWork.Save();
                return Ok();
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }

        /// <summary>
        /// Zmiana roli użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPut("change-role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole([FromRoute] Guid userId, [FromBody] UserRole role)
        {
            try
            {
                await _unitOfWork.UserService.ChangeRole(userId, role);
                await _unitOfWork.Save();
                return Ok("Role has been changed");
            }
            catch (Exception)
            {
                return Problem("Server error");
            }
        }
    }
}
