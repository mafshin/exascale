using ExaScale.ShardManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaScale.Sharding.Common.Algorithms
{
    public class EvenlyDistributedShardKeyAlgorithm : IShardKeyAlgorithm
    {
        private ShardConfiguration shardConfiguration;

        public EvenlyDistributedShardKeyAlgorithm(ShardConfiguration shardConfiguration)
        {
            this.shardConfiguration = shardConfiguration;
        }

        public int GetShardId(string shardKey)
        {
            var inputId = int.Parse(shardKey);
            var shardId = inputId % shardConfiguration.ShardCount;
            if(shardId == 0)
            {
                shardId = shardConfiguration.ShardCount;
            }
            return shardId;
        }
    }
}
