namespace BiSaji.API.Interfaces.ServicesInterfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// Asynchronously determines whether a role with the specified name exists.
        /// </summary>
        /// <param name="roleName">The name of the role to check for existence. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// role exists; otherwise, <see langword="false"/>.</returns>
        public Task<bool> RoleExistsAsync(string roleName);

        /// <summary>
        /// Determines whether all specified roles exist asynchronously.
        /// </summary>
        /// <param name="roleNames">An array of role names to check for existence. Cannot be null or contain null or empty elements.</param>
        /// <returns>A tuple where the first value is <see langword="true"/> if all specified roles exist; otherwise, <see
        /// langword="false"/>. The second value is a list of role names that do not exist. The list is empty if all
        /// roles exist.</returns>
        public Task<List<string>> RolesExistsAsync(params string[] roleNames);

    }
}
