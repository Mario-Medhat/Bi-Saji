namespace BiSaji.API.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string phoneNumber)
            : base($"No user found with {nameof(phoneNumber)} {phoneNumber}.")
        {
        }
        public UserNotFoundException(Guid id)
            : base($"No user found with {nameof(id)} {id}.")
        {
        }
    }
}
