using Climb.Models;
using JetBrains.Annotations;

namespace Climb.ViewModels
{
    public interface IRequestViewModel<out T>
    {
        [UsedImplicitly]
        T Request { get; }
    }

    public class RequestViewModel<T> : BaseViewModel, IRequestViewModel<T>
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