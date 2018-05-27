using System;

namespace IQueryableFilter.Infrastructure.Time
{
    public interface ITimeService
    {
        DateTimeOffset Now { get; }
    }
}