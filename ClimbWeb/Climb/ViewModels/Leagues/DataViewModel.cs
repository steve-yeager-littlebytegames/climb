using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.ViewModels.Leagues
{
    public class DataViewModel : PageViewModel
    {
        public class CharacterData
        {
            public int matches;
            public int wins;
            public int losses;
            public int totalSets;

            public decimal UsagePercent => totalSets == 0 ? 0 : (decimal)matches / totalSets;
            public decimal WinPercent => matches == 0 ? 0 : (decimal)wins / matches;
            public decimal WinTotalPercent => totalSets == 0 ? 0 : (decimal)wins / totalSets;
            public decimal LossPercent => matches == 0 ? 0 : (decimal)losses / matches;
            public decimal LossTotalPercent => totalSets == 0 ? 0 : (decimal)losses / totalSets;
        }

        public IOrderedEnumerable<KeyValuePair<Character, CharacterData>> AllCharacterData { get; }

        private DataViewModel(ApplicationUser user, League league, IReadOnlyDictionary<Character, CharacterData> allCharacterData)
            : base(user, league)
        {
            AllCharacterData = allCharacterData.OrderBy(x => x.Key.Name);
        }

        public static async Task<DataViewModel> Create(ApplicationUser user, int leagueID, ApplicationDbContext dbContext)
        {
            var league = await dbContext.Leagues
                .Include(l => l.Members)
                .Include(l => l.Game).ThenInclude(g => g.Characters).AsNoTracking()
                .Include(l => l.Sets).ThenInclude(s => s.Matches).ThenInclude(m => m.MatchCharacters).ThenInclude(mc => mc.Character).AsNoTracking()
                .FirstOrDefaultAsync(l => l.ID == leagueID);

            var data = league.Game.Characters.ToDictionary(c => c, c => new CharacterData());
            var totalSets = 0;

            foreach(var set in league.Sets)
            {
                if(!set.IsComplete)
                {
                    continue;
                }

                ++totalSets;

                foreach(var match in set.Matches)
                {
                    var winnerID = match.Player1Score > match.Player2Score ? set.Player1ID : set.Player2ID;

                    foreach(var matchCharacter in match.MatchCharacters)
                    {
                        var characterData = data[matchCharacter.Character];
                        ++characterData.matches;

                        if(matchCharacter.LeagueUserID == winnerID)
                        {
                            ++characterData.wins;
                        }
                        else
                        {
                            ++characterData.losses;
                        }
                    }
                }
            }

            foreach(var characterData in data)
            {
                characterData.Value.totalSets = totalSets;
            }

            return new DataViewModel(user, league, data);
        }
    }
}