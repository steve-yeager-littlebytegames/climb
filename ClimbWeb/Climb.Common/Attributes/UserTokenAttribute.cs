using Microsoft.AspNetCore.Mvc;

namespace Climb.Attributes
{
    public class UserTokenAttribute : FromHeaderAttribute
    {
        public UserTokenAttribute()
        {
            Name = "Authorization";
        }
    }
}