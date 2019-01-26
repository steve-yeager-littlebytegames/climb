using System;

namespace Climb.Core.TieBreakers
{
    public interface ITieBreakerFactory
    {
        ITieBreaker Create(DateTime now);
    }
}