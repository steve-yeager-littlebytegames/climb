using Climb.Data;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels.Site
{
    public class AboutViewModel : BaseViewModel
    {
        public string BuildVersion { get; }
        public string SwaggerPath { get; }

        public AboutViewModel(ApplicationUser user, IConfiguration configuration, string baseUrl)
            : base(user, configuration)
        {
            BuildVersion = configuration["BUILD_VERSION"] ?? "Local";
            SwaggerPath = baseUrl + "/swagger";
        }
    }
}