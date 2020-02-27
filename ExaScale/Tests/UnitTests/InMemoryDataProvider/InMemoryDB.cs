using ExaScale.Sharding.Common;
using ExaScale.Sharding.DataProvider;
using ExaScale.ShardManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.InMemoryDataProvider
{
    public class InMemoryDB : ShardedDb<InMemorySubShardDataProvider>
    {
        public InMemoryDB(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm) :
            base(shardConfiguration, algorithm,
            new InMemoryMainShardDataProvider(shardConfiguration, algorithm),
            new InMemoryConnectionProvider())
        {

        }

        protected new InMemorySubShardDataProvider GetShard(string shardKey)
        {
            return base.GetShard(shardKey) as InMemorySubShardDataProvider;
        }

        internal void AddCustomer(SampleCustomerInformation customer)
        {
            var shard = GetShard(customer.CustomerId.ToString());
            shard.AddCustomer(customer);
        }

        public new void AddShard()
        {
            base.AddShard();
        }
    }

    public class InMemoryConnectionProvider : IConnectionProvider
    {
        public string GetConnectionString(int shardId)
        {
            return shardId.ToString();
        }
    }
    public class InMemoryMainShardDataProvider : IMainShardDataProvider
    {
        private Dictionary<string, int> _shardMap;
        private IShardKeyAlgorithm _shardKeyAlgorithm;
        private ShardConfiguration _shardConfiguration;

        public InMemoryMainShardDataProvider(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm)
        {
            _shardMap = new Dictionary<string, int>();
            _shardConfiguration = shardConfiguration;
            _shardKeyAlgorithm = algorithm;
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
                _shardMap.Add(shardKey, shardId);
                return shardId;
            }
        }

        public int AddShard()
        {
            return _shardConfiguration.IncreaseShardCount();
        }
        
    }
    public class InMemorySubShardDataProvider : IInMemorySubShardDataProvider
    {
        List<SampleCustomerInformation> customers = new List<SampleCustomerInformation>();
        public void AddCustomer(SampleCustomerInformation customer)
        {
            customers.Add(customer);
        }

        public void Initialize(string connection)
        {

        }

        public long GetShardItemsCount()
        {
            return customers.Count;
        }
    }

    public interface IInMemorySubShardDataProvider : ISubShardDataProvider
    {
        void AddCustomer(SampleCustomerInformation customer);
    }

    public class SampleCustomerInformation
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
