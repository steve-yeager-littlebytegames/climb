using System;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services.ModelServices
{
    public interface ISeasonService
    {
        Task<Season> Create(int leagueID, DateTime start, DateTime end);
        Task<Season> GenerateSchedule(int seasonID);
        Task<Season> PlaySet(int setID);
        Task<Season> UpdateRanksAsync(int seasonID);
        Task<Season> End(int seasonID);
        Task<Season> LeaveAsync(int participantID);
        Task<Season> JoinAsync(int seasonID, string userID);
    }
}