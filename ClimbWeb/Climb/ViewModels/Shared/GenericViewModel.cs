using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels
{
    public class GenericViewModel<T> : BaseViewModel
    {
        public T Value { get; }

        public GenericViewModel(ApplicationUser user, T value, IConfiguration configuration)
            : base(user, configuration)
        {
            Value = value;
        }
    }
}