using Framework.Configuration;
using Framework.Models.Authentication;
using Framework.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Framework.Middlewares
{
    public class AutoryzacjaHandler : AuthorizationHandler<AutoryzacjaAttribute>
    {
        private readonly ILogger<AutoryzacjaHandler> _logger;

        private readonly IServiceProvider _serviceProvider;
        public AutoryzacjaHandler(ILogger<AutoryzacjaHandler> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        // Check whether a given AutoryzacjaRequirement is satisfied or not for a particular
        // context.
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   AutoryzacjaAttribute requirement)
        {
            // Log as a warning so that it's very clear in sample output which authorization
            // policies(and requirements/handlers) are in use.
            //_logger.LogWarning("Evaluating authorization requirement for age >= {age}",
            //                                                            requirement.Age);

            IAuthenticationService? _authenticationService;

            using (var scope = _serviceProvider.CreateScope())
            {
                _authenticationService = scope.ServiceProvider.GetRequiredKeyedService<IAuthenticationService>(FrameworkConfiguration.AuthorizationProvider);
            }


            AuthenticationUser? authUser = await _authenticationService.GetAuthenticatedUser();

            if (authUser is null || authUser.Id is null) context.Fail();

            if (requirement.Role is not null)
            {
                Claim? userRole = context.User.FindFirst(c => c.Type == ClaimTypes.Role);

                if (authUser?.Role is null || authUser?.Role != requirement.Role.ToString())
                {
                    context.Fail();
                }
            }

            // Check the user's age.



            context.Succeed(requirement);

            //if (dateOfBirthClaim != null)
            //{
            //    // If the user has a date of birth claim, check their age.
            //    var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value,
            //                                                   CultureInfo.InvariantCulture);
            //    var age = DateTime.Now.Year - dateOfBirth.Year;
            //    if (dateOfBirth > DateTime.Now.AddYears(-age))
            //    {
            //        // Adjust age if the user hasn't had a birthday yet this year.
            //        age--;
            //    }

            //    // If the user meets the age criterion, mark the authorization requirement
            //    // succeeded.
            //    if (age >= requirement.Age)
            //    {
            //        _logger.LogInformation(
            //            "Minimum age authorization requirement {age} satisfied",
            //              requirement.Age);
            //        context.Succeed(requirement);
            //    }
            //    else
            //    {
            //        _logger.LogInformation("Current user's DateOfBirth claim ({dateOfBirth})"
            //           + " does not satisfy the minimum age authorization requirement {age}",
            //            dateOfBirthClaim.Value,
            //            requirement.Age);
            //    }
            //}
            //else
            //{
            //    _logger.LogInformation("No DateOfBirth claim present");
            //}

            //return Task.CompletedTask;
        }
    }
}
