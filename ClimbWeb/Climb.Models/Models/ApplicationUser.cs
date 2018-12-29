using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Climb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicKey { get; set; }
        public string Name { get; set; }

        public List<OrganizationUser> Organizations { get; set; }
        public List<LeagueUser> LeagueUsers { get; set; }
        public List<SeasonLeagueUser> Seasons { get; set; }
    }
}