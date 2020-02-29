using ExaScale.Sharding.Common;
using ExaScale.Sharding.DataProvider;
using ExaScale.ShardManager;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UnitTests.Common;

namespace UnitTests.SqlDataProvider
{
    public class SqlDb : ShardedDb<SqlSubShardDataProvider>
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
    public class SqlMainShardDataProvider : IMainShardDataProvider
    {
        private IShardKeyAlgorithm _shardKeyAlgorithm;
        private ShardConfiguration _shardConfiguration;
        private string _connection;

        public SqlMainShardDataProvider(ShardConfiguration shardConfiguration, IShardKeyAlgorithm algorithm)
        {
            _shardConfiguration = shardConfiguration;
            _shardKeyAlgorithm = algorithm;
        }

        public void Initialize(string connection)
        {
            _connection = connection;
        }

        public void LoadShardMap(Dictionary<string, int> _shardMap)
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
                }
                finally
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr.DisposeAsync();
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
    }
    public class SqlSubShardDataProvider : ISqlSubShardDataProvider
    {
        private string _connection;

        public void AddCustomer(SampleCustomerInformation customer)
        {
            using (SqlConnection sqlConn = new SqlConnection(_connection))
            {
                sqlConn.Open();

                SqlCommand sqlCmd = new SqlCommand("AddCustomer", sqlConn);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                sqlCmd.Parameters.AddWithValue("@customerId", customer.CustomerId);
                sqlCmd.Parameters.AddWithValue("@title", customer.Title);
                sqlCmd.Parameters.AddWithValue("@birthdate", customer.Birthdate);
                sqlCmd.Parameters.AddWithValue("@address", customer.Address);
                sqlCmd.Parameters.AddWithValue("@job", customer.Job);

                sqlCmd.ExecuteNonQuery();
            }
        }

        public void Initialize(string connection)
        {
            this._connection = connection;
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
    }

    public interface ISqlSubShardDataProvider : ISubShardDataProvider
    {
        void AddCustomer(SampleCustomerInformation customer);
    }

}
