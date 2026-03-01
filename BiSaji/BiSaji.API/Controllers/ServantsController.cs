using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using BiSaji.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServantsController : ControllerBase
    {
        private readonly ServantService _servantService;

        public ServantsController(ServantService servantService)
        {
            _servantService = servantService;
        }

        // GET: api/Servants/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var usersDto = await _servantService.GetAllAsync(filterOn, filterQuery);
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
            }
        }

        // GET: api/Servants/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                // get user by id from user repository
                var userDto = await _servantService.GetByIdAsync(id);

                // if user is found, return the user
                return Ok(userDto);
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
            }
        }

        // POST: api/Servants/
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] ServantRegiesterRequestDto regiesterRequestDto)
        {
            try
            {
                await _servantService.RegisterAsync(regiesterRequestDto);
                return Ok("Servant Regiestered! Please login.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to regiester user! Error: {ex.Message}");
            }
        }

        // Put: api/Servants/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] SetvantUpdateRequestDto updateRequestDto)
        {
            try
            {
                var servantDto = await _servantService.UpdateAsync(id, updateRequestDto);
                return Ok($"User updated successfully! \n\n{JsonSerializer.Serialize(servantDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update user! Error: {ex.Message}");
            }
        }

        // Delete: api/Servants/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var servantDto = await _servantService.DeleteAsync(id);
                return Ok($"User deleted successfully! \n\n{JsonSerializer.Serialize(servantDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete user! Error: {ex.Message}");
            }
        }
    }
}
