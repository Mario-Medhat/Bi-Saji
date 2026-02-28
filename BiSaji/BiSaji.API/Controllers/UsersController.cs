using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto;
using BiSaji.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BiSaji.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Servant> userManager;
        private readonly IUserRepository userRepository;
        private readonly ILogger<AuthController> logger;

        public UsersController(UserManager<Servant> userManager, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.logger = logger;
        }

        // GET: api/Users/GetAll
        [HttpGet]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                var usersDm = await userRepository.GetAllAsync(filterOn, filterQuery);

                // TODO: Create a auto mapper profile to map identity user to user dto and return the list of user dto instead of identity user
                var usersDto = new List<UserDto>();
                foreach (var user in usersDm)
                {
                    // Mapping identity user to user dto
                    var userDto = new UserDto
                    {
                        Id = Guid.Parse(user.Id),
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber
                    };
                    usersDto.Add(userDto);
                }

                logger.LogInformation($"Got {usersDto.Count()} Users from database with filterOn: {filterOn} and filterQuery: {filterQuery}");

                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while getting users from database with filterOn: {filterOn} and filterQuery: {filterQuery}. Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
            }
        }

        // GET: api/Users/{id}
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                // get user by id from user repository
                var userDm = await userRepository.GetByIdAsync(id);

                // if user is null, return not found
                if (userDm == null)
                {
                    throw new UserNotFoundException(id);
                }

                // Mapping identity user to user dto
                var userDto = new UserDto
                {
                    Id = id,
                    FullName = userDm.FullName,
                    PhoneNumber = userDm.PhoneNumber
                };

                // if user is found, return the user
                logger.LogInformation($"Returning user with id {id}\n User returned:\n{JsonSerializer.Serialize(userDm)}");
                return Ok(userDto);
            }

            catch (UserNotFoundException unfEx)
            {
                logger.LogWarning($"User with id {id} not found in database. Exception: {unfEx.Message}");
                return StatusCode(StatusCodes.Status404NotFound, unfEx.Message);
            }
            catch (Exception ex)
            {

                logger.LogError($"An error occurred while getting user with id {id} from database. Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
            }
        }

        // POST: api/Users/Register
        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegiesterRequestDto regiesterRequestDto)
        {
            try
            {
                // create user with user repository and return the result
                (var identityResult, var servant) = await userRepository.CreateAsync(regiesterRequestDto);

                if (!identityResult.Succeeded)
                {
                    logger.LogError($"Failed to regiester user {regiesterRequestDto.FullName} with roles: {string.Join(", ", regiesterRequestDto.Roles)}. Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    return BadRequest($"Failed to regiester user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                logger.LogInformation($"User {regiesterRequestDto.FullName} regiestered successfully with roles: {string.Join(", ", regiesterRequestDto.Roles)}");
                return Ok("User Regiestered! Please login.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to regiester user {regiesterRequestDto.FullName} with roles: {string.Join(", ", regiesterRequestDto.Roles)}. Error: {ex.Message}");
                return BadRequest($"Failed to regiester user! Error: {ex.Message}");

            }
        }

        // Put: api/Users/UPDATE
        [HttpPut]
        [Route("Update/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRequestDto updateRequestDto)
        {
            try
            {
                // create user with user repository and return the result
                (var identityResult, var servant) = await userRepository.UpdateAsync(id, updateRequestDto);

                if (!identityResult.Succeeded)
                {
                    logger.LogError($"Failed to update user with id {id} Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    return BadRequest($"Failed to update user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                // Mapping identity user to user dto
                var userDto = new UserDto
                {
                    Id = id,
                    FullName = servant.FullName,
                    PhoneNumber = servant.PhoneNumber
                };

                logger.LogInformation($"User {updateRequestDto.FullName} updated successfully");
                return Ok($"User updated successfully! \n\n{JsonSerializer.Serialize(userDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update user with id {id} Error: {ex.Message}");
                return BadRequest($"Failed to update user! Error: {ex.Message}");
            }
        }

        // Delete: api/Users/Delete
        [HttpDelete]
        [Route("Delete/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                (var identityResult, var servant) = await userRepository.DeleteAsync(id);

                if (!identityResult.Succeeded)
                {
                    logger.LogError($"Failed to delete user with id {id} Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    return BadRequest($"Failed to delete user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                // Mapping identity user to user dto
                var userDto = new UserDto
                {
                    Id = id,
                    FullName = servant.FullName,
                    PhoneNumber = servant.PhoneNumber
                };

                logger.LogInformation($"User {servant.FullName} deleted successfully");
                return Ok($"User deleted successfully! \n\n{JsonSerializer.Serialize(userDto, new JsonSerializerOptions { WriteIndented = true })}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete user with id {id} Error: {ex.Message}");
                return BadRequest($"Failed to delete user! Error: {ex.Message}");
            }
        }
    }
}
