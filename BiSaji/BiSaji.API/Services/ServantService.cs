using BiSaji.API.Controllers;
using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Auth;
using BiSaji.API.Models.Dto.Servant;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace BiSaji.API.Services
{
    public class ServantService
    {
        private readonly IServantRepository servantRepository;
        private readonly ILogger<ServantService> logger;

        public ServantService(IServantRepository servantRepository, ILogger<ServantService> logger)
        {
            this.servantRepository = servantRepository;
            this.logger = logger;
        }

        public async Task<List<ServantDto>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            try
            {
                var servantsDm = await servantRepository.GetAllAsync(filterOn, filterQuery);

                // TODO: Create a auto mapper profile to map identity user to user dto and return the list of user dto instead of identity user
                var servantsDto = new List<ServantDto>();
                foreach (var servantDm in servantsDm)
                {
                    // Mapping identity user to user dto
                    var servantDto = new ServantDto
                    {
                        Id = Guid.Parse(servantDm.Id),
                        FullName = servantDm.FullName,
                        PhoneNumber = servantDm.PhoneNumber!,
                        BatchId = servantDm.BatchId,
                        BatchName = servantDm.Batch != null ? servantDm.Batch.Name : null,
                        Roles = await servantRepository.GetRolesAsync(servantDm)
                    };
                    servantsDto.Add(servantDto);
                }

                logger.LogInformation($"Got {servantsDto.Count()} Users from database with filterOn: {filterOn} and filterQuery: {filterQuery}");

                return servantsDto;
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
                var servantDm = await servantRepository.GetByIdAsync(id);

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
                    PhoneNumber = servantDm.PhoneNumber!,
                    BatchId = servantDm.BatchId,
                    BatchName = servantDm.Batch != null ? servantDm.Batch.Name : null,
                    Roles = await servantRepository.GetRolesAsync(servantDm)
                };

                // if user is found, return the user
                logger.LogInformation($"Returning user with id {id}\n User returned:\n{JsonSerializer.Serialize(servantDto)}");
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
            IdentityResult identityResult = new IdentityResult();

            try
            {
                // create user with user repository
                (identityResult, var servant) = await servantRepository.CreateAsync(regiesterRequestDto);

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
                // if identity result errors are more than 0 thats mean the error is related to identity framework and we can get the error from identity result errors
                if (identityResult.Errors.Count() > 0)
                {
                    logger.LogError($"Failed to regiester user {regiesterRequestDto.FullName} with roles: {string.Join(", ", regiesterRequestDto.Roles)}. Error: {string.Join(", ", identityResult.Errors)}");
                    throw new Exception($"Failed to regiester user! Error: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }
                // else the error is not related to identity framework
                else
                {
                    logger.LogError($"Failed to regiester user {regiesterRequestDto.FullName} with roles: {string.Join(", ", regiesterRequestDto.Roles)}. Error: {ex.Message}");
                    throw new Exception($"Failed to regiester user! Error: {ex.Message}");
                }

            }
        }

        public async Task<ServantDto> UpdateAsync(Guid id, SetvantUpdateRequestDto updateRequestDto)
        {
            try
            {
                // create user with user repository and return the result
                (var identityResult, var servantDm) = await servantRepository.UpdateAsync(id, updateRequestDto);

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
                (var identityResult, var servantDm) = await servantRepository.DeleteAsync(id);

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

        public async Task<ServantDto> AddRolesAsync(Guid id, params string[] roles)
        {
            try
            {
                // get user by id from user repository
                Servant servantDm = await servantRepository.GetByIdAsync(id);

                // if user is null, return not found
                if (servantDm == null)
                {
                    throw new NotFoundException($"No user found with {nameof(id)} {id}.");
                }

                foreach (var role in roles)
                {
                    var identityResult = await servantRepository.AddRolesAsync(servantDm, role);
                    if (!identityResult.Succeeded)
                    {
                        logger.LogError($"Failed to assign role {role} to user with id {id} Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                        throw new Exception($"Failed to assign role {role} to user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    }
                }
                // Mapping identity user to user dto
                var servantDto = new ServantDto
                {
                    Id = id,
                    FullName = servantDm.FullName,
                    PhoneNumber = servantDm.PhoneNumber!,
                    Roles = await servantRepository.GetRolesAsync(servantDm)
                };
                logger.LogInformation($"Roles [{string.Join(", ", roles)}] assigned to user {servantDm.FullName} successfully");
                return servantDto;
            }
            catch (NotFoundException unfEx)
            {
                logger.LogWarning($"User with id {id} not found in database. Exception: {unfEx.Message}");
                throw;
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, "Invalid input data for role assignment.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to assign roles [{string.Join(", ", roles)}] to user with id {id} Error: {ex.Message}");
                throw new Exception($"Failed to assign roles [{string.Join(", ", roles)}] to user! Error: {ex.Message}");
            }
        }
        public async Task<ServantDto> RemoveRolesAsync(Guid id, params string[] roles)
        {
            try
            {
                // get user by id from user repository
                Servant servantDm = await servantRepository.GetByIdAsync(id);

                // if user is null, return not found
                if (servantDm == null)
                {
                    throw new NotFoundException($"No user found with {nameof(id)} {id}.");
                }

                foreach (var role in roles)
                {
                    var identityResult = await servantRepository.RemoveRolesAsync(servantDm, role);
                    if (!identityResult.Succeeded)
                    {
                        logger.LogError($"Failed to remove role {role} from user with id {id} Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                        throw new Exception($"Failed to remove role {role} from user! Errors: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                    }
                }
                // Mapping identity user to user dto
                var servantDto = new ServantDto
                {
                    Id = id,
                    FullName = servantDm.FullName,
                    PhoneNumber = servantDm.PhoneNumber!,
                    Roles = await servantRepository.GetRolesAsync(servantDm)
                };
                logger.LogInformation($"Roles [{string.Join(", ", roles)}] removed from user {servantDm.FullName} successfully");
                return servantDto;
            }
            catch (NotFoundException unfEx)
            {
                logger.LogWarning($"User with id {id} not found in database. Exception: {unfEx.Message}");
                throw;
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, "Invalid input data for role assignment.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to remove roles [{string.Join(", ", roles)}] from user with id {id} Error: {ex.Message}");
                throw new Exception($"Failed to remove roles [{string.Join(", ", roles)}] from user! Error: {ex.Message}");
            }
        }

        public async Task<Servant> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordRequestDto changePasswordRequestDto)
        {
            return await servantRepository.ChangePasswordAsync(user, changePasswordRequestDto);
        }
        public async Task<Servant> ChangePasswordAsync(Guid id, BasePasswordRequestDto changePasswordRequestDto)
        {
            return await servantRepository.ChangePasswordAsync(id, changePasswordRequestDto);
        }
        public async Task<Servant> ChangePasswordAsync(Servant servantDm, BasePasswordRequestDto changePasswordRequestDto)
        {
            try
            {
                // create user with user repository and return the result
                servantDm = await servantRepository.ChangePasswordAsync(servantDm, changePasswordRequestDto);

                logger.LogInformation($"User {servantDm.FullName} updated successfully");
                return servantDm;
            }
            catch (NotFoundException unfEx)
            {
                logger.LogWarning($"User with id {servantDm.Id} not found in database. Exception: {unfEx.Message}");
                throw;
            }
            catch (ArgumentException argEx)
            {
                logger.LogWarning(argEx, "Invalid input data for student registration.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update user with id {servantDm.Id} Error: {ex.Message}");
                throw new Exception($"Failed to update user! Error: {ex.Message}");
            }
        }
    }
}
