using System;
using System.Collections.Generic;

namespace ExaScale.Sharding.DataProvider
{
    public interface IMainShardDataProvider : IDisposable
    {
        void AddShard(int shardId);
        void Initialize(string connection);
        void LoadShardMap(List<int> shards, System.Collections.Generic.Dictionary<string, int> _shardMap);
        void AddShardKey(string shardKey, int shardId);
    }
}
