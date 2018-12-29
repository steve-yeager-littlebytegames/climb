using System;
using System.Net;

namespace Climb.Attributes
{
    public class SwaggerResponseAttribute : NSwag.Annotations.SwaggerResponseAttribute
    {
        public SwaggerResponseAttribute(HttpStatusCode statusCode, Type bodyType, string description = "", bool isNullable = false)
            : base(statusCode, bodyType)
        {
            Description = description;
            IsNullable = isNullable;
        }
    }
}