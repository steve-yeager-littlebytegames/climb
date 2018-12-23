using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Climb.Utilities
{
    public class UrlUtility : IUrlUtility
    {
        public string EmailConfirmationLink(IUrlHelper urlHelper, string userID, string code, string scheme)
        {
            return urlHelper.EmailConfirmationLink(userID, code, scheme);
        }

        public string ResetPasswordCallbackLink(UrlHelper urlHelper, string userID, string code, string scheme)
        {
            throw new NotImplementedException();
        }
    }
}