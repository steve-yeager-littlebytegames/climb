using System.Collections.Generic;
using Climb.Models;
using Microsoft.AspNetCore.Identity;
using Climb.Services;
using System;

namespace Climb.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicKey { get; set; }
        public string Name { get; set; }

        public List<OrganizationUser> Organizations { get; set; }
        public List<LeagueUser> LeagueUsers { get; set; }

        public string GetProfilePicUrl(ICdnService cdnService)
        {
            const int defaultPicturesCount = 24;

            if (string.IsNullOrWhiteSpace(ProfilePicKey))
            {
                var idHash = Id.GetHashCode();
                var defaultID = Math.Abs(idHash % defaultPicturesCount);
                return $"/images/profile-default/profile-default-{defaultID}.jpg";
            }
            else
            {
                return cdnService.GetImageUrl(ProfilePicKey, ClimbImageRules.ProfilePic);
            }
        }
    }
}