using Climb.Data;
using JetBrains.Annotations;

namespace Climb.ViewModels
{
    public class RequestViewModel<T> : BaseViewModel
    {
        // Do not change name! MVC automatically binds with prefix 'Request'.
        [UsedImplicitly]
        public T Request { get; }

        public RequestViewModel(ApplicationUser user)
            : base(user)
        {
        }
    }
}