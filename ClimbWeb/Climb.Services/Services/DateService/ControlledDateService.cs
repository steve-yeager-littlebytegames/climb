using System;
using Microsoft.Extensions.Configuration;

namespace Climb.Services
{
    public class ControlledDateService : IDateService
    {
        public const string OverrideKey = "DateTimeOverride";

        public DateTime Now { get; }

        public ControlledDateService(IConfiguration configuration)
        {
            Now = DateTime.Parse(configuration[OverrideKey]);
        }
    }
}