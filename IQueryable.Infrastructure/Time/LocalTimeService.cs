using System;

namespace IQueryableFilter.Infrastructure.Time
{
    public class LocalTimeService : ITimeService
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}