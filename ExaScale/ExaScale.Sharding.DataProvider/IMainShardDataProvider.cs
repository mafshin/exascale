using System;

namespace ExaScale.Sharding.DataProvider
{
    public interface IMainShardDataProvider
    {
        void AddShard(int shardId);
        void Initialize(string connection);
        void LoadShardMap(System.Collections.Generic.Dictionary<string, int> _shardMap);
        void AddShardKey(string shardKey, int shardId);
    }
}
