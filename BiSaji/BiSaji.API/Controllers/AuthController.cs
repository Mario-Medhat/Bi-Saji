using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Interfaces.ServicesInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;

        public AuthController(AuthService authService)
        {
            this.authService = authService;
        }

        // POST: api/Auth/Regiester
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] ServantRegiesterRequestDto regiesterRequestDto)
        {
            try
            {
                await authService.RegisterAsync(regiesterRequestDto);
                return Ok("User registered successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while processing your request. Please try again later.");
            }
        }

        // POST: api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var response = authService.LoginAsync(loginRequestDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
