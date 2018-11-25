using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Site
{
    public class HomeViewModel : BaseViewModel
    {
        public IReadOnlyList<Game> TopGames { get; }

        private HomeViewModel(ApplicationUser user, IReadOnlyList<Game> topGames, IConfiguration configuration)
            : base(user, configuration)
        {
            TopGames = topGames;
        }

        public static async Task<HomeViewModel> Create(ApplicationUser user, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            var games = await dbContext.Games
                .Include(g => g.Leagues).AsNoTracking()
                .OrderByDescending(g => g.Leagues.Count)
                .Take(5).ToArrayAsync();

            return new HomeViewModel(user, games, configuration);
        }
    }
}