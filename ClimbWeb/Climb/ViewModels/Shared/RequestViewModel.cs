using Climb.Data;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels
{
    public class RequestViewModel<T> : BaseViewModel
    {
        // Do not change name! MVC automatically binds with prefix 'Request'.
        [UsedImplicitly]
        public T Request { get; }

        public RequestViewModel(ApplicationUser user, IConfiguration configuration)
            : base(user, configuration)
        {
        }
    }
}