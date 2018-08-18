using System.Threading.Tasks;
using Climb.Data;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Fight()
        {
            var user = await GetViewUserAsync();

            var viewModel = new BaseViewModel(user);
            return View(viewModel);
        }
    }
}