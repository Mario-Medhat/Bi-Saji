using BiSaji.API.Controllers;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Interfaces.ServicesInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Services
{
    public class AuthService
    {
        private readonly UserManager<Servant> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenRepository tokenRepository;
        private readonly IServantRepository userRepository;
        private readonly IRoleService roleService;
        private readonly ServantService servantService;
        private readonly ILogger<AuthService> logger;

        public AuthService(UserManager<Servant> userManager, RoleManager<IdentityRole> roleManager,
            ITokenRepository tokenRepository, IServantRepository userRepository, IRoleService roleService,
            ServantService servantService, ILogger<AuthService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
            this.roleService = roleService;
            this.servantService = servantService;
            this.logger = logger;
        }

        public async Task RegisterAsync(ServantRegiesterRequestDto regiesterRequestDto)
        {
            await servantService.RegisterAsync(regiesterRequestDto);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            try
            {
                var servant = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == loginRequestDto.PhoneNumber);

                if (servant == null)
                    throw new Exception("Invalid username!");

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
                        return response;
                    }
                    else
                    {
                        logger.LogWarning($"User {loginRequestDto.PhoneNumber} has no roles assigned!");
                        throw new Exception("User has no roles assigned!");
                    }
                }
                else
                {
                    logger.LogWarning($"Invalid password attempt for user {loginRequestDto.PhoneNumber}!");
                    throw new Exception("Invalid password!");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred during login for user {loginRequestDto.PhoneNumber}: {ex.Message}");
                throw;
            }
        }
    }
}
