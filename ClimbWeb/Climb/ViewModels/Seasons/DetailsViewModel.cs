using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Climb.Data;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class DetailsViewModel : PageViewModel
    {
        public class SetDetails
        {
            public int Points { get; }
            public DateTime Date { get; }
            public int OpponentID { get; }
            public string OpponentName { get; }
            public int OpponentRank { get; }

            public SetDetails(int points, DateTime date, SeasonLeagueUser opponent)
            {
                Points = points;
                Date = date;
                OpponentID = opponent.ID;
                OpponentName = opponent.LeagueUser.DisplayName;
                OpponentRank = opponent.LeagueUser.Rank;
            }
        }

        public SeasonLeagueUser DetailsParticipant { get; }
        public IEnumerable<SetDetails> Sets { get; }

        public DetailsViewModel(ApplicationUser user, SeasonLeagueUser participant, Season season, IHostingEnvironment environment)
            : base(user, season, environment)
        {
            DetailsParticipant = participant;

            var sets = participant.P1Sets
                .Concat(participant.P2Sets)
                .Where(s => s.IsComplete)
                .OrderByDescending(s => s.UpdatedDate);

            Sets = sets.Select(s =>
            {
                Debug.Assert(s.UpdatedDate != null, "s.UpdatedDate != null");

                return s.SeasonPlayer1ID == participant.ID
                    ? new SetDetails(s.Player1SeasonPoints, s.UpdatedDate.Value, s.SeasonPlayer2)
                    : new SetDetails(s.Player2SeasonPoints, s.UpdatedDate.Value, s.SeasonPlayer1);
            });
        }
    }
}