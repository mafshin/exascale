using ExaScale.Sharding.Common;
using ExaScale.ShardManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExaScale.Sharding.DataProvider
{
    public class ShardedDb<T> where T : ISubShardDataProvider, new()
    {
        private readonly IMainShardDataProvider _mainShardDataProvider;
        private readonly IConnectionProvider _connectionProvider;
        private Dictionary<int, ISubShardDataProvider> subShardMap;

        public ShardedDb(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm,
            IMainShardDataProvider mainShardDataProvider, IConnectionProvider connectionProvider)
        {
            this._mainShardDataProvider = mainShardDataProvider;
            this._connectionProvider = connectionProvider;
            subShardMap = new Dictionary<int, ISubShardDataProvider>();

            //TODO: Load existing shards
            // LoadShardMap();

            if(subShardMap.Count < shardConfiguration.ShardCount)
            {
                SetupShard(shardConfiguration.ShardCount - subShardMap.Count);
            }
        }

        private void SetupShard(int newShardCount)
        {
            for (int i = 0; i < newShardCount; i++)
            {
                SetupShard();
            }
        }

        private int SetupShard()
        {
            var shard = Activator.CreateInstance<T>();
            var newShardId = subShardMap.Keys.DefaultIfEmpty().Max() + 1;
            var connection = _connectionProvider.GetConnectionString(newShardId);
            shard.Initialize(connection);
            subShardMap.Add(newShardId, shard);
            return newShardId;
        }

        protected int AddShard()
        {
            _mainShardDataProvider.AddShard();

            var newShardId = SetupShard();

            return newShardId;
        }

        protected int GetShardId(string shardKey)
        {
            return _mainShardDataProvider.GetShardId(shardKey);
        }

        protected ISubShardDataProvider GetShard(string shardKey)
        {
            var shardId = GetShardId(shardKey);            

            return subShardMap[shardId];
        }

        public List<ShardStats> GetTotalShardItems()
        {
            return subShardMap.Select(d => new ShardStats()
            {
                ShardId = d.Key,
                ItemsCount = d.Value.GetShardItemsCount()
            }).ToList();
        }
    }

    public class ShardStats
    {
        public int ShardId { get; set; }
        public long ItemsCount { get; set; }
    }
}
