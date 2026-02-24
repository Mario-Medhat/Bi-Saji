using BiSaji.API.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;

namespace BiSaji.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<RoleService> logger;

        public RoleService(RoleManager<IdentityRole> roleManager, ILogger<RoleService> logger)
        {
            this.roleManager = roleManager;
            this.logger = logger;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var exists = await roleManager.RoleExistsAsync(roleName);

            if (!exists)
                logger.LogWarning("Role {RoleName} does not exist", roleName);

            return exists;
        }

        public async Task<List<string>> RolesExistsAsync(params string[] roleNames)
        {
            // Check if the input array is not null and contains elements before iterating
            if (roleNames == null || roleNames.Length == 0)
            {
                logger.LogWarning("No role names provided to check for existence.");
                return new List<string>();
            }

            // Initialize a list to hold the names of roles that do not exist
            var nonExistingRoles = new List<string>();

            foreach (var roleName in roleNames)
            {
                // Check if the role exists
                var exists = await roleManager.RoleExistsAsync(roleName);

                // If the role does not exist, add it to the list of non-existing roles
                if (!exists)
                    nonExistingRoles.Add(roleName);

            }

            // Log a warning if there are any non-existing roles
            if (nonExistingRoles.Count != 0)
                logger.LogWarning("Some roles do not exist: {Roles}", nonExistingRoles);

            // Return a tuple containing the result of whether all roles exist and the list of non-existing roles
            return nonExistingRoles;
        }
    }
}
