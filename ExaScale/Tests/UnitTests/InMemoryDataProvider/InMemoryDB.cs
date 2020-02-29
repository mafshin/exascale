using ExaScale.Sharding.Common;
using ExaScale.Sharding.DataProvider;
using ExaScale.ShardManager;
using System;
using System.Collections.Generic;
using System.Text;
using UnitTests.Common;

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
    }

    public class InMemoryConnectionProvider : IConnectionProvider
    {
        public string GetMainConnectionString()
        {
            return null;
        }

        public string GetShardConnectionString(int shardId)
        {
            return shardId.ToString();
        }
    }
    public class InMemoryMainShardDataProvider : IMainShardDataProvider
    {
        private IShardKeyAlgorithm _shardKeyAlgorithm;
        private ShardConfiguration _shardConfiguration;

        public InMemoryMainShardDataProvider(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm)
        {
            _shardConfiguration = shardConfiguration;
            _shardKeyAlgorithm = algorithm;
        }

        public void Initialize(string connection)
        {

        }

        public void LoadShardMap(Dictionary<string, int> _shardMap)
        {

        }

        public void AddShardKey(string shardKey, int shardId)
        {

        }

        public void AddShard(int shardId)
        {

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
}
