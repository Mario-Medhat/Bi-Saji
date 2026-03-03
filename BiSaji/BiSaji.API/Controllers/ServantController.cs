using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Servant;
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
    public class ServantController : ControllerBase
    {
        private readonly ServantService servantService;

        public ServantController(ServantService servantService)
        {
            this.servantService = servantService;
        }

        // GET: api/Servants/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var usersDto = await servantService.GetAllAsync(filterOn, filterQuery);
                return Ok(usersDto);
            }
            catch
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
                var userDto = await servantService.GetByIdAsync(id);

                // if user is found, return the user
                return Ok(userDto);
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch
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
                await servantService.RegisterAsync(regiesterRequestDto);
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
                var servantDto = await servantService.UpdateAsync(id, updateRequestDto);
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
                var servantDto = await servantService.DeleteAsync(id);
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

        // PUT: api/Servants/{id}/AddRole
        [HttpPut("{id:guid}/AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole([FromRoute] Guid id, [FromBody] RolesRequestDto rolesRequestDto)
        {
            try
            {
                await servantService.AddRolesAsync(id, rolesRequestDto.Roles);
                return Ok("Role assigned successfully!");
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to assign role! Error: {ex.Message}");
            }
        }

        // PUT: api/Servants/{id}/RemoveRole
        [HttpPut("{id:guid}/RemoveRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole([FromRoute] Guid id, [FromBody] RolesRequestDto rolesRequestDto)
        {
            try
            {
                await servantService.RemoveRolesAsync(id, rolesRequestDto.Roles);
                return Ok("Role removed successfully!");
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to remove role! Error: {ex.Message}");
            }
        }

        // PUT: api/Servants/{id}/ChangePassword
        [HttpPut("{id:guid}/ChangePassword")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] BasePasswordRequestDto changePasswordRequestDto)
        {
            try
            {
                await servantService.ChangePasswordAsync(id, changePasswordRequestDto);
                return Ok("Password changed successfully!");
            }
            catch (NotFoundException unfEx)
            {
                return NotFound(unfEx.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to change password! Error: {ex.Message}");
            }
        }
    }
}
