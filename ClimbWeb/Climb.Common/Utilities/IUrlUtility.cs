using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Climb.Utilities
{
    public interface IUrlUtility
    {
        string EmailConfirmationLink(IUrlHelper urlHelper, string userID, string code, string scheme);
        string ResetPasswordCallbackLink(UrlHelper urlHelper, string userID, string code, string scheme);
    }
}