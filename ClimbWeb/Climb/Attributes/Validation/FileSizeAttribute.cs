using System.ComponentModel.DataAnnotations;
using Climb.Extensions;
using Microsoft.AspNetCore.Http;

namespace Climb.Attributes.Validation
{
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly long bytes;
        private readonly bool isRequired;

        public FileSizeAttribute(long bytes, bool isRequired = false)
            : base($"File needs to be smaller than {bytes.ToBytesReadable()}.")
        {
            this.bytes = bytes;
            this.isRequired = isRequired;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!isRequired && value == null)
            {
                return ValidationResult.Success;
            }

            var file = (IFormFile)value;
            return file.Length > bytes ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}