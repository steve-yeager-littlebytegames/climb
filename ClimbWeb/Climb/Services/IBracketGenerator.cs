using System.Collections.Generic;
using Climb.Models;

namespace Climb.Services
{
    public interface IBracketGenerator
    {
        int MinCompetitors { get; }
        List<SetSlot> Generate(IReadOnlyList<TournamentUser> competitors);
    }
}