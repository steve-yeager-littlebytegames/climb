using System;
using System.Collections.Generic;
using System.Linq;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class DetailsViewModel : PageViewModel
    {
        public class SetDetails
        {
            public int ID { get; }
            public int Points { get; }
            public DateTime Date { get; }
            public int OpponentID { get; }
            public string OpponentName { get; }
            public int OpponentRank { get; }
            public bool Won { get; }

            public SetDetails(int id, int points, DateTime date, SeasonLeagueUser opponent, bool won)
            {
                Points = points;
                Date = date;
                Won = won;
                ID = id;
                OpponentID = opponent.ID;
                OpponentName = opponent.LeagueUser.DisplayName;
                OpponentRank = opponent.LeagueUser.Rank;
            }
        }

        public SeasonLeagueUser DetailsParticipant { get; }
        public IReadOnlyList<SetDetails> Sets { get; }
        public string ProfilePic { get; }
        public int RemainingSets { get; }

        public DetailsViewModel(ApplicationUser user, SeasonLeagueUser participant, Season season, IHostingEnvironment environment, ICdnService cdnService)
            : base(user, season, environment)
        {
            DetailsParticipant = participant;

            var sets = participant.P1Sets
                .Concat(participant.P2Sets)
                .OrderByDescending(s => s.UpdatedDate)
                .ToArray();

            Sets = sets.Where(s => s.IsComplete).Select(s => s.SeasonPlayer1ID == participant.ID
                    ? new SetDetails(s.ID, s.Player1SeasonPoints, s.UpdatedDate ?? season.StartDate, s.SeasonPlayer2, s.SeasonWinnerID == participant.ID)
                    : new SetDetails(s.ID, s.Player2SeasonPoints, s.UpdatedDate ?? season.StartDate, s.SeasonPlayer1, s.SeasonWinnerID == participant.ID))
                .ToArray();

            RemainingSets = sets.Count(s => !s.IsComplete);

            ProfilePic = cdnService.GetUserProfilePicUrl(participant.UserID, participant.User.ProfilePicKey, ClimbImageRules.ProfilePic);
        }
    }
}