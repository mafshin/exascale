using System;

namespace ExaScale.Sharding.DataProvider
{
    public interface IMainShardDataProvider
    {
        int AddShard();
        int GetShardId(string shardKey);
    }
}
