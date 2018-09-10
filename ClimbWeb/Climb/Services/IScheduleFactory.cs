using System;
using System.Collections.Generic;
using Climb.Models;

namespace Climb.Services
{
    public interface IScheduleFactory
    {
        List<Set> GenerateSchedule(DateTime startDate, DateTime endDate, IReadOnlyList<SeasonLeagueUser> participants);
        void Reschedule(DateTime startDate, DateTime endDate, IReadOnlyList<Set> sets, IReadOnlyList<SeasonLeagueUser> participants);
    }
}