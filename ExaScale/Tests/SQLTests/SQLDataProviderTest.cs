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


                var stats = db.GetTotalShardItems();
                Assert.AreEqual(1, stats[0].ItemsCount);
                Assert.AreEqual(1, stats[1].ItemsCount);
                Assert.AreEqual(1, stats[2].ItemsCount);
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
