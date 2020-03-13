using ExaScale.Sharding.Common;
using ExaScale.Sharding.Common.Algorithms;
using ExaScale.ShardManager;
using PerformanceTest.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PerfTest();

            Console.ReadKey();
        }

        public static void PerfTest()
        {
            //RunTest(1, 100000, 10);
            RunTest(5, 10000, 10);
            //RunTest(2, 1000, 100);
            //RunTest(3, 1000, 10);
            //RunTest(4, 1000);
            ///RunTest(5, 100000, 100);
        }

        public static void RunTest(int shardCount, int itemsCount, int customerItemsCount)
        {
            int totalSuccess = 0;
            var tasks = new List<Task>();
            var factory = new System.Threading.Tasks.TaskFactory();
            //using (GetTransaction())
            //{
            Console.WriteLine($"Running test with {shardCount} shards and {itemsCount} items and {customerItemsCount} customer items");

            ShardConfiguration shardConfiguration = new ShardConfiguration(shardCount);
            IShardKeyAlgorithm shardKeyAlgorithm = new EvenlyDistributedShardKeyAlgorithm(shardConfiguration);
            using (SqlDb db = new SqlDb(shardConfiguration, shardKeyAlgorithm))
            {
                try
                {
                    //db.ClearAll();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (int i = 0; i < itemsCount; i++)
                    {
                        try
                        {
                            db.AddCustomer(new Common.SampleCustomerInformation()
                            {
                                CustomerId = i,
                                Title = $"Customer {i}",
                                Address = "Address one + two",
                                Birthdate = DateTime.Now,
                                Job = "Jobs of the future is related to data"
                            });
                            Interlocked.Increment(ref totalSuccess);
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    sw.Stop();
                    var avg = sw.ElapsedMilliseconds / itemsCount;

                    Console.WriteLine($"ShardCount: {shardCount}, Insert Count: {itemsCount}, Avg: {avg}, Total Success: {totalSuccess}");


                    sw.Reset();
                    sw.Start();
                    var list = Enumerable.Range(0, itemsCount).ToList();

                    Parallel.ForEach(list, (customerId) =>
                     {
                         //foreach (var customerId in list)
                         //{
                         for (int i = 0; i < customerItemsCount; i++)
                         {
                             try
                             {
                                 db.AddOrder(new Common.SampleOrderInformation()
                                 {
                                     CustomerId = customerId,
                                     Count = customerId + i + 10,
                                     Price = (customerId + i + 3453) * 10,
                                     Description = "some order detail",
                                     OrderEntryDate = DateTime.Now
                                 });
                             }
                             catch (Exception ex)
                             {

                             }
                         }
                     });
                    //}

                    sw.Stop();

                    avg = sw.ElapsedMilliseconds / itemsCount;

                    Console.WriteLine($"ShardCount: {shardCount}, Insert Count: {itemsCount * customerItemsCount}, Avg: {avg}, Total: {sw.Elapsed.TotalSeconds:0.0}");
                }
                finally
                {
                    db.ClearAll();
                }
            }
        }

        static TransactionScope transaction;

        public static System.Transactions.TransactionScope GetTransaction()
        {
            transaction = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew);
            return transaction;
        }
    }
}
