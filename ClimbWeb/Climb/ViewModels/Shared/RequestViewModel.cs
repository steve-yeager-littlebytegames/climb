using Climb.Data;
using JetBrains.Annotations;

namespace Climb.ViewModels
{
    public class RequestViewModel<T> : BaseViewModel
    {
        [UsedImplicitly]
        public T RequestType { get; }

        public RequestViewModel(ApplicationUser user)
            : base(user)
        {
        }
    }
}