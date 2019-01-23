using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Hosting;

namespace Climb.ViewModels.Seasons
{
    public class DataViewModel : PageViewModel
    {
        public int OverdueSets { get; }
        public int CompletedSets { get; }
        public int RemainingSets { get; }
        public decimal CompletePercent { get; }
        public decimal TargetPercent { get; }

        public DataViewModel(ApplicationUser user, Season season, IHostingEnvironment environment, IDateService dateService)
            : base(user, season, environment)
        {
            if(!season.IsActive)
            {
                return;
            }

            var today = dateService.Now;

            var targetSetsCompleted = 0;
            foreach(var set in season.Sets)
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