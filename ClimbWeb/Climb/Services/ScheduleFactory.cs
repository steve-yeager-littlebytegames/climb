using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;

namespace Climb.Services
{
    public abstract class ScheduleFactory : IScheduleFactory
    {
        protected abstract List<Set> GenerateScheduleInternal(Season season);

        public async Task<List<Set>> GenerateScheduleAsync(Season season, ApplicationDbContext dbContext)
        {
            if(season.Participants == null || season.Participants.Count < 2)
            {
                throw new InvalidOperationException("Season needs more than 2 participants.");
            }

            if(season.Sets.Count > 0)
            {
                dbContext.RemoveRange(season.Sets);
                await dbContext.SaveChangesAsync();
                season.Sets.Clear();
            }

            var sets = GenerateScheduleInternal(season);
            dbContext.Sets.AddRange(sets);
            await dbContext.SaveChangesAsync();

            return sets;
        }
    }
}