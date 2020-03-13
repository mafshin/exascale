using System;

namespace ExaScale.Sharding.DataProvider
{
    public interface ISubShardDataProvider: IDisposable
    {
        long GetShardItemsCount();
        void Initialize(string connection);
    }
}
