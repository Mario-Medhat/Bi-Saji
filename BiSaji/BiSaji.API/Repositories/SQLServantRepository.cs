using BiSaji.API.Exceptions;
using BiSaji.API.Interfaces.RepositoryInterfaces;
using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Repositories
{
    public class SQLServantRepository : IServantRepository
    {
        private readonly UserManager<Servant> userManager;

        public SQLServantRepository(UserManager<Servant> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IdentityResult> AddRolesToUserAsync(Servant servant, params string[] roles)
        {
            return await userManager.AddToRolesAsync(servant, roles);
        }

        public async Task<(IdentityResult, Servant)> CreateAsync(ServantRegiesterRequestDto regiesterRequestDto)
        {
            if (regiesterRequestDto == null)
                throw new ArgumentNullException(nameof(regiesterRequestDto));

            // Check if a user with the same phone number already exists
            bool isUserAlreadyExists = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == regiesterRequestDto.PhoneNumber) != null;

            // If a user with the same phone number exists, throw an exception
            if (isUserAlreadyExists)
                throw new UserAlreadyExistsException($"User with phone number {regiesterRequestDto.PhoneNumber} already exists.");

            // Create a new Servant based on the registration details
            var servant = new Servant
            {
                // Assigning the phone number as the main UserName
                UserName = regiesterRequestDto.PhoneNumber,
                FullName = regiesterRequestDto.FullName,
                BatchId = regiesterRequestDto.BatchId ?? null,
                PhoneNumber = regiesterRequestDto.PhoneNumber
            };

            var identityResult = await userManager.CreateAsync(servant, regiesterRequestDto.Password);

            return (identityResult, servant);
        }

        public async Task<IEnumerable<Servant>> GetAllAsync(string? filterOn, string? filterQuery)
        {
            var users = userManager.Users.AsQueryable();

            // Apply filtering if filterOn and filterQuery are provided
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("fullname", StringComparison.OrdinalIgnoreCase) || filterOn.Equals("name", StringComparison.OrdinalIgnoreCase))
                {
                    users = users.Where(user => EF.Functions.Like(user.FullName, $"%{filterQuery}%"));
                }
                else if (filterOn.Equals("phonenumber", StringComparison.OrdinalIgnoreCase) || filterOn.Equals("phone", StringComparison.OrdinalIgnoreCase))
                {
                    users = users.Where(user => EF.Functions.Like(user.PhoneNumber, $"%{filterQuery}%"));
                }
            }

            return await users.ToListAsync();
        }

        public async Task<Servant?> GetByIdAsync(Guid id)
        {
            return await userManager.Users.FirstOrDefaultAsync(user => user.Id == id.ToString());
        }

        public async Task<(IdentityResult, Servant)> UpdateAsync(Guid id, SetvantUpdateRequestDto updateRequestDto)
        {

            try
            {
                var user = await GetByIdAsync(id);


                if (user == null)
                {
                    throw new NotFoundException($"No user found with {nameof(id)} {id}.");
                }

                // Update the user's properties based on the provided updateRequestDto
                if (!string.IsNullOrWhiteSpace(updateRequestDto.FullName))
                    user.FullName = updateRequestDto.FullName;

                if (!string.IsNullOrWhiteSpace(updateRequestDto.PhoneNumber))
                {
                    user.UserName = updateRequestDto.PhoneNumber;
                    user.PhoneNumber = updateRequestDto.PhoneNumber;
                }

                if (!string.IsNullOrWhiteSpace(updateRequestDto.Password))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetResult = await userManager.ResetPasswordAsync(user, token, updateRequestDto.Password);
                    if (!passwordResetResult.Succeeded)
                    {
                        throw new Exception("Failed to reset the user's password.");
                    }
                }

                var identityResult = await userManager.UpdateAsync(user);

                return (identityResult, user);

            }
            catch (NotFoundException unfEx)
            {
                throw new NotFoundException($"No user found with {nameof(id)} {id}.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IdentityResult, Servant?)> DeleteAsync(Guid id)
        {
            try
            {
                var user = await GetByIdAsync(id);

                if (user == null)
                {
                    throw new NotFoundException($"No user found with {nameof(id)} {id}.");
                }

                var identityResult = await userManager.DeleteAsync(user);
                if (!identityResult.Succeeded)
                {
                    throw new Exception("Failed to delete the user.");
                }

                return (identityResult, user);
            }
            catch (NotFoundException unfEx)
            {
                throw new NotFoundException($"No user found with {nameof(id)} {id}.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
