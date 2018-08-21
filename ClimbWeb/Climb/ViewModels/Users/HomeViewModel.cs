using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Microsoft.EntityFrameworkCore;

namespace Climb.ViewModels.Users
{
    public class HomeViewModel : BaseViewModel
    {
        public class SharedLeagueUsers
        {
            public LeagueUser Requester { get; }
            public LeagueUser Challenged { get; }

            public SharedLeagueUsers(LeagueUser requester, LeagueUser challenged)
            {
                Requester = requester;
                Challenged = challenged;
            }
        }

        public ApplicationUser HomeUser { get; }
        public string ProfilePic { get; }
        public bool IsViewingUserHome => User.Id == HomeUser.Id;
        public IReadOnlyList<Set> RecentSets { get; }
        public IReadOnlyList<Set> AvailableSets { get; }
        public IReadOnlyList<SharedLeagueUsers> SharedLeagues { get; }
        public IReadOnlyList<SetRequest> SetRequests { get; }
        public bool ShowSetRequests { get; }

        private HomeViewModel(ApplicationUser user, ApplicationUser homeUser, string profilePic, IReadOnlyList<Set> recentSets, IReadOnlyList<Set> availableSets, IReadOnlyList<SetRequest> setRequests, bool showSetRequests)
            : base(user)
        {
            HomeUser = homeUser;
            ProfilePic = profilePic;
            RecentSets = recentSets;
            AvailableSets = availableSets;
            SetRequests = setRequests;
            ShowSetRequests = showSetRequests;

            var sharedLeagues = new List<SharedLeagueUsers>();
            foreach(var requester in user.LeagueUsers)
            {
                var challenged = homeUser.LeagueUsers.FirstOrDefault(lu => lu.LeagueID == requester.LeagueID);
                if(challenged != null)
                {
                    sharedLeagues.Add(new SharedLeagueUsers(requester, challenged));
                }
            }
            SharedLeagues = sharedLeagues;
        }

        public static async Task<HomeViewModel> CreateAsync(ApplicationUser user, ApplicationUser homeUser, ICdnService cdnService, ApplicationDbContext dbContext)
        {
            var profilePic = cdnService.GetUserProfilePicUrl(homeUser.Id, homeUser.ProfilePicKey, ClimbImageRules.ProfilePic);
            var sets = homeUser.LeagueUsers.SelectMany(lu => lu.P1Sets.Union(lu.P2Sets)).ToArray();
            var recentSets = sets.Where(s => s.IsComplete).Take(10).ToArray();
            var availableSets = sets.Where(s => !s.IsComplete).Take(100).ToArray();

#if DEBUG
            const bool showSetRequests = true;
#else
            var showSetRequests = user.Id == homeUser.Id;
#endif 

            IReadOnlyList<SetRequest> setRequests = null;
            if(showSetRequests)
            {
                setRequests = await dbContext.SetRequests
                    .Include(sr => sr.Requester).ThenInclude(lu => lu.User).AsNoTracking()
                    .Include(sr => sr.Challenged).ThenInclude(lu => lu.User).AsNoTracking()
                    .Include(sr => sr.League).AsNoTracking()
                    .Where(sr => homeUser.LeagueUsers.Any(lu => lu.ID == sr.ChallengedID))
                    .OrderByDescending(sr => sr.DateCreated)
                    .ToArrayAsync();
            }

            return new HomeViewModel(user, homeUser, profilePic, recentSets, availableSets, setRequests, showSetRequests);
        }
    }
}