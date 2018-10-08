using System;

namespace Climb.Services
{
    public class DateService : IDateService
    {
        public DateTime Now => DateTime.Now;
    }
}