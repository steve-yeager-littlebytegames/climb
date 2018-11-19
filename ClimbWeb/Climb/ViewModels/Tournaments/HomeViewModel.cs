using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Tournaments
{
    public class HomeViewModel : PageViewModel
    {
        public string Name { get; }
        public IReadOnlyList<Round> Winners { get; }
        public IReadOnlyList<Round> Losers { get; }
        public IReadOnlyList<Round> GrandFinals { get; }

        public HomeViewModel(ApplicationUser user, Tournament tournament)
            : base(user)
        {
            Name = tournament.Name;

            var winners = new List<Round>();
            var losers = new List<Round>();
            var grands = new List<Round>();

            foreach(var round in tournament.Rounds)
            {
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