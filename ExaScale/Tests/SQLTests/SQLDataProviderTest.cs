using ExaScale.Sharding.Common;
using ExaScale.Sharding.Common.Algorithms;
using ExaScale.Sharding.DataProvider;
using ExaScale.ShardManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnitTests.Common;
using UnitTests.SqlDataProvider;

namespace UnitTests
{
    [TestClass]
    public class SQLDataProviderTest : BaseTest
    {
        [TestMethod]
        public void AddShardItem_Test()
        {
            using (GetTransaction())
            {
                ShardConfiguration shardConfiguration = new ShardConfiguration(3);
                IShardKeyAlgorithm shardKeyAlgorithm = new EvenlyDistributedShardKeyAlgorithm(shardConfiguration);
                SqlDb db = new SqlDb(shardConfiguration, shardKeyAlgorithm);

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
        }

        private void AddCustomer(SqlDb db, int customerId)
        {
            var customer = new SampleCustomerInformation()
            {
                Birthdate = DateTime.Today.AddYears(-new Random().Next(5, 30)),
                CustomerId = customerId,
                Title = "Mohsen",
                Address = $"Sample Address {customerId}",
                Job = $"Senior Software Engineer # {customerId}"                
            };
            db.AddCustomer(customer);
        }
    }
}
