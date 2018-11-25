using Climb.Data;
using Climb.Models;
using Climb.Services;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Seasons
{
    public class DataViewModel : PageViewModel
    {
        public int OverdueSets { get; }
        public int CompletedSets { get; }
        public int RemainingSets { get; }
        public decimal CompletePercent { get; }
        public decimal TargetPercent { get; }

        public DataViewModel(ApplicationUser user, Season season, IConfiguration configuration, IDateService dateService)
            : base(user, season, configuration)
        {
            if(!season.IsActive)
            {
                return;
            }

            var today = dateService.Now;

            var targetSetsCompleted = 0;
            foreach (var set in season.Sets)
            {
                if(set.DueDate < today)
                {
                    ++targetSetsCompleted;
                }

                if(set.IsComplete)
                {
                    ++CompletedSets;
                }
                else if(set.DueDate >= today)
                {
                    ++RemainingSets;
                }
                else
                {
                    ++OverdueSets;
                }
            }

            var setCount = (decimal)season.Sets.Count;
            CompletePercent = CompletedSets / setCount;
            TargetPercent = targetSetsCompleted / setCount;
        }
    }
}