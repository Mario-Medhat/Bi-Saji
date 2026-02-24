namespace BiSaji.API.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string phoneNumber)
            : base($"User with {nameof(phoneNumber)} {phoneNumber} already exists.")
        {
        }
    }
}
