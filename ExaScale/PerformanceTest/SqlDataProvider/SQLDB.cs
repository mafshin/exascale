using ExaScale.Sharding.Common;
using ExaScale.Sharding.DataProvider;
using ExaScale.ShardManager;
using PerformanceTest.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace PerformanceTest.SqlDataProvider
{
    public class SqlDb : ShardedDb<SqlMainShardDataProvider, SqlSubShardDataProvider>
    {
        public SqlDb(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm) :
            base(shardConfiguration, algorithm,
            new SqlMainShardDataProvider(shardConfiguration, algorithm),
            new SqlConnectionProvider())
        {

        }

        protected new SqlSubShardDataProvider GetShard(string shardKey)
        {
            return base.GetShard(shardKey) as SqlSubShardDataProvider;
        }

        internal void AddCustomer(SampleCustomerInformation customer)
        {
            var shard = GetShard(customer.CustomerId.ToString());
            shard.AddCustomer(customer);
        }

        internal void AddOrder(SampleOrderInformation order)
        {
            var shard = GetShard(order.CustomerId.ToString());
            shard.AddOrder(order);
        }

        internal void ClearAll()
        {
            GetMainShard().ClearAll();

            var shards = GetAllShards();
            foreach (var shard in shards)
            {
                shard.ClearAll();
            }
        }
    }

    public class SqlConnectionProvider : IConnectionProvider
    {
        string dbName = "Customers";
        string format = @"Server=.;Database={0}-{1};Integrated Security=True";
        public string GetMainConnectionString()
        {
            return string.Format(format, dbName, "Main");
        }
        public string GetShardConnectionString(int shardId)
        {
            return string.Format(format, dbName, $"Shard{shardId:D2}");
        }
    }
    public class SqlMainShardDataProvider : IMainShardDataProvider, IDisposable, ISqlDataProvider
    {
        private IShardKeyAlgorithm _shardKeyAlgorithm;
        private ShardConfiguration _shardConfiguration;
        private string _connection;
        private SqlConnection sqlConn;

        public SqlMainShardDataProvider(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm)
        {
            _shardConfiguration = shardConfiguration;
            _shardKeyAlgorithm = algorithm;
        }

        public void Initialize(string connection)
        {
            _connection = connection;

            if (sqlConn == null)
                sqlConn = new SqlConnection(_connection);

            if (sqlConn.State != System.Data.ConnectionState.Open)
                sqlConn.Open();

        }

        public void LoadShardMap(List<int> shards, Dictionary<string, int> _shardMap)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("GetShardMap", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader dr = null;
                try
                {
                    dr = sqlCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var shardId = Convert.ToInt32(dr["ShardId"]);
                        var shardKey = dr["ShardKey"].ToString();

                        _shardMap.Add(shardKey, shardId);
                    }

                    dr.NextResult();
                    while (dr.Read())
                    {
                        var shardId = Convert.ToInt32(dr["ShardId"]);

                        shards.Add(shardId);
                    }
                }
                finally
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr.Dispose();
                    }
                }
            }
        }

        public void LoadShardMap(Dictionary<string, int> _shardMap)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("GetShards", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlDataReader dr = null;
                try
                {
                    dr = sqlCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var shardId = Convert.ToInt32(dr["ShardId"]);
                        var shardKey = dr["ShardKey"].ToString();

                        _shardMap.Add(shardKey, shardId);
                    }
                }
                finally
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr.Dispose();
                    }
                }
            }
        }

        public void AddShardKey(string shardKey, int shardId)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("AddShardKey", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter sqlShardIdParam = new SqlParameter("@shardId", System.Data.SqlDbType.Int);
                sqlShardIdParam.Value = shardId;

                SqlParameter sqlShardKeyParam = new SqlParameter("@shardKey", System.Data.SqlDbType.NVarChar, 100);
                sqlShardKeyParam.Value = shardKey;

                sqlCmd.Parameters.Add(sqlShardIdParam);
                sqlCmd.Parameters.Add(sqlShardKeyParam);

                sqlCmd.ExecuteNonQuery();
            }
        }

        public void AddShard(int shardId)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("AddShard", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter sqlShardIdParam = new SqlParameter("@shardId", shardId);

                sqlCmd.Parameters.Add(sqlShardIdParam);

                sqlCmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            if (sqlConn != null)
            {
                sqlConn.Close();
            }
        }
        public void ClearAll()
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("ClearAll", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCmd.ExecuteNonQuery();
            }
        }
    }
    public class SqlSubShardDataProvider : ISqlSubShardDataProvider
    {
        private string _connection;
        private SqlConnection sqlConn;

        public void AddCustomer(SampleCustomerInformation customer)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("AddCustomer", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCmd.Parameters.AddWithValue("@customerId", customer.CustomerId);
                sqlCmd.Parameters.AddWithValue("@title", customer.Title);
                sqlCmd.Parameters.AddWithValue("@birthdate", customer.Birthdate.Date);
                sqlCmd.Parameters.AddWithValue("@address", customer.Address);
                sqlCmd.Parameters.AddWithValue("@job", customer.Job);

                sqlCmd.ExecuteNonQuery();
            }
        }

        public void Initialize(string connection)
        {
            this._connection = connection;

            if (sqlConn == null)
                sqlConn = new SqlConnection(_connection);

            if (sqlConn.State != System.Data.ConnectionState.Open)
                sqlConn.Open();
        }

        public long GetShardItemsCount()
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("GetShardItemsCount", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                var ret = sqlCmd.ExecuteScalar();

                return Convert.ToInt64(ret.ToString());
            }
        }

        public void Dispose()
        {
            if (sqlConn != null)
            {
                sqlConn.Close();
            }
        }

        internal void AddOrder(SampleOrderInformation order)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("AddOrder", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCmd.Parameters.AddWithValue("@customerId", order.CustomerId);
                sqlCmd.Parameters.AddWithValue("@count", order.Count);
                sqlCmd.Parameters.AddWithValue("@price", order.Price);
                sqlCmd.Parameters.AddWithValue("@description", order.Description);
                sqlCmd.Parameters.AddWithValue("@orderEntryDate", order.OrderEntryDate);

                sqlCmd.ExecuteNonQuery();
            }
        }

        public void ClearAll()
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("ClearAll", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCmd.ExecuteNonQuery();
            }
        }
    }

    public interface ISqlSubShardDataProvider : ISubShardDataProvider, ISqlDataProvider
    {
        void AddCustomer(SampleCustomerInformation customer);
    }

}
