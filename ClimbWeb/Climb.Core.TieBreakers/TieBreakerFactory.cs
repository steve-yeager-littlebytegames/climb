using System;

namespace Climb.Core.TieBreakers
{
    public class TieBreakerFactory : ITieBreakerFactory
    {
        public ITieBreaker Create(DateTime now)
        {
            return new TieBreaker()
                .AddRule(new TotalWinsRule())
                .AddRule(new TiedWinsRule())
                .AddRule(new LeaguePointsRule())
                .AddRule(new MembershipDurationRule(now));
        }
    }
}