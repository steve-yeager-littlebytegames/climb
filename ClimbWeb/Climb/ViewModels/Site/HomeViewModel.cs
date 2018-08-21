using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.ViewModels.Site
{
    public class HomeViewModel : BaseViewModel
    {
        public IReadOnlyList<Game> Games { get; }

        private HomeViewModel(ApplicationUser user, IReadOnlyList<Game> games)
            : base(user)
        {
            Games = games;
        }

        public static async Task<HomeViewModel> Create(ApplicationUser user, ApplicationDbContext dbContext)
        {
            var games = await dbContext.Games
                .Include(g => g.Leagues).AsNoTracking()
                .OrderByDescending(g => g.Leagues.Count)
                .Take(5).ToArrayAsync();

            return new HomeViewModel(user, games);
        }
    }
}