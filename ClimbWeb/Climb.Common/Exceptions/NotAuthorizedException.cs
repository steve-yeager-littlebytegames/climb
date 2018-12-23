using System;

namespace Climb.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
            : base("User is not permitted to do the attempted action.")
        {
        }

        public NotAuthorizedException(string userID, string message)
            : base($"User '{userID}' is not authorized to {message}.")
        {
        }
    }
}