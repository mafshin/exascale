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
        private Dictionary<string, int> _shardMap;
        private readonly IMainShardDataProvider _mainShardDataProvider;
        private readonly IConnectionProvider _connectionProvider;
        private Dictionary<int, ISubShardDataProvider> subShardMap;
        protected readonly ShardConfiguration _shardConfiguration;
        private IShardKeyAlgorithm _shardKeyAlgorithm;
        public ShardedDb(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm,
            IMainShardDataProvider mainShardDataProvider, IConnectionProvider connectionProvider)
        {
            this._shardConfiguration = shardConfiguration;
            this._shardKeyAlgorithm = algorithm;
            this._mainShardDataProvider = mainShardDataProvider;
            this._connectionProvider = connectionProvider;
            subShardMap = new Dictionary<int, ISubShardDataProvider>();
            _shardMap = new Dictionary<string, int>();

            var connection = connectionProvider.GetMainConnectionString();
            mainShardDataProvider.Initialize(connection);

            mainShardDataProvider.LoadShardMap(_shardMap);

            if (subShardMap.Count < shardConfiguration.ShardCount)
            {
                SetupShard(shardConfiguration.ShardCount - subShardMap.Count);
            }
        }

        public virtual void LoadShardMap(Dictionary<string, int> map)
        {

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
            var connection = _connectionProvider.GetShardConnectionString(newShardId);
            shard.Initialize(connection);
            _mainShardDataProvider.AddShard(newShardId);
            subShardMap.Add(newShardId, shard);
            return newShardId;
        }

        public int AddShard()
        {
            _shardConfiguration.IncreaseShardCount();

            var newShardId = SetupShard();

            return newShardId;
        }


        public int GetShardId(string shardKey)
        {
            if (_shardMap.ContainsKey(shardKey))
            {
                return _shardMap[shardKey];
            }
            else
            {
                var shardId = _shardKeyAlgorithm.GetShardId(shardKey);
                _mainShardDataProvider.AddShardKey(shardKey, shardId);
                _shardMap.Add(shardKey, shardId);
                return shardId;
            }
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
