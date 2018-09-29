using System.Threading.Tasks;
using Climb.Data;
using Climb.Extensions;
using Climb.Services;
using Climb.ViewModels.Sets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.Controllers
{
    public class SetController : BaseController<SetController>
    {
        public SetController(ApplicationDbContext dbContext, ILogger<SetController> logger, IUserManager userManager)
            : base(logger, userManager, dbContext)
        {
        }

        [HttpGet("sets/fight/{setID:int}")]
        public async Task<IActionResult> Fight(int setID)
        {
            var referer = Request.GetReferer();
            var user = await GetViewUserAsync();

            var set = await dbContext.Sets
                .Include(s => s.Player1).AsNoTracking()
                .Include(s => s.Player2).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == setID);

            var viewModel = new FightViewModel(user, set, referer);
            return View(viewModel);
        }
    }
}