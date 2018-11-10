using System;
using System.Collections.Generic;
using Climb.Models;

namespace Climb.Services
{
    public class BracketGenerator: IBracketGenerator
    {
        public int MinCompetitors => 4;

        public List<SetSlot> Generate(int competitorCount)
        {
            if(competitorCount < MinCompetitors)
            {
                throw new ArgumentException($"Need at least {MinCompetitors} competitors to generate bracket.", nameof(competitorCount));
            }

            throw new NotImplementedException();
        }
    }
}