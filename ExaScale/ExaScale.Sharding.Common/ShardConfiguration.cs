using System;

namespace ExaScale.Sharding.Common
{
    public class ShardConfiguration
    {
        public int ShardCount { get; private set; }
        public ShardConfiguration(int shardCount)
        {
            this.ShardCount = shardCount;
        }
        public int IncreaseShardCount()
        {
            this.ShardCount++;

            return this.ShardCount;
        }

    }
}
