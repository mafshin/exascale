namespace ExaScale.Sharding.DataProvider
{
    public interface IConnectionProvider
    {
        string GetShardConnectionString(int shardId);
        string GetMainConnectionString();
    }
}