using BiSaji.API.Controllers;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BiSaji.API.Services
{
    public class ServantService
    {
        private readonly IServantRepository _servantRepository;
        private readonly ILogger<ServantService> logger;

        public ServantService(IServantRepository servantRepository, ILogger<ServantService> logger)
        {
            _servantRepository = servantRepository;
            this.logger = logger;
        }

        public async Task<List<ServantDto>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                var servantDm = await _servantRepository.GetAllAsync(filterOn, filterQuery);

                // TODO: Create a auto mapper profile to map identity user to user dto and return the list of user dto instead of identity user
                var servantDto = new List<ServantDto>();
                foreach (var user in servantDm)
                {
                    // Mapping identity user to user dto
                    var userDto = new ServantDto
                    {
                        Id = Guid.Parse(user.Id),
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber!
                    };
                    servantDto.Add(userDto);
                }

                logger.LogInformation($"Got {servantDto.Count()} Users from database with filterOn: {filterOn} and filterQuery: {filterQuery}");

                return servantDto;
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while getting users from database with filterOn: {filterOn} and filterQuery: {filterQuery}. Error: {ex.Message}");
                throw new Exception("An error occurred while processing your request. Please try again later.");
            }
        }

        public async Task<ServantDto> GetByIdAsync(Guid id)
        {
            try
            {
                // get user by id from user repository
                var servantDm = await _servantRepository.GetByIdAsync(id);

                // if user is null, return not found
                if (servantDm == null)
                {
                    throw new NotFoundException($"No user found with {nameof(id)} {id}.");
                }

                // Mapping identity user to user dto
                var servantDto = new ServantDto
                {
                    Id = id,
                    FullName = servantDm.FullName,
                    PhoneNumber = servantDm.PhoneNumber!
                };

                // if user is found, return the user
                logger.LogInformation($"Returning user with id {id}\n User returned:\n{JsonSerializer.Serialize(servantDm)}");
                return servantDto;
            }
            catch (NotFoundException unfEx)
            {
                logger.LogWarning($"User with id {id} not found in database. Exception: {unfEx.Message}");
                throw;
            }
            catch (Exception ex)
            {

                logger.LogError($"An error occurred while getting user with id {id} from database. Error: {ex.Message}");
                throw;
            }
        }

        public async Task RegisterAsync(ServantRegiesterRequestDto regiesterRequestDto)
        {
            try
            {
                // create user with user repository and return the result
                (var identityResult, var servant) = await _servantRepository.CreateAsync(regiesterRequestDto);

                if (!identityResult.Succeeded)
                {
                    logger.LogError($"Failed to regiester user {regiesterRequestDto.FullName} with roles: {string.Join(", ", regiesterRequestDto.Roles)}. Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    throw new Exception($"Failed to regiester user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                logger.LogInformation($"User {regiesterRequestDto.FullName} regiestered successfully with roles: {string.Join(", ", regiesterRequestDto.Roles)}");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to regiester user {regiesterRequestDto.FullName} with roles: {string.Join(", ", regiesterRequestDto.Roles)}. Error: {ex.Message}");
                throw new Exception($"Failed to regiester user! Error: {ex.Message}");

            }
        }

        public async Task<ServantDto> UpdateAsync(Guid id, SetvantUpdateRequestDto updateRequestDto)
        {
            try
            {
                // create user with user repository and return the result
                (var identityResult, var servantDm) = await _servantRepository.UpdateAsync(id, updateRequestDto);

                if (!identityResult.Succeeded)
                {
                    logger.LogError($"Failed to update user with id {id} Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    throw new Exception($"Failed to update user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                // Mapping identity user to user dto
                var servantDto = new ServantDto
                {
                    Id = id,
                    FullName = servantDm.FullName,
                    PhoneNumber = servantDm.PhoneNumber!
                };

                logger.LogInformation($"User {servantDto.FullName} updated successfully");
                return servantDto;
            }
            catch (NotFoundException unfEx)
            {
                logger.LogWarning($"User with id {id} not found in database. Exception: {unfEx.Message}");
                throw;
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, "Invalid input data for student registration.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update user with id {id} Error: {ex.Message}");
                throw new Exception($"Failed to update user! Error: {ex.Message}");
            }
        }

        public async Task<ServantDto> DeleteAsync(Guid id)
        {
            try
            {
                (var identityResult, var servantDm) = await _servantRepository.DeleteAsync(id);

                if (!identityResult.Succeeded)
                {
                    logger.LogError($"Failed to delete user with id {id} Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    throw new Exception($"Failed to delete user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }

                // Mapping identity user to user dto
                var servantDto = new ServantDto
                {
                    Id = id,
                    FullName = servantDm!.FullName,
                    PhoneNumber = servantDm.PhoneNumber!
                };

                logger.LogInformation($"User {servantDm.FullName} deleted successfully");
                return servantDto;
            }
            catch (NotFoundException unfEx)
            {
                logger.LogWarning($"User with id {id} not found in database. Exception: {unfEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete user with id {id} Error: {ex.Message}");
                throw;
            }
        }
    }
}
