using System.Collections.Generic;
using Climb.Data;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Organizations
{
    public class IndexViewModel : BaseViewModel
    {
        public IReadOnlyList<Organization> AllOrganizations { get; }

        public IndexViewModel(ApplicationUser user, IReadOnlyList<Organization> allOrganizations, IConfiguration configuration)
            : base(user, configuration)
        {
            AllOrganizations = allOrganizations;
        }
    }
}