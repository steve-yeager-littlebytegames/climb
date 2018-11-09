using System.Collections.Generic;
using Climb.Models;

namespace Climb.Services
{
    public class BracketGenerator: IBracketGenerator
    {
        public int MinCompetitors => 4;

        public List<SetSlot> Generate(IReadOnlyList<TournamentUser> competitors)
        {
            throw new System.NotImplementedException();
        }
    }
}