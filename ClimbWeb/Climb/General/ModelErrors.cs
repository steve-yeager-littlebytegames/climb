using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Climb
{
    public class ModelErrors
    {
        [JsonProperty]
        private Dictionary<string, List<string>> errors;

        private void SaveErrors(ModelStateDictionary modelState)
        {
            errors = new Dictionary<string, List<string>>(modelState.Count);
            foreach(var error in modelState.Where(x => x.Value.Errors.Count > 0))
            {
                errors[error.Key] = error.Value.Errors.Select(e => e.ErrorMessage).ToList();
            }
        }

        public void AssignErrors(ModelStateDictionary modelState)
        {
            foreach(var error in errors)
            {
                foreach(var message in error.Value)
                {
                    modelState.AddModelError(error.Key, message);
                }
            }
        }

        public static ModelErrors Create(ModelStateDictionary modelState)
        {
            var modelErrors = new ModelErrors();
            modelErrors.SaveErrors(modelState);
            return modelErrors;
        }
    }
}