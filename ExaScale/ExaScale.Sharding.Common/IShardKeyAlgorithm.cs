using System;

namespace ExaScale.ShardManager
{
    public interface IShardKeyAlgorithm
    {
        int GetShardId(string shardKey);
    }
}
