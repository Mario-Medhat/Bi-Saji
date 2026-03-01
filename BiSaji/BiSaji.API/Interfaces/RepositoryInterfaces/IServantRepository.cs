using BiSaji.API.Models.Domain;
using BiSaji.API.Models.Dto.Auth;
using BiSaji.API.Models.Dto.Servant;
using BiSaji.API.Models.Dto.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface IServantRepository
    {
        public Task<IEnumerable<Servant>> GetAllAsync(string? filterOn, string? filterQuery);
        public Task<Servant?> GetByIdAsync(Guid id);
        public Task<(IdentityResult, Servant)> CreateAsync(ServantRegiesterRequestDto regiesterRequestDto);
        public Task<(IdentityResult, Servant)> UpdateAsync(Guid id, SetvantUpdateRequestDto updateRequestDto);
        public Task<(IdentityResult, Servant?)> DeleteAsync(Guid id);
        public Task<IdentityResult> AddRolesToUserAsync(Servant servant, params string[] roles);
        public Task<Servant> ChangePasswordAsync(Guid id, BasePasswordRequestDto changePasswordRequestDto);
        public Task<Servant> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordRequestDto changePasswordRequestDto);
        public Task<Servant> ChangePasswordAsync(Servant servant, BasePasswordRequestDto changePasswordRequestDto);
    }
}
