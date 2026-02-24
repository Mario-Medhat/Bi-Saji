using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Interfaces.RepositoryInterfaces
{
    public interface ITokenRepository
    {
        string CreateTWTToken(IdentityUser identityUser, List<string> roles);
    }
}
