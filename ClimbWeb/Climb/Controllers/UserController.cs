using System.Threading.Tasks;
using Climb.Data;
using Climb.Services;
using Climb.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class UserController : BaseController<UserController>
    {
        private readonly ICdnService cdnService;
        private readonly IDateService dateService;

        public UserController(ApplicationDbContext dbContext, ILogger<UserController> logger, ICdnService cdnService, IUserManager userManager, IDateService dateService)
            : base(logger, userManager, dbContext)
        {
            this.cdnService = cdnService;
            this.dateService = dateService;
        }

        [HttpGet("users/home/{userID?}")]
        public async Task<IActionResult> Home(string userID)
        {
            var appUser = await GetViewUserAsync();
            var id = userID ?? appUser?.Id;

            if(string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("LogIn", "Account");
            }

            var user = await dbContext.Users
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Game).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P1Sets).ThenInclude(s => s.Matches).ThenInclude(m => m.MatchCharacters).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P1Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P1Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P1Sets).ThenInclude(s => s.League).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P2Sets).ThenInclude(s => s.Matches).ThenInclude(m => m.MatchCharacters).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P2Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P2Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User).AsNoTracking()
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.P2Sets).ThenInclude(s => s.League).AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = await HomeViewModel.CreateAsync(appUser, user, cdnService, dbContext, dateService);

            return View(viewModel);
        }
    }
}