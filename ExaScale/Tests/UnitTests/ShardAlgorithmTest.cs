using ExaScale.Sharding.Common;
using ExaScale.Sharding.Common.Algorithms;
using ExaScale.Sharding.DataProvider;
using ExaScale.ShardManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Common;
using UnitTests.InMemoryDataProvider;

namespace UnitTests
{
    [TestClass]
    public class ShardAlgorithmTest
    {
        [TestMethod]
        public void ChangeShardCount_Test()
        {
            ShardConfiguration shardConfiguration = new ShardConfiguration(3);
            IShardKeyAlgorithm shardKeyAlgorithm = new EvenlyDistributedShardKeyAlgorithm(shardConfiguration);            
            InMemoryDB dataProvider = new InMemoryDB(shardConfiguration, shardKeyAlgorithm);
            int shardId;
            shardId = dataProvider.GetShardId("1");
            Assert.AreEqual(1, shardId);
            shardId = dataProvider.GetShardId("2");
            Assert.AreEqual(2, shardId);
            shardId = dataProvider.GetShardId("3");
            Assert.AreEqual(3, shardId);
            shardId = dataProvider.GetShardId("4");
            Assert.AreEqual(1, shardId);
            shardId = dataProvider.GetShardId("123");
            Assert.AreEqual(3, shardId);
            
            // 3 + 1 shards
            dataProvider.AddShard();

            shardId = dataProvider.GetShardId("1");
            Assert.AreEqual(1, shardId);
            shardId = dataProvider.GetShardId("2");
            Assert.AreEqual(2, shardId);
            shardId = dataProvider.GetShardId("3");
            Assert.AreEqual(3, shardId);
            shardId = dataProvider.GetShardId("4");
            Assert.AreEqual(1, shardId);
            shardId = dataProvider.GetShardId("123");
            Assert.AreEqual(3, shardId);
            shardId = dataProvider.GetShardId("5");
            Assert.AreEqual(1, shardId);
            shardId = dataProvider.GetShardId("6");
            Assert.AreEqual(2, shardId);
            shardId = dataProvider.GetShardId("7");
            Assert.AreEqual(3, shardId);
            shardId = dataProvider.GetShardId("8");
            Assert.AreEqual(4, shardId);
        }

        [TestMethod]
        public void AddShardItem_Test()
        {            
            ShardConfiguration shardConfiguration = new ShardConfiguration(3);
            IShardKeyAlgorithm shardKeyAlgorithm = new EvenlyDistributedShardKeyAlgorithm(shardConfiguration);
            InMemoryDB db = new InMemoryDB(shardConfiguration, shardKeyAlgorithm);

            AddCustomer(db, 1);
            AddCustomer(db, 2);
            AddCustomer(db, 3);
            AddCustomer(db, 4);

            // 3 shards
            var stats = db.GetTotalShardItems();
            Assert.AreEqual(2, stats[0].ItemsCount);
            Assert.AreEqual(1, stats[1].ItemsCount);
            Assert.AreEqual(1, stats[2].ItemsCount);

            // 4 shards
            db.AddShard();
            AddCustomer(db, 5);
            AddCustomer(db, 6);

            stats = db.GetTotalShardItems();
            Assert.AreEqual(3, stats[0].ItemsCount);
            Assert.AreEqual(2, stats[1].ItemsCount);
            Assert.AreEqual(1, stats[2].ItemsCount);

            // 5 shards
            db.AddShard();

            AddCustomer(db, 7);
            AddCustomer(db, 8);
            AddCustomer(db, 9);
            AddCustomer(db, 10);
            AddCustomer(db, 11);

            stats = db.GetTotalShardItems();
            Assert.AreEqual(4, stats[0].ItemsCount);
            Assert.AreEqual(3, stats[1].ItemsCount);
            Assert.AreEqual(2, stats[2].ItemsCount);
            Assert.AreEqual(1, stats[3].ItemsCount);
            Assert.AreEqual(1, stats[4].ItemsCount);            
        }

        private void AddCustomer(InMemoryDB db, int customerId)
        {
            var customer = new SampleCustomerInformation()
            {
                Age = 10,
                CustomerId = customerId,
                Name = "Mohsen"
            };
            db.AddCustomer(customer);
        }
    }
}
