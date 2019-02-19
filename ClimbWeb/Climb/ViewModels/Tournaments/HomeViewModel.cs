using System.Collections.Generic;
using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Tournaments
{
    public class HomeViewModel : PageViewModel
    {
        public IReadOnlyList<Round> Winners { get; }
        public IReadOnlyList<Round> Losers { get; }
        public IReadOnlyList<Round> GrandFinals { get; }

        public HomeViewModel(ApplicationUser user, Tournament tournament, IConfiguration configuration)
            : base(user, tournament, configuration)
        {
            var winners = new List<Round>();
            var losers = new List<Round>();
            var grands = new List<Round>();

            foreach(var round in tournament.Rounds)
            {
                round.SetSlots.Sort((a, b) => a.Identifier.CompareTo(b.Identifier));
                switch(round.Bracket)
                {
                    case Round.Brackets.Winners:
                        winners.Add(round);
                        break;
                    case Round.Brackets.Losers:
                        losers.Add(round);
                        break;
                    case Round.Brackets.Grands:
                        grands.Add(round);
                        break;
                }
            }

            winners.Sort((x, y) => x.Index.CompareTo(y.Index));
            losers.Sort((x, y) => x.Index.CompareTo(y.Index));
            grands.Sort((x, y) => x.Index.CompareTo(y.Index));

            Winners = winners;
            Losers = losers;
            GrandFinals = grands;
        }
    }
}