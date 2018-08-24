using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;

namespace Climb.Services
{
    public interface IScheduleFactory
    {
        Task<List<Set>> GenerateScheduleAsync(Season season, ApplicationDbContext dbContext);
    }
}