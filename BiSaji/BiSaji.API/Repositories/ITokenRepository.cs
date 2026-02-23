using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Repositories
{
    public interface ITokenRepository
    {
        string CreateTWTToken(IdentityUser identityUser, List<string> roles);
    }
}
