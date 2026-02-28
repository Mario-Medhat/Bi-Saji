using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Interfaces.ServicesInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Servant> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenRepository tokenRepository;
        private readonly IUserRepository userRepository;
        private readonly IRoleService roleService;
        private readonly ILogger<AuthController> logger;

        public AuthController(UserManager<Servant> userManager, RoleManager<IdentityRole> roleManager,
            ITokenRepository tokenRepository, IUserRepository userRepository, IRoleService roleService, ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
            this.roleService = roleService;
            this.logger = logger;
        }


        // POST: api/Auth/Regiester
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] ServantRegiesterRequestDto regiesterRequestDto)
        {
            try
            {
                // create user and get the result
                (var identityResult, var servant) = await userRepository.CreateAsync(regiesterRequestDto);

                // If the user creation failed, log the error and return a bad request response
                if (!identityResult.Succeeded)
                {
                    logger.LogError($"User {regiesterRequestDto.FullName} regiesteration failed. Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    return BadRequest("User Regiesteration failed! Please try again.");
                }

                // Check if the provided roles exist in the system
                var nonExistingRoles = await roleService.RolesExistsAsync(regiesterRequestDto.Roles);

                // If there are any non-existing roles, return a bad request response
                if (nonExistingRoles.Any())
                    return BadRequest($"[{string.Join(", ", nonExistingRoles)}] roles does not exist! Please provide a valid roles.");

                // If all roles exist, proceed to add the user to the specified roles
                identityResult = await userRepository.AddRolesToUserAsync(servant, regiesterRequestDto.Roles);

                // If the user was successfully added to the roles, return a success response
                logger.LogInformation($"User {regiesterRequestDto.FullName} regiestered successfully with roles: {string.Join(", ", regiesterRequestDto.Roles)}");
                return Ok("User Regiestered! Please login.");

            }
            catch( UserAlreadyExistsException ueEx)
            {
                logger.LogError(ueEx.Message);
                return BadRequest(ueEx.Message);
            }
            catch (DbUpdateException dbEx)
            {
                logger.LogError(dbEx, $"A database error occurred while registering user {regiesterRequestDto.FullName}. Exception: {dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "A database error occurred while processing your request. Please try again later.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while registering user {regiesterRequestDto.FullName}. Exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
            }
        }

        // POST: api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var servant = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == loginRequestDto.PhoneNumber);

            if (servant == null)
                return BadRequest("Invalid username!");

            var isValidPassword = await userManager.CheckPasswordAsync(servant, loginRequestDto.Password);
            if (isValidPassword)
            {
                var roles = await userManager.GetRolesAsync(servant);
                if (roles != null)
                {
                    // Generate JWT token and return to client
                    var jwtToken = tokenRepository.CreateTWTToken(servant, roles.ToList());

                    var response = new LoginResponseDto
                    {
                        JwtToken = jwtToken,
                    };

                    logger.LogInformation($"User {loginRequestDto.PhoneNumber} logged in successfully with token: {jwtToken}");
                    return Ok(response);
                }
                else
                    return BadRequest("User has no roles assigned!");
            }
            else
                return BadRequest("Invalid password!");
        }

        // TODO: Implement Logout endpoint to invalidate the JWT token on the client side (if needed)
        // TODO: Implement ChangePassword endpoint to allow users to change their password
    }
}
