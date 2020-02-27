namespace ExaScale.Sharding.DataProvider
{
    public interface IConnectionProvider
    {
        string GetConnectionString(int shardId);
    }
}