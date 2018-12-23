using System;

namespace Climb.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(Type modelType, int id)
            : base($"Could not find {modelType.Name} with ID '{id}'.")
        {
        }
        
        public NotFoundException(Type modelType, string id)
            : base($"Could not find {modelType.Name} with ID '{id}'.")
        {
        }
    }
}