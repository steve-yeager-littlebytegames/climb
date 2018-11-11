using Climb.Data;

namespace Climb.ViewModels
{
    public class GenericViewModel<T> : BaseViewModel
    {
        public T Value { get; }

        public GenericViewModel(ApplicationUser user, T value)
            : base(user)
        {
            Value = value;
        }
    }
}