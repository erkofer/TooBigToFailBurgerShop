﻿using System;

namespace TooBigToFailBurgerShop.Ordering.Domain.Core.SeedWork
{
    public interface IDomainEvent<out TKey>
    {
        long AggregateVersion { get; }
        TKey AggregateId { get; }
        DateTime Timestamp { get; }
    }
}
