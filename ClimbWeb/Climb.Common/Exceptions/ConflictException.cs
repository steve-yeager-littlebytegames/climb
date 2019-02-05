using System;

namespace Climb.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException()
        {
        }

        public ConflictException(Type modelType, string propertyName, string value)
            : base($"There is already a {modelType.Name} with {propertyName} of '{value}'.")
        {
        }
    }
}