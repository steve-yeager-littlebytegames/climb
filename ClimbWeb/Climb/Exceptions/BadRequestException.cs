using System;
using JetBrains.Annotations;

namespace Climb.Exceptions
{
    public class BadRequestException : Exception
    {
        [UsedImplicitly]
        public BadRequestException()
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string argumentName, string message)
            : base($"Problem with {argumentName}. {message}")
        {
        }
    }
}