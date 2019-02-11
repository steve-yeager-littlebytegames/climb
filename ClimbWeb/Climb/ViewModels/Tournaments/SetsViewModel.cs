using System.Collections.Generic;
using System.Collections.ObjectModel;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Tournaments
{
    public class SetsViewModel : PageViewModel
    {
        public IReadOnlyCollection<Set> Sets { get; }

        public SetsViewModel(IList<Set> sets, Tournament tournament, ApplicationUser user, IConfiguration configuration)
            : base(user, tournament, configuration)
        {
            Sets = new ReadOnlyCollection<Set>(sets);
        }
    }
}