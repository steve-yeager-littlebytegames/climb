using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.ViewModels.Organizations
{
    public class IndexViewModel : BaseViewModel
    {
        public IReadOnlyList<Organization> AllOrganizations { get; }

        public IndexViewModel(ApplicationUser user, IReadOnlyList<Organization> allOrganizations)
            : base(user)
        {
            AllOrganizations = allOrganizations;
        }
    }
}